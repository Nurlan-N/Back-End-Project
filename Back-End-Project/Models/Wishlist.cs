﻿namespace Back_End_Project.Models
{
    public class Wishlist : BaseEntity
    {
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public int Count { get; set; }
    }
}
