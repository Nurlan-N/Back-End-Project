﻿using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        [StringLength(255)]
        public string? UpdatetBy { get; set; }
        public Nullable<DateTime> UpdatetAt { get; set; }
        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
