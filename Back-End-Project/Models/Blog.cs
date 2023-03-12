using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class Blog : BaseEntity
    {
        [StringLength(250)]
        public string? Image { get; set; }
        [StringLength(250)]
        public string? Title { get; set; }
        [StringLength(1500)]
        public string? Description { get; set; }
    }
}
