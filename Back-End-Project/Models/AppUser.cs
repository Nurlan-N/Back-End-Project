﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(20)]
        public string? Name { get; set; }
        [StringLength(20)]
        public string? SurName { get; set; }
    }
}