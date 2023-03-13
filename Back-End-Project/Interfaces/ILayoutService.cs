using Back_End_Project.ViewModels.BasketViewModels;

namespace Back_End_Project.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();

    }
}
