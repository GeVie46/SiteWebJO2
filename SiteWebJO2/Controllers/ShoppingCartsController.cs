using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SiteWebJO2.Controllers
{

    /*
     * * Manage shopping cart
     * access only for authentified users
     */
    [Authorize]
    public class ShoppingCartsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
