using Microsoft.AspNetCore.Mvc;

namespace SiteWebJO2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
