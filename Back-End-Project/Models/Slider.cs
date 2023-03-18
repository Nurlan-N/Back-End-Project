using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_End_Project.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string? SubTitle { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(255)]
        public string? Link { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
