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
        public async Task CreateBidAsync(BidModel bidModel, string userName)
        {
            if (bidModel == null)
                throw new ArgumentNullException("bidModel");

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            try
            {
                using (var context = CreateContext())
                {
                    Bid bid = context.Bids.Create();
                    CopyBidDetails(bidModel, userName, bid);

                    context.Bids.Add(bid);

                    // Insert entity into database in an implicit transaction
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

        public async Task UpdateBidAsync(BidModel bidModel, string userName)
        {
            if (bidModel == null)
                throw new ArgumentNullException("bidModel");

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
                            // Retrieve bid as we'll need to compare their states
                            Bid bid = await context.Bids.FindAsync(bidModel.Id);
                            if (bid == null)
                                throw new RepositoryException(RepositoryErrorCode.NotFound, "No bid was found that could be updated.");

                            ValidateBidChangeRequest(bidModel, userName, bid);

                            bool rejectOtherBids = bid.State == BidState.Open && bidModel.State == BidState.Accepted;

                            // Update this entity for the model
                            CopyBidDetails(bidModel, userName, bid);

                            if (rejectOtherBids)
                            {
                                var otherOpenBidsQuery = context.Bids.Where(b => b.PropertyId == bid.PropertyId && b.Id != bid.Id && b.State == BidState.Open);
                                foreach (Bid openBid in otherOpenBidsQuery)
                                {
                                    openBid.State = BidState.Rejected;
                                }
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

        private static void ValidateBidChangeRequest(BidModel bidModel, string userName, Bid bid)
        {
            if (bid.Owner != userName)
            {
                if (bid.Price != bidModel.Price)
                    throw new RepositoryException(RepositoryErrorCode.Unauthorized, "Only the owner of a bid can change the bid price.");

                if (userName != bid.Property.Owner)
                    throw new RepositoryException(RepositoryErrorCode.Unauthorized, "Only the owner of a bid or property can change a bid.");
            }
            if (bid.State != BidState.Open && bidModel.State == BidState.Open)
            {
                throw new RepositoryException(RepositoryErrorCode.Validation, "Closed bids cannot be reopened.");
            }
        }

        private static void CopyBidDetails(BidModel bidModel, string userName, Bid bid)
        {
            bid.Id = bidModel.Id;
            bid.Owner = userName;
            bid.Price = bidModel.Price;
            bid.State = bidModel.State;
            bid.PropertyId = bidModel.PropertyId;
        }

        #endregion
    }
}
