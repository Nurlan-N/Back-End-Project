using Back_End_Project.Models;

namespace Back_End_Project.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        public IEnumerable<Category>? Categories { get; set; }
        public PageNatedList<Product>? Products { get; set;} 
    }
}
