using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.Repositories.SQL.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL
{
    public sealed partial class SqlPropertyRepository : IPropertyRepository
    {
        public async Task<IEnumerable<PropertyModel>> QueryPropertiesAsync(PropertyFilter filter, PropertySorting sorting)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BidModel>> QueryBidsAsync(BidFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
