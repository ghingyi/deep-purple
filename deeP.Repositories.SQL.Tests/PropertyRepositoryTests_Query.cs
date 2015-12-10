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
        [TestCategory("Query")]
        [Description("Test to see if the repository can query properties without filter conditions.")]
        public async Task QueryProperties_NoFilter()
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

            PropertyModel propertyModel2 = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
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

            // Create entities
            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            await this.PropertyRepository.CreatePropertyAsync(propertyModel2, "dummy2");

            // Query entities without filter, ordered by price
            IEnumerable<PropertyModel> propertyModels = await this.PropertyRepository.QueryPropertiesAsync(new PropertyFilter(), PropertySorting.PriceAsc);

            Assert.IsNotNull(propertyModels, "We expected to retrieve a collection of properties.");
            Assert.AreEqual(2, propertyModels.Count(), "We expected to get exactly two properties in the result set.");

            PropertyModel retrievedPropertyModel = propertyModels.First();
            Assert.AreEqual(propertyModel.Description, retrievedPropertyModel.Description, "We expected property descriptions to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.Type, retrievedPropertyModel.Type, "We expected property types to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.Bedrooms, retrievedPropertyModel.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.Price, retrievedPropertyModel.Price, "We expected property prices to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.Address, retrievedPropertyModel.Address, "We expected property addresses to remain unchanged once stored.");
            Assert.AreEqual(JsonConvert.SerializeObject(propertyModel.LocationDetails), JsonConvert.SerializeObject(retrievedPropertyModel.LocationDetails), "We expected property location details to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.State, retrievedPropertyModel.State, "We expected property location state to remain unchanged once stored.");
            Assert.AreEqual(propertyModel.ImageInfos.Length, retrievedPropertyModel.ImageInfos.Length, "We expected to find two images associated with the property.");

            Assert.AreEqual(propertyModel.ImageInfos[0].Id, retrievedPropertyModel.ImageInfos[0].Id, "We expected the image info ids to remain the same after storage.");
            Assert.AreEqual(propertyModel.ImageInfos[0].Title, retrievedPropertyModel.ImageInfos[0].Title, "We expected the image info titles to remain the same after storage.");
            Assert.AreEqual(propertyModel.ImageInfos[0].Uri, retrievedPropertyModel.ImageInfos[0].Uri, "We expected the image info uris to remain the same after storage.");
            Assert.AreEqual(propertyModel.ImageInfos[1].Id, retrievedPropertyModel.ImageInfos[1].Id, "We expected the image info ids to remain the same after storage.");
            Assert.AreEqual(propertyModel.ImageInfos[1].Title, retrievedPropertyModel.ImageInfos[1].Title, "We expected the image info titles to remain the same after storage.");
            Assert.AreEqual(propertyModel.ImageInfos[1].Uri, retrievedPropertyModel.ImageInfos[1].Uri, "We expected the image info uris to remain the same after storage.");

            retrievedPropertyModel = propertyModels.Last();

            Assert.AreEqual(propertyModel2.Description, retrievedPropertyModel.Description, "We expected property descriptions to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.Type, retrievedPropertyModel.Type, "We expected property types to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.Bedrooms, retrievedPropertyModel.Bedrooms, "We expected property bedrooms to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.Price, retrievedPropertyModel.Price, "We expected property prices to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.Address, retrievedPropertyModel.Address, "We expected property addresses to remain unchanged once stored.");
            Assert.AreEqual(JsonConvert.SerializeObject(propertyModel2.LocationDetails), JsonConvert.SerializeObject(retrievedPropertyModel.LocationDetails), "We expected property location details to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.State, retrievedPropertyModel.State, "We expected property location state to remain unchanged once stored.");
            Assert.AreEqual(propertyModel2.ImageInfos.Length, retrievedPropertyModel.ImageInfos.Length, "We expected to find one image associated with the property.");

            Assert.AreEqual(propertyModel2.ImageInfos[0].Id, retrievedPropertyModel.ImageInfos[0].Id, "We expected the image info ids to remain the same after storage.");
            Assert.AreEqual(propertyModel2.ImageInfos[0].Title, retrievedPropertyModel.ImageInfos[0].Title, "We expected the image info titles to remain the same after storage.");
            Assert.AreEqual(propertyModel2.ImageInfos[0].Uri, retrievedPropertyModel.ImageInfos[0].Uri, "We expected the image info uris to remain the same after storage.");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Query")]
        [Description("Test to see if the repository can query properties with filter conditions.")]
        public async Task QueryProperties_SomeFilter()
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

            PropertyModel propertyModel2 = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
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

            // Create entities
            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            await this.PropertyRepository.CreatePropertyAsync(propertyModel2, "dummy2");

            // Query entities with filter for price and room number, ordered by price
            PropertyFilter filter = new PropertyFilter()
            {
                MinPrice = 1234.4,
                MinBedrooms = 4
            };
            IEnumerable<PropertyModel> propertyModels = await this.PropertyRepository.QueryPropertiesAsync(filter, PropertySorting.PriceAsc);

            Assert.IsNotNull(propertyModels, "We expected to retrieve a collection of properties.");
            Assert.AreEqual(1, propertyModels.Count(), "We expected to get exactly one property in the result set.");

            PropertyModel retrievedPropertyModel = propertyModels.First();
            Assert.AreEqual(propertyModel2.Id, retrievedPropertyModel.Id, "We expected the second property to be retrieved.");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Query")]
        [Description("Test to see if the repository can query obeying sorting requirements.")]
        public async Task QueryProperties_Sorting()
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

            PropertyModel propertyModel2 = new PropertyModel()
            {
                Id = Guid.NewGuid().ToString(),
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

            // Create entities
            await this.PropertyRepository.CreatePropertyAsync(propertyModel, "dummy");

            await this.PropertyRepository.CreatePropertyAsync(propertyModel2, "dummy2");

            // Query entities without filter, ordered by price and then descending by price
            PropertyFilter filter = new PropertyFilter();
            IEnumerable<PropertyModel> propertyModelAscByPrice = await this.PropertyRepository.QueryPropertiesAsync(filter, PropertySorting.PriceAsc);
            IEnumerable<PropertyModel> propertyModelDescByPrice = await this.PropertyRepository.QueryPropertiesAsync(filter, PropertySorting.PriceDesc);

            Assert.IsNotNull(propertyModelAscByPrice, "We expected to retrieve a collection of properties.");
            Assert.AreEqual(2, propertyModelAscByPrice.Count(), "We expected to get exactly one property in the result set.");

            PropertyModel retrievedPropertyModel = propertyModelAscByPrice.First();
            Assert.AreEqual(propertyModel.Id, retrievedPropertyModel.Id, "We expected the first property to be retrieved.");

            Assert.IsNotNull(propertyModelDescByPrice, "We expected to retrieve a collection of properties.");
            Assert.AreEqual(2, propertyModelDescByPrice.Count(), "We expected to get exactly one property in the result set.");

            retrievedPropertyModel = propertyModelDescByPrice.First();
            Assert.AreEqual(propertyModel2.Id, retrievedPropertyModel.Id, "We expected the second property to be retrieved.");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Query")]
        [Description("Test to see if the repository can query bids without filter conditions.")]
        public async Task QueryBids_NoFilter()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel1 = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Title = "Best flat in Brum",
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel1, "dummy");

            BidModel bidModel2 = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Title = "Best flat in Brum",
                Price = 1233,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel2, "dummy2");

            // Query entities without filter
            IEnumerable<BidModel> bidModels = await this.PropertyRepository.QueryBidsAsync(new BidFilter());

            Assert.IsNotNull(bidModels, "We expected to retrieve a collection of bids.");
            Assert.AreEqual(2, bidModels.Count(), "We expected to get exactly two bids in the result set.");

            // Note: the bid query doesn't define ordering so we order desc by price for comparisons
            bidModels = bidModels.OrderByDescending(b => b.Price).ToArray();

            BidModel retrievedBidModel = bidModels.First();
            Assert.AreEqual(bidModel1.PropertyId, retrievedBidModel.PropertyId, "We expected property property ids to remain unchanged once stored.");
            Assert.AreEqual(bidModel1.Price, retrievedBidModel.Price, "We expected prices types to remain unchanged once stored.");
            Assert.AreEqual(bidModel1.State, retrievedBidModel.State, "We expected states to remain unchanged once stored.");

            retrievedBidModel = bidModels.Last();
            Assert.AreEqual(bidModel2.PropertyId, retrievedBidModel.PropertyId, "We expected property property ids to remain unchanged once stored.");
            Assert.AreEqual(bidModel2.Price, retrievedBidModel.Price, "We expected prices types to remain unchanged once stored.");
            Assert.AreEqual(bidModel2.State, retrievedBidModel.State, "We expected states to remain unchanged once stored.");
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Query")]
        [Description("Test to see if the repository can query vids with filter conditions.")]
        public async Task QueryBids_SomeFilter()
        {
            // Create the setup by the following test
            await CreateProperty_NoImages();

            string propertyId;
            using (var context = CreateContext())
            {
                // Read the only property in the database
                propertyId = context.Properties.Single().Id;
            }

            BidModel bidModel1 = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Title = "Best flat in Brum",
                Price = 1234,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel1, "dummy");

            BidModel bidModel2 = new BidModel()
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = propertyId,
                Title = "Best flat in Brum",
                Price = 1233,
                State = BidState.Open
            };

            await this.PropertyRepository.CreateBidAsync(bidModel2, "dummy2");

            // Query entities without filter
            IEnumerable<BidModel> bidModels = await this.PropertyRepository.QueryBidsAsync(new BidFilter() { BuyerName = "dummy2" });

            Assert.IsNotNull(bidModels, "We expected to retrieve a collection of bids.");
            Assert.AreEqual(1, bidModels.Count(), "We expected to get exactly one bid in the result set.");

            BidModel retrievedBidModel = bidModels.First();
            Assert.AreEqual(bidModel2.PropertyId, retrievedBidModel.PropertyId, "We expected property property ids to remain unchanged once stored.");
            Assert.AreEqual(bidModel2.Price, retrievedBidModel.Price, "We expected prices types to remain unchanged once stored.");
            Assert.AreEqual(bidModel2.State, retrievedBidModel.State, "We expected states to remain unchanged once stored.");
        }
    }
}
