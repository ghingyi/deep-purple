using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.Repositories.SQL.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL
{
    public sealed partial class SqlPropertyRepository : IPropertyRepository
    {
        private Func<deePContext> CreateContext;

        public SqlPropertyRepository(string connectionStringOrName = null)
        {
            this.CreateContext = () => deePContext.Create(connectionStringOrName);
        }
    }
}
