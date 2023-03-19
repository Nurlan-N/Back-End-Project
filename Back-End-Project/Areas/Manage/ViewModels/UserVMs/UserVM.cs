
using System.Data;

namespace Back_End_Project.Areas.Manage.ViewModels.UserVMs
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
