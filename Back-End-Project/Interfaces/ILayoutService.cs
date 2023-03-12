namespace Back_End_Project.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();

    }
}
