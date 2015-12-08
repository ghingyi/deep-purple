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
    [TestClass]
    public partial class PropertyRepositoryTests : DbTestClass
    {
        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository will detect null model parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateProperty_NullPropertyModel()
        {
            await this.PropertyRepository.CreatePropertyAsync(null, "dummy");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository will detect null user parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateProperty_NullPropertyOwner()
        {
            await this.PropertyRepository.CreatePropertyAsync(new PropertyModel() { Id = "Dummy" }, null);
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can detect the creation of properties as unavailable.")]
        public async Task CreateProperty_TakenState()
        {
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

            try
            {
                await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Validation, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can create a property without images.")]
        public async Task CreateProperty_NoImages()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Open,
                ImageInfos = null
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            using (var context = CreateContext())
            {
                Property property = await context.Properties.FindAsync(propertyModel.Id);

                Assert.IsNotNull(property, "We expected a property to be stored.");

                Assert.AreEqual(propertyModel.Description, property.Description, "We expected property descriptions to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Type, property.Type, "We expected property types to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Bedrooms, property.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Price, property.Price, "We expected property prices to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Address, property.Address, "We expected property addresses to remain unchanged once stored.");
                Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), property.LocationDetails, "We expected property location details to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.State, property.State, "We expected property location state to remain unchanged once stored.");
                Assert.AreEqual(0, property.ImageInfos.Count, "We expected to find no images associated with the property.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can detect missing required fields.")]
        public async Task CreateProperty_MissingRequiredField()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString()
            };

            try
            {
                await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Validation, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can detect invalid fields.")]
        public async Task CreateProperty_InvalidField()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = string.Join(",", Enumerable.Range(1, 1000)),   // This is too long for address
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Taken,
                ImageInfos = null
            };

            try
            {
                await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Validation, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can detect dubplicate ids.")]
        public async Task CreateProperty_DuplicateId()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Open,
                ImageInfos = null
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            try
            {
                await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.General, re.ErrorCode);
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can create a property with an array of images.")]
        public async Task CreateProperty_TwoImages()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Open,
                ImageInfos = new ImageInfoModel[] { 
                    new ImageInfoModel(){
                        Id = Guid.NewGuid().ToString(),
                        Title ="First image",
                        Uri = "http://www.prettysurethisdoesntexists.com"
                    },
                    new ImageInfoModel(){
                        Id = Guid.NewGuid().ToString(),
                        Title ="Second image",
                        Uri = "http://www.iwasrightthefirsttime.com"
                    }
                }
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            using (var context = CreateContext())
            {
                Property property = await context.Properties.FindAsync(propertyModel.Id);

                Assert.IsNotNull(property, "We expected a property to be stored.");

                Assert.AreEqual(propertyModel.Description, property.Description, "We expected property descriptions to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Type, property.Type, "We expected property types to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Bedrooms, property.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Price, property.Price, "We expected property prices to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Address, property.Address, "We expected property addresses to remain unchanged once stored.");
                Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), property.LocationDetails, "We expected property location details to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.State, property.State, "We expected property location state to remain unchanged once stored.");
                Assert.AreEqual(2, property.ImageInfos.Count, "We expected to find two images associated with the property.");

                ImageInfo[] imageInfos = property.ImageInfos.OrderBy(ii => ii.Order).ToArray();
                Assert.AreEqual(propertyModel.ImageInfos[0].Id, imageInfos[0].Id, "We expected the image info ids to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Title, imageInfos[0].Title, "We expected the image info titles to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Uri, imageInfos[0].Uri, "We expected the image info uris to remain the same after storage.");
                Assert.AreEqual(0, imageInfos[0].Order, "We expected the first image info to get order 0 during storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Id, imageInfos[1].Id, "We expected the image info ids to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Title, imageInfos[1].Title, "We expected the image info titles to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Uri, imageInfos[1].Uri, "We expected the image info uris to remain the same after storage.");
                Assert.AreEqual(1, imageInfos[1].Order, "We expected the second image info to get order 1 during storage.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(2, context.ImageInfos.Count(), "We expected to find two image infos.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository will detect updating with null model parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateProperty_NullPropertyModel()
        {
            await this.PropertyRepository.UpdatePropertyAsync(null, "dummy");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository will detect updating with null user parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateProperty_NullPropertyOwner()
        {
            await this.PropertyRepository.UpdatePropertyAsync(new PropertyModel() { Id = "Dummy" }, null);
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can prevent unauthorized changes.")]
        public async Task UpdateProperty_PreventUnauthorizedChanges()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Open,
                ImageInfos = null
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            // Create completely new model with the same Id
            PropertyModel propertyModel2 = new PropertyModel()
            {
                Id = propertyModel.Id,
                Description = "The id is the only thing we keep!",
                Type = PropertyType.Flat,
                Bedrooms = 4,
                Price = 5678,
                Address = "Omicron Persei 8",
                LocationDetails = new Dictionary<string, object>() { { "Good news everyone!", "Nah." } },
                State = PropertyState.Open,
                ImageInfos = new ImageInfoModel[] { 
                        new ImageInfoModel(){
                            Id = Guid.NewGuid().ToString(),
                            Title ="Some new image",
                            Uri = "http://www.reallyexhasustingtothinkofdomains.com"
                        }
                    }
            };

            try
            {
                // We use a different owner user
                await this.PropertyRepository.UpdatePropertyAsync(propertyModel2, "someOtherGuyOrGirl");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Unauthorized, re.ErrorCode);
            }

            // Ensure no changes were made before the exception (well, just to assume there could be :) )
            using (var context = CreateContext())
            {
                Property property = await context.Properties.FindAsync(propertyModel.Id);

                Assert.IsNotNull(property, "We expected a property to be stored.");

                Assert.AreEqual(propertyModel.Description, property.Description, "We expected property descriptions to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Type, property.Type, "We expected property types to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Bedrooms, property.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Price, property.Price, "We expected property prices to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Address, property.Address, "We expected property addresses to remain unchanged once stored.");
                Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), property.LocationDetails, "We expected property location details to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.State, property.State, "We expected property location state to remain unchanged once stored.");
                Assert.AreEqual(0, property.ImageInfos.Count, "We expected to find no images associated with the property.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can update a property by replacing most properties and all images.")]
        public async Task UpdateProperty_PropertySubstantially()
        {
            // Create the setup by the following test
            await CreateProperty_TwoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            // Create completely new model with the same Id
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = propertyId,
                Description = "The id is the only thing we keep!",
                Type = PropertyType.Flat,
                Bedrooms = 4,
                Price = 5678,
                Address = "Omicron Persei 8",
                LocationDetails = new Dictionary<string, object>() { { "Good news everyone!", "Nah." } },
                State = PropertyState.Open,
                ImageInfos = new ImageInfoModel[] { 
                        new ImageInfoModel(){
                            Id = Guid.NewGuid().ToString(),
                            Title ="Replacement image",
                            Uri = "http://www.exhasustingtothinkofdomains.com"
                        }
                    }
            };

            // We use the same owner user
            await this.PropertyRepository.UpdatePropertyAsync(propertyModel, "dummy");

            using (var context = CreateContext())
            {
                Property property = await context.Properties.FindAsync(propertyModel.Id);

                Assert.IsNotNull(property, "We expected a property to be stored.");

                Assert.AreEqual(propertyModel.Description, property.Description, "We expected property descriptions to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Type, property.Type, "We expected property types to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Bedrooms, property.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Price, property.Price, "We expected property prices to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Address, property.Address, "We expected property addresses to remain unchanged once stored.");
                Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), property.LocationDetails, "We expected property location details to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.State, property.State, "We expected property location state to remain unchanged once stored.");
                Assert.AreEqual(1, property.ImageInfos.Count, "We expected to find one image associated with the property.");

                ImageInfo[] imageInfos = property.ImageInfos.ToArray();
                Assert.AreEqual(propertyModel.ImageInfos[0].Id, imageInfos[0].Id, "We expected the image info ids to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Title, imageInfos[0].Title, "We expected the image info titles to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Uri, imageInfos[0].Uri, "We expected the image info uris to remain the same after storage.");
                Assert.AreEqual(0, imageInfos[0].Order, "We expected the first image info to get order 0 during storage.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(1, context.ImageInfos.Count(), "We expected to find two image infos.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Property")]
        [Description("Test to see if the repository can update a property by changing the order of its two images and their titles.")]
        public async Task UpdateProperty_ImageOrderAndTitles()
        {
            // Create the setup by the following test
            await CreateProperty_TwoImages();

            Property property;
            PropertyModel propertyModel;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                property = context.Properties.Single();

                // Create completely new model with the same Id
                propertyModel = new PropertyModel()
                {
                    Id = property.Id,
                    Description = property.Description,
                    Type = property.Type,
                    Bedrooms = property.Bedrooms,
                    Price = property.Price,
                    Address = property.Address,
                    LocationDetails = JsonConvert.DeserializeObject<Dictionary<string, object>>(property.LocationDetails),
                    State = property.State,
                    ImageInfos = new ImageInfoModel[] { 
                        new ImageInfoModel(){
                            Id = property.ImageInfos.Last().Id,
                            Title ="New title for former second image",
                            Uri = property.ImageInfos.Last().Uri
                        },
                        new ImageInfoModel(){
                            Id = property.ImageInfos.First().Id,
                            Title ="New title for former first image",
                            Uri = property.ImageInfos.First().Uri
                        }
                    }
                };
            }

            // We use the same owner user
            await this.PropertyRepository.UpdatePropertyAsync(propertyModel, "dummy");

            using (var context = CreateContext())
            {
                property = await context.Properties.FindAsync(propertyModel.Id);

                Assert.IsNotNull(property, "We expected a property to be stored.");

                Assert.AreEqual(propertyModel.Description, property.Description, "We expected property descriptions to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Type, property.Type, "We expected property types to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Bedrooms, property.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Price, property.Price, "We expected property prices to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.Address, property.Address, "We expected property addresses to remain unchanged once stored.");
                Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), property.LocationDetails, "We expected property location details to remain unchanged once stored.");
                Assert.AreEqual(propertyModel.State, property.State, "We expected property location state to remain unchanged once stored.");
                Assert.AreEqual(2, property.ImageInfos.Count, "We expected to find two images associated with the property.");

                ImageInfo[] imageInfos = property.ImageInfos.OrderBy(ii => ii.Order).ToArray();
                Assert.AreEqual(propertyModel.ImageInfos[0].Id, imageInfos[0].Id, "We expected the image info ids to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Title, imageInfos[0].Title, "We expected the image info titles to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[0].Uri, imageInfos[0].Uri, "We expected the image info uris to remain the same after storage.");
                Assert.AreEqual(0, imageInfos[0].Order, "We expected the first image info to get order 0 during storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Id, imageInfos[1].Id, "We expected the image info ids to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Title, imageInfos[1].Title, "We expected the image info titles to remain the same after storage.");
                Assert.AreEqual(propertyModel.ImageInfos[1].Uri, imageInfos[1].Uri, "We expected the image info uris to remain the same after storage.");
                Assert.AreEqual(1, imageInfos[1].Order, "We expected the second image info to get order 1 during storage.");

                Assert.AreEqual(1, context.Properties.Count(), "We expected to find only one property.");
                Assert.AreEqual(0, context.Images.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(2, context.ImageInfos.Count(), "We expected to find two image infos.");
            }
        }


        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Bid")]
        [Description("Test to see if the repository can detect attempts to change a property that was taken off the market.")]
        public async Task UpdateProperty_TakenProperty()
        {
            PropertyModel propertyModel = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Comfy, quiet residence",
                Type = PropertyType.House,
                Bedrooms = 3,
                Price = 1234,
                Address = "LV426",
                LocationDetails = new Dictionary<string, object>() { { "cartridge", 0 } },
                State = PropertyState.Open,
                ImageInfos = null
            };

            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            BidModel bidModelAccepted = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyModel.Id,
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModelAccepted, "winner");

            // Change model state of the designated bid to Accepted
            bidModelAccepted.State = BidState.Accepted;

            // We use the owner of the property
            await this.PropertyRepository.UpdateBidAsync(bidModelAccepted, "dummy");

            // Simulate owner trying to change the price
            propertyModel.Price = 9999999;

            try
            {
                await this.PropertyRepository.UpdatePropertyAsync(propertyModel, "dummy");
            }
            catch (RepositoryException re)
            {
                Assert.AreEqual(RepositoryErrorCode.Validation, re.ErrorCode);
            }
        }
    }
}
