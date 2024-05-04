using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using System.Text.Json;


namespace SiteWebJO2.Controllers
{

    /*
     * * Manage shopping cart
     * access only for authentified users
     */
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        //constructor, with dependency injection of dbContext
        public ShoppingCartsController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public string GetTicketData([FromBody] JoTicketSimplified joTicketSimplified) {

            //get data of joSession
            var js = (from s in _applicationDbContext.JoSessions
                      where s.JoSessionId == joTicketSimplified.JoSessionId
                      select s).FirstOrDefault();

            //get data of joTicketPack
            var jtp = (from p in _applicationDbContext.JoTicketPacks
                       where p.JoTicketPackId == joTicketSimplified.JoTicketPackId
                       select p).FirstOrDefault();

            if (js != null && jtp != null)
            {
                //create object
                ShoppingCartTicket ticket = new ShoppingCartTicket(jtp.JoTicketPackId, js.JoSessionId, js.JoSessionName, js.JoSessionDate, js.JoSessionPlace, js.JoSessionImage, JoSessionsController.GetJoTicketPackPrice(js.JoSessionPrice, jtp.NbAttendees, jtp.ReductionRate),  jtp.JoTicketPackName, jtp.NbAttendees, jtp.ReductionRate);
                return JsonSerializer.Serialize(ticket);
            }
            else
            {
                throw new InvalidOperationException("Error during ticket data request");
            }
        }
    }

}
