using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL
{
    /// <summary>
    /// This class is required to force MS Test to copy the provider assembly and start properly.
    /// </summary>
    internal static class MissingDllHack
    {
        private static SqlProviderServices instance = SqlProviderServices.Instance;
    }
}
