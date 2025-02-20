using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Phonecase.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCredit { get; set; } = 0.00m;

        // Navigation Properties
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
