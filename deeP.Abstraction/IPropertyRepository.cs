using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction
{
    public interface IPropertyRepository
    {
        // Property handling methods

        /// <summary>
        /// Adds a new property to the database.
        /// </summary>
        /// <param name="propertyModel">Model of property to add.</param>
        /// <param name="userName">User name of owner.</param>
        Task CreatePropertyAsync(PropertyModel propertyModel, string userName);

        /// <summary>
        /// Updates a property in the database.
        /// </summary>
        /// <param name="propertyModel">Model of property to update.</param>
        /// <param name="userName">User name of owner.</param>
        Task UpdatePropertyAsync(PropertyModel propertyModel, string userName);

        // Well, we only need two letters from CRUD :)

        /// <summary>
        /// Queries properties from the database.
        /// </summary>
        /// <param name="filter">Filter to restrict the query on.</param>
        /// <param name="sorting">Sorting to be applied to the results.</param>
        /// <returns>A collection of <see cref="PropertyModel"/> instances representing the result set.</returns>
        Task<IEnumerable<PropertyModel>> QueryPropertiesAsync(PropertyFilter filter, PropertySorting sorting);

        // Bid handling methods

        /// <summary>
        /// Adds a new bid to the database.
        /// </summary>
        /// <param name="bidModel">Model of bid to add.</param>
        /// <param name="userName">User name of owner.</param>
        Task CreateBidAsync(BidModel bidModel, string userName);

        /// <summary>
        /// Updates a bid for a property in the database along with associated entities.
        /// </summary>
        /// <param name="bidModel">Model of property to update.</param>
        /// <param name="userName">User name of owner.</param>
        /// <remarks>
        /// If the bid status changes from open to accepted, the operation rejects all outstanding bids for the same property.
        /// </remarks>
        Task UpdateBidAsync(BidModel bidModel, string userName);

        /// <summary>
        /// Queries bids from the database.
        /// </summary>
        /// <param name="filter">Filter to restrict the query on.</param>
        /// <returns>A collection of <see cref="BidModel"/> instances representing the result set.</returns>
        Task<IEnumerable<BidModel>> QueryBidsAsync(BidFilter filter);
    }
}
