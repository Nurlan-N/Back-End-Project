using Back_End_Project.Models;

namespace Back_End_Project.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        public int? CategoryId { get; set; }
        public int Sort { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public PageNatedList<Product>? Products { get; set;} 
    }
}
