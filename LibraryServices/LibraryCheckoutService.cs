using LibraryData;
using LibraryData.Models;
using LibraryServices.BaseService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryCheckoutService : BaseService<Checkout>, ILibraryCheckoutService
    {
        #region Properties

        private readonly ILibraryAssetService libraryAssetService;
        private readonly DateTime dateTimeNow;

        #endregion

        #region Constructor
        /// <summary>
        /// For Dependency Injection to work, the constructor parameters for any Service
        /// should be declared as type of the interface they implement. For example in this
        /// case the libraryAssetService parameter is declared as ILibraryAsset and not as
        /// the concrete class. Declaring as concrete class will produce the error
        /// HTTP Error 500.30 - ANCM In-Process Start Failure
        /// </summary>
        /// <param name="context">DbContext Object</param>
        /// <param name="assetService">ILibraryAsset object</param>
        public LibraryCheckoutService(LibraryContext context, ILibraryAssetService assetService) : base(context)
        {
            libraryAssetService = assetService;
            dateTimeNow = DateTime.UtcNow;
        }

        #endregion

        #region ICheckout Implementation
        public override IEnumerable<Checkout> GetAll()
        {
            return libraryContext.Checkouts;
        }
        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return libraryContext.CheckoutHistories
                        .Include(h => h.LibraryAsset)
                        .Include(h => h.LibraryCard)
                        .Where(h => h.LibraryAsset.Id == id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return libraryContext.Holds
                        .Include(h => h.LibraryAsset)
                        .Include(h => h.LibraryCard)
                        .Where(h => h.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return libraryContext.Checkouts.Where(checkout => checkout.LibraryAsset.Id == assetId)
                        .OrderByDescending(checkout => checkout.Since)
                        .FirstOrDefault();
        }

        public void MarkFound(int assetId)
        {
            UpdateAssetStatus(assetId, "Available");

            //Remove existing checkout on item
            RemoveExistingCheckout(assetId);

            //Close any existing checkout history
            CloseAnyExistingCheckoutHistory(assetId);
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");

            libraryContext.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var asset = libraryAssetService.GetById(assetId);
            var libraryCard = libraryContext.LibraryCards
                                .FirstOrDefault(lc => lc.Id == libraryCardId);

            if (asset == null || libraryCard == null) return;

            if(asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            libraryContext.Add(new Hold
            {
                HoldPlaced = dateTimeNow,
                LibraryAsset = asset,
                LibraryCard = libraryCard
            });

            libraryContext.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            //Get the latest checkout
            var latestCheckOut = libraryContext.Checkouts.Where(co => co.LibraryAsset.Id == assetId)
                                    .OrderByDescending(co => co.Since)
                                    .FirstOrDefault();

            if (latestCheckOut != null)
            {
                libraryContext.Update(latestCheckOut);
                //Update check in date
                latestCheckOut.Until = dateTimeNow;
            }

            //Remove existing checkouts
            RemoveExistingCheckout(assetId);
            //Close existing checkout history
            CloseAnyExistingCheckoutHistory(assetId);


            //Checkout asset to the earliest hold library card.
            CheckoutAssetToEarliestHoldLibraryCard(assetId);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            var libraryCard = libraryContext.LibraryCards
                                .Include(lc => lc.Checkouts)
                                .FirstOrDefault(lc => lc.Id == libraryCardId);

            var asset = libraryAssetService.GetById(assetId);

            if (IsCheckedout(assetId) || libraryCard == null || asset == null) return;

            UpdateAssetStatus(assetId, "Checked Out");

            CreateNewChekout(libraryCard, asset);

            CreateCheckoutHistoryRecord(libraryCard, asset);

            libraryContext.SaveChanges();
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = libraryContext.Holds
                        .Include(h => h.LibraryAsset)
                        .Include(h => h.LibraryCard)
                        .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var patron = libraryContext.Patrons
                            .Include(p => p.LibraryCard)
                            .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return $"{patron?.FirstName} {patron?.LastName}";
        }
        //public DateTime GetCurrentHoldPlaced(int id)
        //{
        //    return libraryContext.Holds.FirstOrDefault(h => h.LibraryCard.Id == id).HoldPlaced;
        //}

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkOut = GetCheckoutByAssetId(assetId);

            if (checkOut == null) return string.Empty;

            var cardId = checkOut.LibraryCard.Id;

            var patron = libraryContext.Patrons
                            .Include(p => p.LibraryCard)
                            .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return $"{patron?.FirstName} {patron?.LastName}";
        }

        public bool IsCheckedout(int assetId)
        {
            return libraryContext.Checkouts.Any(co => co.LibraryAsset.Id == assetId);
        }

        #endregion

        #region Class Methods

        private void RemoveExistingCheckout(int assetId)
        {
            var checkout = libraryContext.Checkouts.FirstOrDefault(c => c.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                libraryContext.Remove(checkout);
                UpdateAssetStatus(assetId, "Available");
            }
        }

        private void CloseAnyExistingCheckoutHistory(int assetId)
        {
            var checkoutHistory = libraryContext.CheckoutHistories
                                    .FirstOrDefault(ch => ch.LibraryAsset.Id == assetId &&
                                                        ch.CheckedIn == null);
            if (checkoutHistory != null)
            {
                libraryContext.Update(checkoutHistory);
                checkoutHistory.CheckedIn = dateTimeNow;

                libraryContext.SaveChanges();
            }
        }

        private void UpdateAssetStatus(int assetId, string status)
        {
            var libraryAsset = libraryAssetService.GetById(assetId);

            if(libraryAsset != null)
            {
                libraryContext.Update(libraryAsset);

                libraryAsset.Status = libraryContext.Statuses.FirstOrDefault(s => s.Name == status);
            }
        }

        private void CheckoutAssetToEarliestHoldLibraryCard(int assetId)
        {
            //Checkout asset to the next person in line if there are holds on the asset
            var hold = libraryContext.Holds.Where(h => h.LibraryAsset.Id == assetId)
                                .Include(h => h.LibraryAsset)
                                .Include(h => h.LibraryCard)
                                .OrderBy(h => h.HoldPlaced)
                                .FirstOrDefault();

            if (hold != null)
            {
                //Remove earliest hold.
                libraryContext.Remove(hold);

                CheckOutItem(assetId, hold.LibraryCard.Id);
            }
            else
            {
                //Update asset status to Available
                UpdateAssetStatus(assetId, "Available");
            }

            libraryContext.SaveChanges();
        }
        private DateTime GetDefaultCheckoutTime(DateTime dateTimeNow)
        {
            return dateTimeNow.AddDays(30);
        }

        private void CreateCheckoutHistoryRecord(LibraryCard libraryCard, LibraryAsset asset)
        {
            //Add new checkout history
            libraryContext.Add(new CheckoutHistory
            {
                CheckedOut = dateTimeNow,
                LibraryAsset = asset,
                LibraryCard = libraryCard
            });
        }

        private void CreateNewChekout(LibraryCard libraryCard, LibraryAsset asset)
        {
            libraryContext.Add(new Checkout
            {
                LibraryAsset = asset,
                LibraryCard = libraryCard,
                Since = dateTimeNow,
                Until = GetDefaultCheckoutTime(dateTimeNow)
            });
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return libraryContext.Checkouts
                                .Include(co => co.LibraryAsset)
                                .Include(co => co.LibraryCard)
                                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);

        }

        #endregion
    }
}
