using Back_End_Project.Models;

namespace Back_End_Project.ViewModels.HomeViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider>? Sliders { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Blog>? Blogs { get; set; }
    }
}
