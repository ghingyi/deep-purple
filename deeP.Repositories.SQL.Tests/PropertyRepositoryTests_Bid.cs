using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using deeP.Repositories.SQL.Context;
using System.Text;
using deeP.Abstraction.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using deeP.Abstraction;

namespace deeP.Repositories.SQL.Tests
{
    public partial class PropertyRepositoryTests : DbTestClass
    {
        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository will detect null model parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateBid_NullPropertyModel()
        {
            await this.PropertyRepository.CreateBidAsync(null, "dummy");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository will detect null user parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateBid_NullPropertyOwner()
        {
            await this.PropertyRepository.CreateBidAsync(new BidModel() { Id = "Dummy" }, null);
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can create a bid for a property.")]
        public async Task CreateBid_Simple()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");

            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModel.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(bidModel.State, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can detect missing required fields.")]
        public async Task CreateBid_MissingRequiredField()
        {
            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                //PropertyId = "Something valid",
                Price = 1234,
                State = BidState.Open
            };

            try
            {
                await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Validation, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can detect invalid property references.")]
        public async Task CreateBid_InvalidField()
        {
            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = "Something invalid",
                Price = 1234,
                State = BidState.Open
            };

            try
            {
                await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.General, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can detect dubplicate ids.")]
        public async Task CreateBid_DuplicateId()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");

            try
            {
                await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.General, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository will detect updating with null model parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateBid_NullPropertyModel()
        {
            await this.PropertyRepository.UpdateBidAsync(null, "dummy");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository will detect updating with null user parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateBid_NullPropertyOwner()
        {
            await this.PropertyRepository.UpdateBidAsync(new BidModel() { Id = "Dummy" }, null);
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can prevent unauthorized changes.")]
        public async Task UpdateBid_PreventUnauthorizedChanges()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy");

            // Create completely new model with the same Id
            BidModel bidModel2 = new BidModel()
            {
                Id = bidModel.Id,
                PropertyId = propertyId,
                Price = 4567,
                State = BidState.Accepted
            };

            try
            {
                // We use a different owner user
                await this.PropertyRepository.UpdateBidAsync(bidModel2, "someOtherGuyOrGirl");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Unauthorized, re.ErrorCode);
            }

            // Ensure no changes were made before the exception (well, just to assume there could be :) )
            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModel.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(bidModel.State, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can update a bid by replacing most properties.")]
        public async Task UpdateBid_PropertySubstantially()
        {
            // Create the setup by the following test
            await CreateBid_Simple();

            Bid bid;
            using (var context = CreateContext())
            {
                // Read the only bid in the database
                bid = context.Bids.Single();
            }

            // Create additional property
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Taken,
                ImageInfos = null
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            // Create completely new model with the same Id
            BidModel bidModel = new BidModel()
            {
                Id = bid.Id,
                PropertyId = propertyModel.Id,
                Price = 4567,
                State = BidState.Accepted
            };

            // We use the same user
            await this.PropertyRepository.UpdateBidAsync(bidModel, "dummy");

            // Ensure no changes were made before the exception (well, just to assume there could be :) )
            using (var context = CreateContext())
            {
                bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModel.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(bidModel.State, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(2, context.Properties.Count(), "We expected to find two properties.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can be accepted by the property owner.")]
        public async Task UpdateBid_CloseByOwner()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy2");

            // Change model state to Accepted
            bidModel.State = BidState.Accepted;

            // We use the owner of the property
            await this.PropertyRepository.UpdateBidAsync(bidModel, "dummy");

            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModel.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(bidModel.State, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can reject other open bids when a property owner accepts one.")]
        public async Task UpdateBid_AcceptOneRejectOthers()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModelAccepted = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModelAccepted, "winner");

            BidModel bidModelRejected = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1233,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModelRejected, "loser");

            // Change model state of the designated bid to Accepted
            bidModelAccepted.State = BidState.Accepted;

            // We use the owner of the property
            await this.PropertyRepository.UpdateBidAsync(bidModelAccepted, "dummy");

            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModelAccepted.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModelAccepted.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModelAccepted.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(BidState.Accepted, bid.State, "We expected states to be changed to accepted.");

                bid = await context.Bids.FindAsync(bidModelRejected.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModelRejected.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModelRejected.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(BidState.Rejected, bid.State, "We expected state to be changed to rejected.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(2, context.Bids.Count(), "We expect to find two bids.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can prevent changing the price by the owner.")]
        public async Task UpdateBid_PreventPriceChangeByOwner()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy2");

            // Change price
            bidModel.Price = 4567;

            try
            {
                // We use the owner of the property
                await this.PropertyRepository.UpdateBidAsync(bidModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Unauthorized, re.ErrorCode);
            }

            // Ensure no changes were made before the exception (well, just to assume there could be :) )
            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(1234, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(bidModel.State, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can prevent reopening an already closed bid.")]
        public async Task UpdateBid_PreventReopeningBid()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel, "dummy2");

            // Accept bid
            bidModel.State = BidState.Accepted;

            // We use the owner of the property
            await this.PropertyRepository.UpdateBidAsync(bidModel, "dummy");

            // Try to open the bid back
            bidModel.State = BidState.Open;

            try
            {
                // We use the owner of the bid, trying to reopen it
                await this.PropertyRepository.UpdateBidAsync(bidModel, "dummy2");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Unauthorized, re.ErrorCode);
            }

            // Ensure no changes were made before the exception (well, just to assume there could be :) )
            using (var context = CreateContext())
            {
                Bid bid = await context.Bids.FindAsync(bidModel.Id);

                Assert.IsNotNull(bid, "We expected a bid to be stored.");

                Assert.AreEqual(bidModel.PropertyId, bid.PropertyId, "We expected property property ids to remain unchanged once stored.");
                Assert.AreEqual(bidModel.Price, bid.Price, "We expected prices types to remain unchanged once stored.");
                Assert.AreEqual(BidState.Accepted, bid.State, "We expected states to remain unchanged once stored.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(1, context.Bids.Count(), "We expect to find only one bid.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }
    }
}
