using LibraryData;
using LibraryData.Models;
using LibraryServices.BaseService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryBranchService : BaseService<LibraryBranch>, ILibraryBranchService
    {
        #region Constructor
        public LibraryBranchService(LibraryContext context) : base(context) { }

        #endregion

        #region ILibraryBranchService Implementation
        public override IEnumerable<LibraryBranch> GetAll()
        {
            return libraryContext.LibraryBranches
                        .Include(lb => lb.LibraryAssets)
                        .Include(lb => lb.Patrons);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return GetById(branchId).LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var branchHours = libraryContext.BranchHours
                                .Include(bh => bh.LibraryBranch)
                                .Where(bh => bh.LibraryBranch.Id == branchId);
            return DataHelpers.Instance.ToReadableBusinessHours(branchHours);
        }
        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return GetById(branchId).Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            int currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
            int currentHour = DateTime.Now.Hour;

            return libraryContext.BranchHours
                                .Include(bh => bh.LibraryBranch)
                                .Any(bh => bh.LibraryBranch.Id == branchId &&
                                            bh.DayOfWeek == currentDayOfWeek &&
                                            currentHour >= bh.OpenTime && bh.CloseTime > currentHour);
        }

        #endregion
    }
}
