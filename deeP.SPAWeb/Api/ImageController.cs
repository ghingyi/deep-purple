using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.SPAWeb.Constants;
using deeP.SPAWeb.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace deeP.SPAWeb.Api
{
    [RoutePrefix("api/images")]
    public class ImageController : ApiController
    {
        private IImageRepository ImageRepository { get; set; }
        private ILoggingService LoggingService { get; set; }

        /// <summary>
        /// Stores an image sent as a multiplart form.
        /// </summary>
        /// <remarks>
        /// Notes: we assume jpg and doesn't resize them in this sample.
        /// (Not to mention that they end up stored in SQL...)
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = RoleName.Seller)]
        [Route("storeimage")]
        public async Task<IHttpActionResult> StoreImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Unsupported media type.");

            try
            {
                MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // Define query for files in multipart form
                var files = provider.Contents.Where(c =>
                    {
                        // c is a file if there's ContentDisposition header with a filename
                        ContentDispositionHeaderValue contentDisposition = c.Headers.ContentDisposition;
                        return contentDisposition != null && !String.IsNullOrEmpty(contentDisposition.FileName);
                    });

                List<string> imageIds = new List<string>();
                foreach (var file in files)
                {
                    // Get file stream
                    Stream fileStream = await file.ReadAsStreamAsync();

                    // Store file with repository
                    string imageId = await ImageRepository.StoreImageAsync(fileStream);

                    // Remember it's Uri
                    imageIds.Add(imageId);
                }

                return Ok(imageIds);
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetImage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                BadRequest("Image id cannot be empty.");

            try
            {
                // Get jpg stream from repository
                Stream jpgStream = await ImageRepository.GetImageStreamAsync(id);
                return new StreamActionResult(jpgStream, new MediaTypeHeaderValue("image/jpeg"));
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        protected IHttpActionResult GetErrorResult(Exception exception)
        {
            RepositoryException repEx = exception as RepositoryException;
            if (repEx != null)
            {
                // Let's return the response most closely describing the problem
                switch (repEx.ErrorCode)
                {
                    case RepositoryErrorCode.Conflict:
                        return Conflict();
                    case RepositoryErrorCode.NotFound:
                        return NotFound();
                    default:
                        ModelState.AddModelError("", repEx.Message);
                        return BadRequest(ModelState);
                }
            }
            else if (!ModelState.IsValid)
            {
                // Some model state issue could have caused the exception/error
                return BadRequest(ModelState);
            }
            else
            {
                // We do not want to reveal arbitrary exception messages
                return InternalServerError();
            }
        }

        public ImageController(IImageRepository imageRepository, ILoggingService loggingService)
        {
            this.ImageRepository = imageRepository;
            this.LoggingService = loggingService;
        }

        #region Nested types

        public class StreamActionResult : IHttpActionResult
        {
            public Stream Stream { get; private set; }
            public MediaTypeHeaderValue MediaTypeHeaderValue { get; private set; }

            public StreamActionResult(Stream stream, MediaTypeHeaderValue mediaTypeHeaderValue)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                this.Stream = stream;
                this.MediaTypeHeaderValue = mediaTypeHeaderValue;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StreamContent(this.Stream);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                if (this.MediaTypeHeaderValue != null)
                {
                    response.Content.Headers.ContentType = this.MediaTypeHeaderValue;
                }

                return Task.FromResult(response);
            }
        }

        #endregion
    }
}
