using Back_End_Project.DataAccessLayer;
using Back_End_Project.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _appDbContext;

        public LayoutService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _appDbContext.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }

    }
}
