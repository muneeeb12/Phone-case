using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phonecase.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }  // Navigation Property

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
