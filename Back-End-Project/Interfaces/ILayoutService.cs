using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;

namespace Back_End_Project.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<BasketVM>> GetBaskets();

    }
}
