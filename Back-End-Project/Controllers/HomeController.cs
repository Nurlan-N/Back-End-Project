using Microsoft.AspNetCore.Mvc;

namespace Back_End_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
