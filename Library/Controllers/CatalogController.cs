using Library.Models.Catalog;
using Library.Models.Catalog.CheckOut;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        #region Properties

        private readonly ILibraryAssetService libraryAsset;
        private readonly ILibraryCheckoutService checkoutService;

        #endregion Properties

        #region Constructor

        public CatalogController(ILibraryAssetService libAsset, ILibraryCheckoutService checkout)
        {
            libraryAsset = libAsset;
            checkoutService = checkout;
        }

        #endregion Constructor

        #region Methods

        public IActionResult Index()
        {
            var assetModels = libraryAsset.GetAll();

            var listingResult =
                    assetModels.Select(result => new AssetIndexListingModel
                    {
                        AuthorOrDirector = libraryAsset.GetAuthorOrDirector(result.Id),
                        DeweyCallNumber = libraryAsset.GetDeweyIndex(result.Id),
                        Id = result.Id,
                        ImageUrl = result.ImageUrl,
                        NumberOfCopies = result.NumberOfCopies,
                        Title = result.Title,
                        Type = libraryAsset.GetType(result.Id)
                    });

            var model = new AssetIndexModel { Assets = listingResult };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = libraryAsset.GetById(id);

            var currentHolds = checkoutService.GetCurrentHolds(id)
                                .Select(a => new AssetHoldModel
                                {
                                    //HoldPlaced = checkoutService.GetCurrentHoldPlaced(a.Id).ToString("d"),
                                    PatronName = checkoutService.GetCurrentHoldPatronName(a.Id)
                                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                AuthorOrDirector = libraryAsset.GetAuthorOrDirector(id),
                Cost = asset.Cost,
                CurrentLocation = asset.Location.Name,
                DeweyCallNumber = libraryAsset.GetDeweyIndex(id),
                ImageUrl = asset.ImageUrl,
                ISBN = libraryAsset.GetIsbn(id),
                Status = asset.Status.Name,
                Title = asset.Title,
                Type = libraryAsset.GetType(id),
                Year = asset.Year,
                CheckoutHistory = checkoutService.GetCheckoutHistory(id),
                LatestCheckout = checkoutService.GetLatestCheckout(id),
                PatronName = checkoutService.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds  
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = libraryAsset.GetById(id);

            var checkoutModel = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                IsCheckedOut = checkoutService.IsCheckedout(id),
                LibraryCardId = "",
            };

            return View(checkoutModel);
        }

        public IActionResult CheckIn(int id)
        {
            checkoutService.CheckInItem(id);
            return RedirectToAction("Detail", new { id });
        }
        public IActionResult Hold(int id)
        {
            var asset = libraryAsset.GetById(id);

            var checkoutModel = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                IsCheckedOut = checkoutService.IsCheckedout(id),
                LibraryCardId = "",
                HoldCount = checkoutService.GetCurrentHolds(id).Count()
            };

            return View(checkoutModel);
        }

        public IActionResult MarkLost(int id)
        {
            checkoutService.MarkLost(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkFound(int id)
        {
            checkoutService.MarkFound(id);
            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            checkoutService.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            checkoutService.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        #endregion Methods
    }
}