using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using deeP.Repositories.SQL.Context;
using System.Text;

namespace deeP.Repositories.SQL.Tests
{
    [TestClass]
    public class ImageRepositoryTests : DbTestClass
    {
        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Image")]
        [Description("Test to see if the repository will detect null parameter properly.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ImageStore_NullStream()
        {
            string result = await this.ImageRepository.StoreImageAsync(null);
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Image")]
        [Description("Test to see if the repository can store zero byte buffers.")]
        public async Task ImageStore_EmptyStream()
        {
            string id = await this.ImageRepository.StoreImageAsync(new MemoryStream(new byte[0]));
            Assert.IsNotNull(id, "The Id of the image stored was not expected to be null.");

            int imageId;
            Assert.IsTrue(int.TryParse(id, out imageId), "We expected an integer Id for images stored in SQL.");

            using (var context = CreateContext())
            {
                Image image = await context.Images.FindAsync(imageId);

                Assert.IsNotNull(image, "We expected an image to be stored.");
                Assert.IsNotNull(image.ImageData, "We expected image data to be present.");
                Assert.AreEqual(0, image.ImageData.Length, "We expected image data to be empty.");

                Assert.AreEqual(1, context.Images.Count(), "We expected to find only one image.");

                Assert.AreEqual(0, context.Properties.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Image")]
        [Description("Test to see if content is properly stored and can be retrieved.")]
        public async Task ImageStore_MemStream()
        {
            string content = "Look Mom, no hands!";
            byte[] buffer = Encoding.UTF8.GetBytes(content);

            string id = await this.ImageRepository.StoreImageAsync(new MemoryStream(buffer));
            Assert.IsNotNull(id, "The Id of the image stored was not expected to be null.");

            int imageId;
            Assert.IsTrue(int.TryParse(id, out imageId), "We expected an integer Id for images stored in SQL.");

            using (var context = CreateContext())
            {
                Image image = await context.Images.FindAsync(imageId);

                Assert.IsNotNull(image, "We expected an image to be stored.");
                Assert.IsNotNull(image.ImageData, "We expected image data to be present.");
                Assert.AreEqual(buffer.Length, image.ImageData.Length, "We expected image data to be exactly 10 bytes.");
                Assert.AreEqual(content, Encoding.UTF8.GetString(image.ImageData), "We expected image data to contain the same content.");

                Assert.AreEqual(1, context.Images.Count(), "We expected to find only one image.");

                Assert.AreEqual(0, context.Properties.Count(), "We did not expect any properties to be found.");
                Assert.AreEqual(0, context.Bids.Count(), "We did not expect any bids to be found.");
                Assert.AreEqual(0, context.ImageInfos.Count(), "We did not expect any image infos to be found.");
            }
        }

        [TestMethod]
        [Owner("ghingyi")]
        [TestCategory("Image")]
        [Description("Test to see if content is properly stored and can be retrieved using the repository both ways.")]
        public async Task ImageGet_MemStream()
        {
            string content = "The force is with you, Luke.";
            byte[] buffer = Encoding.UTF8.GetBytes(content);

            string id = await this.ImageRepository.StoreImageAsync(new MemoryStream(buffer));
            Assert.IsNotNull(id, "The Id of the image stored was not expected to be null.");

            int imageId;
            Assert.IsTrue(int.TryParse(id, out imageId), "We expected an integer Id for images stored in SQL.");

            Stream contentStream = await this.ImageRepository.GetImageStreamAsync(id);

            Assert.IsNotNull(contentStream, "We expected to get a stream back.");

            byte[] bufferRead = new byte[buffer.Length];
            await contentStream.ReadAsync(bufferRead, 0, buffer.Length);

            Assert.IsTrue(buffer.SequenceEqual(bufferRead), "We expected the buffer read back to contain the same content as was stored originally.");
        }
    }
}
