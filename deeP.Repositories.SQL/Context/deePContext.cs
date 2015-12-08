using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL.Context
{
    public class deePContext : DbContext
    {
        public static deePContext Create(string connectionString = null)
        {
            return connectionString != null ? new deePContext(connectionString) : new deePContext();
        }

        public DbSet<Bid> Bids { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageInfo> ImageInfos { get; set; }
        public DbSet<Property> Properties { get; set; }

        public deePContext()
            : base("DefaultConnection")
        {
        }

        public deePContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
