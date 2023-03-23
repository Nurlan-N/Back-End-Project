using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;
using Back_End_Project.ViewModels.WishlistViewModels;

namespace Back_End_Project.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();
        Task<IEnumerable<WishlistVM>> GetWishlist();
    }
}
