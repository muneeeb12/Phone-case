using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phonecase.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int VendorId { get; set; }  // Foreign Key
        public Vendor Vendor { get; set; }  // Navigation Property

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Debit { get; set; } = 0;  // Amount spent

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Credit { get; set; } = 0; // Amount received

        [Column(TypeName = "decimal(10,2)")]
        public decimal CashBalance { get; set; } // Running balance
    }
}