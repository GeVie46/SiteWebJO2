using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteWebJO2.Models;
using System.Text.Json;

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

      
        public string GetTicketData([FromBody] JoTicketSimplified joTicketSimplified) {
  
            return JsonSerializer.Serialize(new { Name = joTicketSimplified.joTicketPackId, DateTime = DateTime.Now.ToShortDateString() });
        }
    }

}
