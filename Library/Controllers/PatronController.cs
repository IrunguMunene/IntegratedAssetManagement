using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        #region Properties

        private ILibraryPatronService libraryPatronService;

        #endregion

        #region Constructor

        public PatronController(ILibraryPatronService patronService)
        {
            libraryPatronService = patronService;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var allPatrons = libraryPatronService.GetAll();

            var patronDetailModels = allPatrons.Select(p => new PatronDetailModel
            {
                Address = p.Address,
                AssetsCheckedOut = libraryPatronService.GetCheckouts(p.Id),
                CheckoutHistory = libraryPatronService.GetCheckoutHistory(p.Id),
                FirstName = p.FirstName,
                Holds = libraryPatronService.GetHolds(p.Id),
                Id = p.Id,
                HomeLibraryBranch = p.HomeLibraryBranch.Name,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                MemberSince = p.LibraryCard.Created,
                OverdueFees = p.LibraryCard.Fees,
                Telephone = p.TelephoneNumber,
            }).ToList();

            var model = new PatronIndexModel
            {
                Patrons = patronDetailModels
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var patron = libraryPatronService.GetById(id);

            var model = new PatronDetailModel
            {
                Address = patron.Address,
                AssetsCheckedOut = libraryPatronService.GetCheckouts(id).ToList() ?? new List<Checkout>(),
                CheckoutHistory = libraryPatronService.GetCheckoutHistory(id).ToList() ?? new List<CheckoutHistory>(),
                FirstName = patron.FirstName,
                Holds = libraryPatronService.GetHolds(id).ToList() ?? new List<Hold>(),
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                Id = patron.Id,
                LastName = patron.LastName,
                LibraryCardId = patron.LibraryCard.Id,
                MemberSince = patron.LibraryCard.Created,
                OverdueFees = patron.LibraryCard.Fees,
                Telephone = patron.TelephoneNumber
            };

            return View(model);
        }

        #endregion
    }
}
