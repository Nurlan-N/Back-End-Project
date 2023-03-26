using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class Blog : BaseEntity
    {
        [StringLength(220)]
        public string? Image { get; set; }
        [StringLength(1000)]
        public string? Url { get; set; }
        [StringLength(250)]
        public string  Title { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }
    }
}
