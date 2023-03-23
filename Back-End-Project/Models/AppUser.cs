using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(20)]
        public string? Name { get; set; }
        [StringLength(20)]
        public string? SurName { get; set; }
        public Nullable<DateTime> LastOnline { get; set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }

    }
}
