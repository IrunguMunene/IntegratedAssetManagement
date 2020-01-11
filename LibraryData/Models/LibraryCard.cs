using System;
using System.Collections.Generic;

namespace LibraryData.Models
{
    public class LibraryCard : IEntityWithId
    {
        public int Id { get; set; }
        public decimal Fees { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<Checkout> Checkouts { get; set; }
    }
}