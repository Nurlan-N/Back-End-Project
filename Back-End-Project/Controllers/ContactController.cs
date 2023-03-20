using Microsoft.AspNetCore.Mvc;

namespace Back_End_Project.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
