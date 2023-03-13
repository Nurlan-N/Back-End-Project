using System.ComponentModel.DataAnnotations;

namespace Back_End_Project.Models
{
    public class ProductImage: BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
