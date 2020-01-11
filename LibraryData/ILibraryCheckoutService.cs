using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ILibraryCheckoutService
    {
        IEnumerable<Checkout> GetAll();
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int assetId);
        IEnumerable<Hold> GetCurrentHolds(int assetId);

        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);

        bool IsCheckedout(int assetId);
        //DateTime GetCurrentHoldPlaced(int holdId);
        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatronName(int holdId);
        

        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        void PlaceHold(int assetId, int libraryCardId);
        void MarkLost(int assetId);
        void MarkFound(int assetId);
    }
}
