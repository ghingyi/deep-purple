using deeP.Abstraction;
using deeP.Abstraction.Models;
using deeP.Repositories.SQL.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL
{
    public sealed partial class SqlPropertyRepository : IPropertyRepository
    {
        public async Task CreatePropertyAsync(PropertyModel propertyModel, string userName)
        {
            if (propertyModel == null)
                throw new ArgumentNullException("propertyModel");

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            try
            {
                using (var context = CreateContext())
                {
                    Property property = context.Properties.Create();
                    CopyPropertyDetails(propertyModel, userName, property);

                    context.Properties.Add(property);

                    // Associate image info references if there are any
                    if (propertyModel.ImageInfos != null)
                    {
                        for (int i = 0; i < propertyModel.ImageInfos.Length; ++i)
                        {
                            ImageInfoModel imageInfoModel = propertyModel.ImageInfos[i];

                            ImageInfo imageInfo = context.ImageInfos.Create();
                            CopyImageProperties(imageInfoModel, i, imageInfo);

                            property.ImageInfos.Add(imageInfo);
                        }
                    }

                    // Insert tracked entities into database in an implicit transaction
                    await context.SaveChangesAsync();
                }
            }
            catch (DbEntityValidationException eve)
            {
                throw new RepositoryException(RepositoryErrorCode.Validation, "The model did not satisfy schema requirements.", eve);
            }
            catch (DbUpdateException ue)
            {
                throw new RepositoryException(RepositoryErrorCode.General, "An error has occured while trying to save changes to the property. Please try again.", ue);
            }
        }

        public async Task UpdatePropertyAsync(PropertyModel propertyModel, string userName)
        {
            if (propertyModel == null)
                throw new ArgumentNullException("propertyModel");

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            try
            {
                using (var context = CreateContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            // Retrieve property
                            Property property = await context.Properties.FindAsync(propertyModel.Id);
                            if (property == null)
                                throw new RepositoryException(RepositoryErrorCode.NotFound, "No property was found that could be updated.");

                            if (property.Owner != userName)
                                throw new RepositoryException(RepositoryErrorCode.Unauthorized, "Only the owner of a property can make changes.");

                            CopyPropertyDetails(propertyModel, userName, property);
                            
                            // There might be changes to the list of associated images; we enlist the necessary delete/update/inserts below

                            // First collect the currently associated images instances in a dictionary with their respective order
                            Dictionary<string, Tuple<int, ImageInfoModel>> imageInfoModelTupleDictionary = BuildImageInfoDictionary(propertyModel);

                            // We query the currently associated images from the database end evaluate them for delete/update need
                            // based on their presence in the current image dictionary
                            var associatedImageInfoQuery = context.ImageInfos.Where(ii => ii.PropertyId == property.Id);
                            foreach (ImageInfo imageInfo in associatedImageInfoQuery)
                            {
                                Tuple<int, ImageInfoModel> imageInfoModelTuple;
                                if (!imageInfoModelTupleDictionary.TryGetValue(imageInfo.Id, out imageInfoModelTuple))
                                {
                                    // This images is no longer associated with the property; let's remove it from the database
                                    context.ImageInfos.Remove(imageInfo);

                                    // Note: this would be a good time to schedule for the removal of the actual image blob(s)
                                    // We ignore that in this sample
                                }
                                else
                                {
                                    // The image is still associated with the property; let's try to update it
                                    // EF chage-tracking will decide whether update is necessary
                                    ImageInfoModel imageInfoModel = imageInfoModelTuple.Item2;
                                    int order = imageInfoModelTuple.Item1;

                                    CopyImageProperties(imageInfoModel, order, imageInfo);

                                    // Now let's modify the image info dictionary so that we only keep elements that haven't been added for update
                                    imageInfoModelTupleDictionary.Remove(imageInfoModel.Id);
                                }
                            }

                            // Go through the remaining collection of images and insert them
                            foreach (var imageInfoModelTuple in imageInfoModelTupleDictionary.Values)
                            {
                                ImageInfoModel imageInfoModel = imageInfoModelTuple.Item2;
                                int order = imageInfoModelTuple.Item1;

                                // Create image info entity
                                ImageInfo imageInfo = context.ImageInfos.Create();
                                CopyImageProperties(imageInfoModel, order, imageInfo);

                                // Associate it with context and property
                                property.ImageInfos.Add(imageInfo);
                            }

                            // Flush changes into database and commit transaction
                            await context.SaveChangesAsync();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();

                            // Rethrow
                            throw;
                        }
                    }
                }
            }
            catch (DbEntityValidationException eve)
            {
                throw new RepositoryException(RepositoryErrorCode.Validation, "The model did not satisfy schema requirements.", eve);
            }
            catch (DbUpdateConcurrencyException uce)
            {
                throw new RepositoryException(RepositoryErrorCode.Conflict, "A conflict has occurred while trying to save changes to the property. Please make sure you are authorized to edit the property.", uce);
            }
            catch (DbUpdateException ue)
            {
                throw new RepositoryException(RepositoryErrorCode.General, "An error has occured while trying to save changes to the property. Please try again.", ue);
            }
        }

        #region Private methods

        private static Dictionary<string, Tuple<int, ImageInfoModel>> BuildImageInfoDictionary(PropertyModel propertyModel)
        {
            Dictionary<string, Tuple<int, ImageInfoModel>> imageInfoModelTupleDictionary = new Dictionary<string, Tuple<int, ImageInfoModel>>();
            if (propertyModel.ImageInfos != null)
            {
                for (int i = 0; i < propertyModel.ImageInfos.Length; ++i)
                {
                    ImageInfoModel imageInfoModel = propertyModel.ImageInfos[i];

                    imageInfoModelTupleDictionary.Add(imageInfoModel.Id, new Tuple<int, ImageInfoModel>(i, imageInfoModel));
                }
            }
            return imageInfoModelTupleDictionary;
        }

        private static void CopyImageProperties(ImageInfoModel imageInfoModel, int order, ImageInfo imageInfo)
        {
            imageInfo.Id = imageInfoModel.Id;
            imageInfo.Uri = imageInfoModel.Uri;
            imageInfo.Title = imageInfoModel.Title;
            imageInfo.Order = order;
        }

        private static void CopyPropertyDetails(PropertyModel propertyModel, string userName, Property property)
        {
            property.Id = propertyModel.Id;
            property.Owner = userName;
            property.Description = propertyModel.Description;
            property.Type = propertyModel.Type;
            property.Bedrooms = propertyModel.Bedrooms;
            property.Price = propertyModel.Price;
            property.Address = propertyModel.Address;
            property.LocationDetails = JsonConvert.SerializeObject(propertyModel.LocationDetails, Formatting.None);
            property.State = propertyModel.State;
        }

        #endregion
    }
}
