﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;


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
        private readonly UserManager<ApplicationUser> _userManager;

        //constructor, with dependency injection of dbContext
        public ShoppingCartsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }


        //shopping cart view
        public IActionResult Index()
        {
            return View();
        }

        //Order checkout view
        public IActionResult Checkout()
        {
            try
            {
                string shoppingCart = GetCookieValue("jo2024Cart");
                if (shoppingCart.IsNullOrEmpty()) { throw new Exception("Shopping Cart is empty"); };
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "ShoppingCarts", null);
            }
            return View();
        }


        // Prepare payment and call Payment API
        [HttpGet]
        public IActionResult Payment ()
        {
            //get shopping cart data
            string shoppingCart = "";
            try
            {
                shoppingCart = GetCookieValue("jo2024Cart");
                if (shoppingCart.IsNullOrEmpty()) { throw new Exception("Shopping Cart is empty"); };
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "ShoppingCarts", null);
            }
            // store shopping cart in a session value
            // to secure data from payment to database save
            HttpContext.Session.SetString("_TicketsInOrder", shoppingCart);

            //get data into tickets array
            JoTicketSimplified[] ticketsArray = JsonSerializer.Deserialize<JoTicketSimplified[]>(HttpContext.Session.GetString("_TicketsInOrder"));
    
            // calculate subtotal from cookies
            decimal orderAmount = GetSubtotal(ticketsArray);

            // get next orderId
            var max = _applicationDbContext.Orders.DefaultIfEmpty().Max(r => r == null ? 0 : r.OrderId);
            int orderId = max + 1;

            // send data to payment API: OrderId, OrderAmount, ShopPrivateKey
            // should be a POST request to a real Payment API
            return RedirectToAction("PaymentProcess", "MockPayment", new { orderId = orderId, orderAmount = orderAmount, sitewebjoKey = Environment.GetEnvironmentVariable("ApiPaymentKey") });
        }

        // Payment API sends response to this url
        [HttpGet]
        public IActionResult OrderTreatment(int orderId, decimal orderAmount, string transactionId, string status)
        {
            if (status == "Success")
            {
                //payment done
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //generate order and save in database
                Order order = new Order(orderId, userId, DateTime.Now, status, orderAmount, transactionId);
                _applicationDbContext.Orders.Add(order);
                _applicationDbContext.SaveChanges();

                // generate each ticket
                var ticketsInOrder = HttpContext.Session.GetString("_TicketsInOrder");
                JoTicketSimplified[] ticketsArray = JsonSerializer.Deserialize<JoTicketSimplified[]>(ticketsInOrder);
                List<JoTicket> joTicketArray = new List<JoTicket>();
                foreach (var ticket in ticketsArray)
                {
                    // update NbTotalBooked for the sessions
                    if (!UpdateSessionNbBooked(ticket))
                    {
                        return NotFound();
                    }

                    joTicketArray.Add(GenerateTicket(ticket, userId, order.OrderId));

                    // generate QR code

                }

                // generate invoice : TODO

                // send tickets and invoice to user email : TODO



                // empty shopping cart
                HttpContext.Session.SetString("_TicketsInOrder", "");
                Response.Cookies.Delete("jo2024Cart");

                return View(order);
            }
            else
            {
                //payment not done
                return View(null);
            }
        }


        // create a ShoppingCartTicket based on a JoTicketSimplified
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

        // function to calculate order subtotal
        public decimal GetSubtotal (JoTicketSimplified[] ticketsArray)
        {
            decimal subtotal = 0;

            // sort tickets by joSession and then by joTicketPack
            ticketsArray.OrderBy(x => x.JoSessionId).ThenBy(x => x.JoTicketPackId);

            JoTicketSimplified lastTicket = new JoTicketSimplified(-1, -1);
            foreach (var ticket in ticketsArray)
            {
                if (JsonSerializer.Serialize(ticket) != JsonSerializer.Serialize(lastTicket)) {

                    //get data of JoSessionPrice
                    decimal sessionPrice = (from s in _applicationDbContext.JoSessions
                              where s.JoSessionId == ticket.JoSessionId
                              select s.JoSessionPrice).FirstOrDefault();

                    //get data of joTicketPack: NbAttendees and ReductionRate
                    int nbAttendees = (from p in _applicationDbContext.JoTicketPacks
                               where p.JoTicketPackId == ticket.JoTicketPackId
                               select p.NbAttendees).FirstOrDefault();
                    decimal reductionRate = (from s in _applicationDbContext.JoTicketPacks
                                where s.JoTicketPackId == ticket.JoTicketPackId
                                select s.ReductionRate).FirstOrDefault();
                    
                    int nbTicket = Utilities.Utilities.CountSameTicket(ticket, ticketsArray);

                    subtotal += JoSessionsController.GetJoTicketPackPrice(sessionPrice, nbAttendees, reductionRate) * nbTicket;

                }
                lastTicket = ticket;
            }

            return subtotal;
        }

        
        // generate the ticket and save it in database
        // include generate secured ticketKey
        private JoTicket GenerateTicket(JoTicketSimplified ticket, string userId, int orderId)
        {
            // generate TicketKey
            byte[] ticketKey = Utilities.Utilities.GenerateSecureKey();

            // get TicketPrice
            JoSession joSession = (from s in _applicationDbContext.JoSessions
                                   where s.JoSessionId == ticket.JoSessionId
                                   select s).FirstOrDefault();
            JoTicketPack joTicketPack = (from p in _applicationDbContext.JoTicketPacks
                                   where p.JoTicketPackId == ticket.JoTicketPackId
                                   select p).FirstOrDefault();
            decimal ticketPrice = JoSessionsController.GetJoTicketPackPrice(joSession.JoSessionPrice, joTicketPack.NbAttendees, joTicketPack.ReductionRate);

            // save ticket in database
            // TicketStatus is set to true
            JoTicket completeTicket = new JoTicket()
            {
                ApplicationUserId = userId,
                JoTicketKey = ticketKey,
                JoTicketPackId=ticket.JoTicketPackId,
                JoSessionId = ticket.JoSessionId,
                JoTicketStatus = true,
                JoTicketPrice=ticketPrice,
                OrderId = orderId
            };
            _applicationDbContext.JoTickets.Add(completeTicket);
            _applicationDbContext.SaveChanges();

            return new JoTicket();
        }

        // update JoSessionNbTotalBooked : substract nb of places on ticket
        // return true if operation is well done
        private bool UpdateSessionNbBooked(JoTicketSimplified ticket)
        {
            
            try
            {
                // get nb of attendees for the pack
                var nb = (from p in _applicationDbContext.JoTicketPacks
                           where p.JoTicketPackId == ticket.JoTicketPackId
                           select p.NbAttendees).FirstOrDefault();

                // update nbTotalBooked in JoSessions
                var joSessionToUpdate= _applicationDbContext.JoSessions.FirstOrDefault(s => s.JoSessionId == ticket.JoSessionId);
                int newNbBooked = joSessionToUpdate.JoSessionNbTotalBooked - nb;
                if (newNbBooked < 0)
                {
                    throw new Exception("joSessionToUpdate.JoSessionNbTotalBooked can not be <0");
                }
                joSessionToUpdate.JoSessionNbTotalBooked = newNbBooked;
                _applicationDbContext.SaveChanges();
                return true;

            }
            catch { return false; }
        }


        // function to get a cookie value
        public string GetCookieValue(string cookieName)
        {
            StringValues values;
            HttpContext.Request.Headers.TryGetValue("Cookie", out values);
            var cookies = values.ToString().Split(';').ToList();
            var result = cookies.Select(c => new { Key = c.Split('=')[0].Trim(), Value = c.Split('=')[1].Trim() }).ToList();
            return result.FirstOrDefault(r => r.Key == cookieName).Value;
        }
    }

}
