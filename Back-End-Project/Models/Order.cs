using Back_End_Project.Enums;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Back_End_Project.Models
{
    public class Order:BaseEntity
    {
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
        public int No { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        [StringLength(100)]
        public string? Phone { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? PostalCode { get; set;}
        [StringLength(100)]
        public string? State { get; set; }
        public OrderType Status { get; set; }
        public string? Commet { get; set; }
    }
}
