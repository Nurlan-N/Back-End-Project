using Back_End_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.ViewModels.AccountViewModels
{
    public class ProfileVM
    {
        [StringLength(20)]
        public string? Name { get; set; }
        [StringLength(20)]
        public string? SurName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirimPassword { get; set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public Address Address { get; internal set; }
    }
}
