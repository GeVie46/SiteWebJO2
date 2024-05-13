using Microsoft.AspNetCore.Authorization;
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
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Drawing;
using System.Security.Cryptography;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SiteWebJO2.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.Encodings.Web;
using QRCoder;


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
        private readonly IEmailSender _emailSender;

        //constructor, with dependency injection of dbContext
        public ShoppingCartsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _emailSender = emailSender; // TODO : error in email config
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
                List<JoTicket> joTicketList = new List<JoTicket>();
                foreach (var ticket in ticketsArray)
                {
                    // update NbTotalBooked for the sessions
                    if (!UpdateSessionNbBooked(ticket))
                    {
                        return NotFound();
                    }

                    // generate ticket
                    joTicketList.Add(GenerateTicket(ticket, userId, order.OrderId));

                }

                // generate QR code
                ApplicationUser user = (from u in _applicationDbContext.Users
                                        where u.Id == userId
                                        select u).FirstOrDefault();
                List<byte[]> qrCodeList = new List<byte[]>();
                qrCodeList = GenerateQrCode(joTicketList, user);

                // generate ticket PDF
                List<Document> ticketPDF = GenerateTicketPDF(joTicketList, user, qrCodeList);

                // generate invoice : TODO

                // send tickets and invoice to user email : TODO
                if (SendOrderEmail(user, order, ticketPDF).Result)
                {
                    // empty shopping cart
                    HttpContext.Session.SetString("_TicketsInOrder", "");
                    Response.Cookies.Delete("jo2024Cart");
                }

                

                return View(order);
            }
            else
            {
                //payment not done
                return View(null);
            }
        }

        /*
         *  Function to send email with tickets and order
         */
        public async Task<bool> SendOrderEmail(ApplicationUser user, Order order, List<Document> ticketPDF)
        {
            try
            {
                await _emailSender.SendEmailAsync(user.Email, "Order #" + order.OrderId, 
                    "Hello " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) 
                    + ", \r\n this email is sent following the order you've done on the jo2024Tickets.fly.dev website.");
                return true;
            }
            catch
            {
                return false;
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

            return completeTicket;
        }

        // generate the QRcode for all tickets
        // using NuGet package QRCoder https://www.nuget.org/packages/QRCoder/
        private List<byte[]> GenerateQrCode(List<JoTicket> joTicketList, ApplicationUser user)
        {
            // define licence for QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            List<byte[]> qrCodeList = new List<byte[]>();
            string qrCodeStr;
            byte[] userKey = user.Userkey;
            foreach (var item in joTicketList)
            {
                // concatenate user key and ticket key
                byte[] concatKeys = userKey.Concat(item.JoTicketKey).ToArray();

                // hash concatenation, using SHA-256 hash algorithm, and convert to string
                string hashValueStr = Convert.ToBase64String(SHA256.HashData(concatKeys));

                // add user firstname and lastname
                qrCodeStr = hashValueStr + ";Firstname:" + user.Name + ";Lastname:" + user.Lastname;

                // create QR code
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeStr, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                qrCodeList.Add(qrCodeImage);
            }

            return qrCodeList;
        }

        // create a PDF for each ticket, containing ticket informations and QR code
        private List<Document> GenerateTicketPDF(List<JoTicket> joTicketList, ApplicationUser user, List<byte[]> qrCodeList)
        {
            List<Document> ticketPDF = new List<Document>();
            for (int i = 0; i < joTicketList.Count; i++)
            {
                JoTicket ticket = joTicketList[i];
                // get Ticket data
                JoSession joSession = (from s in _applicationDbContext.JoSessions
                                       where s.JoSessionId == ticket.JoSessionId
                                       select s).FirstOrDefault();
                JoTicketPack joTicketPack = (from p in _applicationDbContext.JoTicketPacks
                                             where p.JoTicketPackId == ticket.JoTicketPackId
                                             select p).FirstOrDefault();

                var pdf = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(50);
                            page.Size(PageSizes.A4);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(16));

                            page.Header()
                                .AlignCenter()
                                .Text("Olympic Games 2024 PARIS")
                                .SemiBold().FontSize(24).FontColor(Colors.Grey.Darken4);

                            page.Content()
                        .Column(x =>
                        {
                            x.Item().Text(text =>
                            {
                                text.Span("Session: " + joSession.JoSessionName);
                                text.Span("Place: " + joSession.JoSessionPlace);
                                text.Span("Date: " + joSession.JoSessionDate);
                                text.Span("Pack: " + joTicketPack.JoTicketPackName + " (" + joTicketPack.NbAttendees + " attendees)");
                                text.Span("Main attendee name: " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) );
                            });
                            x.Item().Image(qrCodeList[i]);
                        });
                        });
                    });
                pdf.GeneratePdf("ticket" + i + ".pdf");
                ticketPDF.Add(pdf);
            }

            return ticketPDF;
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
