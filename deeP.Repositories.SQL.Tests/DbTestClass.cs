using deeP.Abstraction;
using deeP.Repositories.SQL.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL.Tests
{
    [DeploymentItem(@".\Data\deePdb.mdf")]
    [DeploymentItem(@".\Data\deePdb_log.ldf")]
    [TestClass]
    public abstract class DbTestClass
    {
        private TestContext _testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        private string ConnectionString;

        protected deePContext CreateContext()
        {
            return new deePContext(this.ConnectionString);
        }

        protected IImageRepository ImageRepository { get; private set; }
        protected IPropertyRepository PropertyRepository { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            string dbFilePath = Path.Combine(TestContext.TestDeploymentDir, "deePdb.mdf");
            this.ConnectionString = string.Format(@"Data Source=(LocalDB)\v11.0;AttachDbFilename={0};Initial Catalog=deeP;Integrated Security=SSPI;", dbFilePath);

            this.ImageRepository = new SqlImageRepository(this.ConnectionString);
            this.PropertyRepository = new SqlPropertyRepository(this.ConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = CreateContext())
            {
               // Notes: cannot copy/delete files because LocalDb is slow to release
               // Cannot truncate tables either due to foreign key references

                context.Database.ExecuteSqlCommand("DELETE FROM [Image]");
                context.Database.ExecuteSqlCommand("DELETE FROM [ImageInfo]");
                context.Database.ExecuteSqlCommand("DELETE FROM [Bid]");
                context.Database.ExecuteSqlCommand("DELETE FROM [Property]");
            }
        }
    }
}
