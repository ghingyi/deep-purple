using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.SPAWeb.Api.Models;
using deeP.SPAWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace deeP.SPAWeb.Api
{
    [Authorize]
    [RoutePrefix("api/properties")]
    public class PropertyController : ApiController
    {
        private IPropertyRepository PropertyRepository { get; set; }

        private ILoggingService LoggingService { get; set; }

        /// <summary>
        /// Adds a property listing.
        /// </summary>
        /// <param name="propertyModel">Model item of property to add.</param>
        [Route("addproperty")]
        public async Task<IHttpActionResult> AddProperty(PropertyModel propertyModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await PropertyRepository.CreatePropertyAsync(propertyModel, User.Identity.Name);
                return Ok();
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        /// Edits a property listing.
        /// </summary>
        /// <param name="propertyModel">Model item of property to add.</param>
        [Route("editproperty")]
        public async Task<IHttpActionResult> EditProperty(PropertyModel propertyModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await PropertyRepository.UpdatePropertyAsync(propertyModel, User.Identity.Name);
                return Ok();
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        /// Adds or updates a bid for a property.
        /// </summary>
        /// <param name="propertyModel">Model item of bid to add or update.</param>
        [Route("addbid")]
        public async Task<IHttpActionResult> AddBid(BidModel bidModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await PropertyRepository.CreateBidAsync(bidModel, User.Identity.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        ///  Accept or reject a bid.
        /// </summary>
        /// <param name="closeBidModel">Model item of bid close operation.</param>
        [Route("closebid")]
        public async Task<IHttpActionResult> CloseBid(CloseBidModel closeBidModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                closeBidModel.Bid.State = closeBidModel.Accept ? BidState.Accepted : BidState.Rejected;

                await PropertyRepository.UpdateBidAsync(closeBidModel.Bid, User.Identity.Name);
                return Ok();
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        ///  Queries for properties that match the filter.
        /// </summary>
        /// <param name="queryPropertiesModel">Model of property query operation.</param>
        /// <returns>Properties found for the filter ordered by the requested sorting.</returns>
        /// <remarks>
        /// Notes: typically used either to query properties listed by a seller or
        /// to query properties that are still open for bid by buyers.
        /// </remarks>
        [Route("queryproperties")]
        public async Task<IHttpActionResult> QueryProperties(QueryPropertiesModel queryPropertiesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IEnumerable<PropertyModel> properties = await PropertyRepository.QueryPropertiesAsync(queryPropertiesModel.Filter, queryPropertiesModel.Sorting);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        ///  Queries for bids that match the filter.
        /// </summary>
        /// <param name="filter">Filter to use for the query.</param>
        /// <returns>Bids found for the filter.</returns>
        /// <remarks>
        /// Notes: typically used either to query bids posted by a given buyer or
        /// to query open bids posted for any of the properties of a given seller.
        /// </remarks>
        [Route("querybids")]
        public async Task<IHttpActionResult> QueryBids(BidFilter filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IEnumerable<BidModel> bids = await PropertyRepository.QueryBidsAsync(filter);
                return Ok(bids);
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

        public PropertyController(IPropertyRepository propertyRepository, ILoggingService loggingService)
        {
            this.PropertyRepository = propertyRepository;
            this.LoggingService = loggingService;
        }
    }
}
