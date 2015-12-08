using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.Repositories.SQL.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL
{
    public sealed class SqlImageRepository : IImageRepository
    {
        public async Task<string> StoreImageAsync(Stream jpgStream)
        {
            if (jpgStream == null)
                throw new ArgumentNullException("jpgStream");

            if (!jpgStream.CanRead)
                throw new ArgumentException("The specified jpg stream cannot be read.", "jpgStream");

            // Read image into buffer
            byte[] buffer = await ReadJpgImageIntoBuffer(jpgStream);

            using (var context = CreateContext())
            {
                // Create entity
                Image image = context.Images.Create();
                image.ImageData = buffer;

                // Save changes
                context.Images.Add(image);
                await context.SaveChangesAsync();

                return image.Id.ToString();
            }
        }

        public async Task<Stream> GetImageStreamAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            int imageId;
            if (int.TryParse(id, out imageId))
            {
                Image image;

                using (var context = CreateContext())
                {
                    image = await context.Images.FindAsync(imageId);
                }
                if (image == null)
                    throw new RepositoryException(RepositoryErrorCode.NotFound, string.Format("Image for Id '{0}' could not be found.", imageId));

                return new MemoryStream(image.ImageData);
            }
            else
            {
                throw new ArgumentException("Invalid image model identifier value.", "imageModel");
            }
        }

        #region Private methods

        private static async Task<byte[]> ReadJpgImageIntoBuffer(Stream jpgStream)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                // We cannot assume the jpg stream to be seekable with a known length
                // Thus we copy and copy again :(
                await jpgStream.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                buffer = new byte[ms.Length];
                await ms.ReadAsync(buffer, 0, buffer.Length);
            };
            return buffer;
        }

        private Func<deePContext> CreateContext;

        #endregion

        #region Constructor logic

        public SqlImageRepository(string connectionStringOrName = null)
        {
            this.CreateContext = () => deePContext.Create(connectionStringOrName);
        }

        #endregion
    }
}
