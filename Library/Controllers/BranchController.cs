using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        #region Properties

        private readonly ILibraryBranchService libraryBranchService;

        #endregion

        #region Constructor
        public BranchController(ILibraryBranchService branchService)
        {
            libraryBranchService = branchService;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var branches = libraryBranchService.GetAll();

            var branchDetailModels = branches.Select(b => new BranchDetailModel
            {
                Address = b.Address,
                BusinessHours = libraryBranchService.GetBranchHours(b.Id),
                Description = b.Description,
                Id = b.Id,
                ImageUrl = b.ImageUrl,
                IsOpen = libraryBranchService.IsBranchOpen(b.Id),
                Name = b.Name,
                NumberOfAssets = libraryBranchService.GetAssets(b.Id).Count(),
                NumberOfPatrons = libraryBranchService.GetPatrons(b.Id).Count(),
                OpenDate = b.OpenDate.ToString("yyyy-MM-dd"),
                Telephone = b.TelephoneNumber,
                TotalAssetValue = libraryBranchService.GetAssets(b.Id).Sum(a => a.Cost)
            });

            var model = new BranchIndexModel
            {
                Branches = branchDetailModels
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = libraryBranchService.GetById(id);

            var model = new BranchDetailModel
            {
                Address = branch.Address,
                BusinessHours = libraryBranchService.GetBranchHours(branch.Id),
                Description = branch.Description,
                Id = branch.Id,
                ImageUrl = branch.ImageUrl,
                IsOpen = libraryBranchService.IsBranchOpen(branch.Id),
                Name = branch.Name,
                NumberOfAssets = libraryBranchService.GetAssets(branch.Id).Count(),
                NumberOfPatrons = libraryBranchService.GetPatrons(branch.Id).Count(),
                OpenDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                Telephone = branch.TelephoneNumber,
                TotalAssetValue = libraryBranchService.GetAssets(branch.Id).Sum(a => a.Cost)
            };

            return View(model);
        }

        #endregion
    }
}
