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
        public Task<IEnumerable<PropertyModel>> QueryPropertiesAsync(PropertyFilter filter, PropertySorting sorting)
        {
            try
            {
                using (var context = CreateContext())
                {
                    // Define base query - select on properties with equ-join on image infos
                    IEnumerable<Property> query = context.Properties.Include("ImageInfos");

                    // Apply filters
                    if (!string.IsNullOrEmpty(filter.PropertyId))
                    {
                        query = query.Where(p => p.Id == filter.PropertyId);
                    }

                    if (!string.IsNullOrEmpty(filter.SellerName))
                    {
                        query = query.Where(p => p.Owner == filter.SellerName);
                    }

                    if (filter.Type.HasValue)
                    {
                        query = query.Where(p => p.Type == filter.Type.Value);
                    }

                    query = query.Where(p => p.Bedrooms >= filter.MinBedrooms);

                    if (filter.MinPrice.HasValue)
                    {
                        query = query.Where(p => p.Price >= filter.MinPrice.Value);
                    }

                    if (filter.MaxPrice.HasValue)
                    {
                        query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Address))
                    {
                        // TODO: some more sophisticated full text search (not supported by Azure SQL), like Lucene or something context aware
                        string[] words = filter.Address.ToLower().Split(' ', '\t', ',', ';', '.', '?', '!', '\"', '\'', '-', '+', '/', '\\', '*');

                        // It's going to be based on set element matching for containment in the lowercase version of the address string for every rows...
                        query = query.Where(p => words.Any(w => p.Address.ToLower().Contains(w)));
                    }

                    if (!filter.IncludeTaken)
                    {
                        query = query.Where(p => p.State != PropertyState.Taken);
                    }

                    // Apply sorting
                    switch (sorting)
                    {
                        case PropertySorting.PriceAsc:
                            query = query.OrderBy(p => p.Price);
                            break;
                        case PropertySorting.PriceDesc:
                            query = query.OrderByDescending(p => p.Price);
                            break;
                        default:
                            throw new RepositoryException(RepositoryErrorCode.Validation, string.Format("Sorting method '{0}' is not supported.", sorting));
                    }

                    // Apply skip
                    if (filter.Skip.HasValue)
                    {
                        query = query.Skip((int)filter.Skip.Value);
                    }

                    // Apply take
                    if (filter.Take.HasValue)
                    {
                        query = query.Take((int)filter.Take.Value);
                    }

                    // Execute and return mapped results
                    IEnumerable<PropertyModel> result = query.Select(p =>
                    {
                        PropertyModel propertyModel = new PropertyModel();
                        CopyPropertyDetails(p, propertyModel);
                        propertyModel.ImageInfos = p.ImageInfos.OrderBy(ii => ii.Order).Select(ii =>
                        {
                            ImageInfoModel imageInfoModel = new ImageInfoModel();
                            CopyImageProperties(ii, imageInfoModel);
                            return imageInfoModel;
                        }).ToArray();
                        return propertyModel;
                    }).ToArray();

                    return Task.FromResult(result);
                }
            }
            catch (Exception ue)
            {
                throw new RepositoryException(RepositoryErrorCode.General, "An error has occured while trying to query properties. Please try again.", ue);
            }
        }

        public Task<IEnumerable<BidModel>> QueryBidsAsync(BidFilter filter)
        {
            try
            {
                using (var context = CreateContext())
                {
                    IEnumerable<Bid> query;

                    // Apply filters
                    if (!string.IsNullOrEmpty(filter.SellerName))
                    {
                        // This will result in an equi-join for the condition to evaluate
                        query = context.Bids.Include("Property");
                        query = query.Where(b => b.Property.Owner == filter.SellerName);
                    }
                    else
                    {
                        query = context.Bids;
                    }

                    if (!string.IsNullOrEmpty(filter.BuyerName))
                    {
                        query = query.Where(b => b.Owner == filter.BuyerName);
                    }

                    if (!string.IsNullOrEmpty(filter.PropertyId))
                    {
                        query = query.Where(b => b.PropertyId == filter.PropertyId);
                    }

                    if (!filter.IncludeRejected)
                    {
                        query = query.Where(b => b.State != BidState.Rejected);
                    }

                    // Apply skip
                    if (filter.Skip.HasValue)
                    {
                        query = query.Skip((int)filter.Skip.Value);
                    }

                    // Apply take
                    if (filter.Take.HasValue)
                    {
                        query = query.Take((int)filter.Take.Value);
                    }

                    // Execute and return mapped results
                    IEnumerable<BidModel> result = query.Select(b =>
                    {
                        BidModel bidModel = new BidModel();
                        CopyBidDetails(b, bidModel);
                        return bidModel;
                    }).ToArray();

                    return Task.FromResult(result);
                }
            }
            catch (Exception ue)
            {
                throw new RepositoryException(RepositoryErrorCode.General, "An error has occured while trying to query bids. Please try again.", ue);
            }
        }

        #region Private methods

        private static void CopyImageProperties(ImageInfo imageInfo, ImageInfoModel imageInfoModel)
        {
            imageInfoModel.Id = imageInfo.Id;
            imageInfoModel.Uri = imageInfo.Uri;
            imageInfoModel.Title = imageInfo.Title;
        }

        private static void CopyPropertyDetails(Property property, PropertyModel propertyModel)
        {
            propertyModel.Id = property.Id;
            propertyModel.Owner = property.Owner;
            propertyModel.Description = property.Description;
            propertyModel.Type = property.Type;
            propertyModel.Bedrooms = property.Bedrooms;
            propertyModel.Price = property.Price;
            propertyModel.Address = property.Address;
            propertyModel.LocationDetails = !string.IsNullOrEmpty(property.LocationDetails) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(property.LocationDetails) : null;
            propertyModel.State = property.State;
        }

        private static void CopyBidDetails(Bid bid, BidModel bidModel)
        {
            bidModel.Id = bid.Id;
            bidModel.Owner = bid.Owner;
            bidModel.Title = bid.Title;
            bidModel.Price = bid.Price;
            bidModel.State = bid.State;
            bidModel.PropertyId = bid.PropertyId;
        }

        #endregion
    }
}
