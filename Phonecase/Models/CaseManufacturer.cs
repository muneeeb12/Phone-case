using System.ComponentModel.DataAnnotations;

namespace Phonecase.Models
{
    public class CaseManufacturer
    {
        [Key]
        public int CaseManufacturerId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
