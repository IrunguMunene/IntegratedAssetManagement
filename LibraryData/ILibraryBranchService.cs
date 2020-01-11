using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ILibraryBranchService
    {
        IEnumerable<LibraryAsset> GetAssets(int branchId);
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<Patron> GetPatrons(int branchId);
        IEnumerable<string> GetBranchHours(int branchId);

        bool IsBranchOpen(int branchId);
        LibraryBranch GetById(int branchId);
        void Add(LibraryBranch newBranch);
    }
}
