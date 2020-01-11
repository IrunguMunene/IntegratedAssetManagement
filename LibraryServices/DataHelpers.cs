using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class DataHelpers
    {
        #region Singleton Properties

        private static readonly Lazy<DataHelpers> dataHelpers = new Lazy<DataHelpers>(() => new DataHelpers(), true);
        public static DataHelpers Instance => dataHelpers.Value;

        #endregion

        #region Private Constructor
        private DataHelpers() { }

        #endregion

        #region Methods
        public IEnumerable<string> ToReadableBusinessHours(IEnumerable<BranchHours> branchHours)
        {
            // Data has Sunday as index 1 while for C# DayOfWeek Sunday is index 0, hence subtract 1.
            return branchHours.Select(bh => @$"{Enum.GetName(typeof(DayOfWeek), (bh.DayOfWeek - 1))} 
                                                    Opening Time: {new TimeSpan(bh.OpenTime, 0, 0).ToString()}
                                                    Closing Time: {new TimeSpan(bh.CloseTime, 0, 0).ToString()}");
        }

        #endregion
    }
}