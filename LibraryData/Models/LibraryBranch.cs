using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryData.Models
{
    public class LibraryBranch : IEntityWithId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Limit branch name to 30 characters.")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string TelephoneNumber { get; set; }

        public string Description { get; set; }
        public DateTime OpenDate { get; set; }
        public IEnumerable<Patron> Patrons { get; set; }
        public IEnumerable<LibraryAsset> LibraryAssets { get; set; }
        public string ImageUrl { get; set; }
    }
}