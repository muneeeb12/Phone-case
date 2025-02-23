using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Phonecase.Models {
    public class Vendor {
        [Key]
        public int VendorId { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCredit { get; set; } = 0.00m;

        // New field for maintaining running balance
        [Column(TypeName = "decimal(10,2)")]
        public decimal RemainingBalance { get; set; } = 0.00m;
    }
}
