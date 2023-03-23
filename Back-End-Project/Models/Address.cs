using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class Address:BaseEntity
    {
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        [StringLength(100)]
        public string? State { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? PostalCode { get; set; }
        public bool IsMain { get; set; }
        public string? Phone { get; set; }
    }
}
