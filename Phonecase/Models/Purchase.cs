using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Phonecase.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }  // Navigation Property

        public int ProductId { get; set; }
        public Product Product { get; set; }  // Navigation Property

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalPrice => Quantity * UnitPrice;  // Computed column

        [Required]
        public DateTime PurchaseDate { get; set; }
    }
}
