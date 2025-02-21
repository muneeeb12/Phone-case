using System.ComponentModel.DataAnnotations;

namespace Phonecase.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int? ModelId { get; set; }
        public Model Model { get; set; }  // Navigation Property

        public int? CaseManufacturerId { get; set; }
        public CaseManufacturer CaseManufacturer { get; set; }  // Navigation Property

        public string CaseName { get; set; }

        // Navigation Property
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
