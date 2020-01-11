using LibraryData;
using LibraryData.Models;
using LibraryServices.BaseService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryPatronService : BaseService<Patron>, ILibraryPatronService
    {
        #region Constructor

        public LibraryPatronService(LibraryContext context) : base(context)
        {
        }

        #endregion

        #region ILibraryPatronService Implementation
        public override IEnumerable<Patron> GetAll()
        {
            return libraryContext.Patrons
                        .Include(p => p.HomeLibraryBranch)
                        .Include(p => p.LibraryCard);
        }
        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            return libraryContext.CheckoutHistories
                        .Include(ch => ch.LibraryAsset)
                        .Include(ch => ch.LibraryCard)
                        .Where(ch => ch.LibraryCard.Id == GetById(patronId).LibraryCard.Id)
                        .OrderByDescending(ch => ch.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            return libraryContext.Checkouts
                        .Include(co => co.LibraryAsset)
                        .Include(co => co.LibraryCard)
                        .Where(p => p.LibraryCard.Id == GetById(patronId).LibraryCard.Id);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            return libraryContext.Holds
                        .Include(h => h.LibraryAsset)
                        .Include(h => h.LibraryCard)
                        .Where(h => h.LibraryCard.Id == GetById(patronId).LibraryCard.Id)
                        .OrderByDescending(h => h.HoldPlaced);
        }

        #endregion
    }
}
