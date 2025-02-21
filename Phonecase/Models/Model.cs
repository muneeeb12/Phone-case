using System.ComponentModel.DataAnnotations;

namespace Phonecase.Models
{
    public class Model
    {
        [Key]
        public int ModelId { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
