using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;

namespace Back_End_Project.ViewModels.OrderVIewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<BasketVM>? BasketVMs { get; set; }
    }
}
