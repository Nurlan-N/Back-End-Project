using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class Comment : BaseEntity
    {
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
