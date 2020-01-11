using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryData.Models
{
    public class BranchHours : IEntityWithId
    {
        public int Id { get; set; }
        public LibraryBranch LibraryBranch { get; set; }

        [Range(0, 6)]
        public int DayOfWeek { get; set; }

        [Range(0, 23)]
        public int OpenTime { get; set; }

        [Range(0, 23)]
        public int CloseTime { get; set; }
    }
}