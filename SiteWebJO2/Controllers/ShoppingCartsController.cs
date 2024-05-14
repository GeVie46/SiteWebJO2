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
using System.Net.Mail;
using SiteWebJO2.Services;
using Microsoft.Extensions.Options;
using System.Numerics;
using System.Globalization;


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
        private readonly IWebHostEnvironment _env;
       
        // to get the SendGrid key
        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        //constructor, with dependency injection of dbContext
        public ShoppingCartsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _env = environment; // to get wwwroot path
            Options = optionsAccessor.Value;
        }


        /// <summary>
        /// Shopping cart view
        /// </summary>
        /// <returns>view ShoppingCarts/Index</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Order checkout view
        /// </summary>
        /// <returns>view ShoppingCarts/Checkout</returns>
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


        /// <summary>
        /// Prepare payment and call Payment API
        /// </summary>
        /// <returns>redirect to Payment API view</returns>
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


        /// <summary>
        /// url to receive Payment API response and continue order treatment
        /// </summary>
        /// <param name="orderId">order to treat</param>
        /// <param name="orderAmount">amount of the order</param>
        /// <param name="transactionId">transaction Id done between Payment API and bank</param>
        /// <param name="status">status of payment transaction</param>
        /// <returns>view to inform if order is completed or rejected</returns>
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
                List<string> ticketPDF = GenerateTicketPDF(joTicketList, user, qrCodeList);

                // generate invoice : TODO

                // send tickets and invoice to user email : TODO
                SendGrid.Response response = SendOrderEmail(user, order, ticketPDF);
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.Accepted)
                || response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
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


        /// <summary>
        /// Function to send email following the order passed, with each ticket attached
        /// using NuGet package SendGrid 
        /// </summary>
        /// <param name="user">user to send email to</param>
        /// <param name="order">order passed</param>
        /// <param name="ticketPDF">list of PDF tickets filepaths</param>
        /// <returns>result of task 'sendEmail'</returns>
        /// <exception cref="Exception">exception thrown if SendGrid key is unfound</exception>
        public SendGrid.Response SendOrderEmail(ApplicationUser user, Order order, List<string> ticketPDF)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            var client = new SendGridClient(Options.SendGridKey);
            var from = new EmailAddress("gevie46000@gmail.com", "Jo2024Tickets.fly.dev");
            var subject = "Order id: " + order.OrderId;
            var to = new EmailAddress(user.Email, user.Name);

            var plainTextContent = "Hello " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) + ", " +
                "This email is sent following the order you've done on the jo2024Tickets.fly.dev website on " + order.OrderDate.ToString("f", CultureInfo.CreateSpecificCulture("en-US")) + "." +
                "" +
                "Order id: " + order.OrderId +
                "" +
                "Attendee name: " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) +
                "" +
                "Please find all tickets attached to this email." +
                "" +
                "Important reminder: " +
                "Please present yourself to the event 2 hours before starting time with your(s) ticket(s)" +
                "QR code must be visible, and you must present an official ID card to justify your identity (corresponding to the one mentioned above).";

            var htmlContent = "Hello " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) + ", " +
                "<br /> This email is sent following the order you've done on the jo2024Tickets.fly.dev website on " + order.OrderDate.ToString("f", CultureInfo.CreateSpecificCulture("en-US")) + "." +
                "<br /> " +
                "<br /> Order id: " + order.OrderId +
                "<br /> " +
                "<br /> Attendee name: " + Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname) +
                "<br /> " +
                "<br /> Please find all tickets attached to this email." +
                "<br /> " +
                "<br /> <strong>Important reminder: </strong>" +
                "<br /> Please present yourself to the event 2 hours before starting time with your(s) ticket(s)" +
                "<br /> QR code must be visible, and you must present an official ID card to justify your identity (corresponding to the one mentioned above).";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // add attachements: tickets
            if (ticketPDF.Count != 0)
            {
                foreach (var ticketPath in ticketPDF)
                {
                    var bytes = System.IO.File.ReadAllBytes(ticketPath);
                    var file = Convert.ToBase64String(bytes);
                    msg.AddAttachment("ticket.pdf", file);
                }
            }

            // send email
            var response = client.SendEmailAsync(msg).Result;
            return response;
        }

        /// <summary>
        /// create a ShoppingCartTicket based on a JoTicketSimplified, to display data on shopping cart page
        /// </summary>
        /// <param name="joTicketSimplified">the ticket we want more data on</param>
        /// <returns>ShoppingCartTicket corresponding to the ticket given</returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// function to calculate order subtotal
        /// </summary>
        /// <param name="ticketsArray">tickets array in the order</param>
        /// <returns>subtotal of all tickets price, considering JoTicketPacks (NbAttendees, ReductionRate) and JoSessionPrice</returns>
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


        /// <summary>
        /// generate the ticket and save it in database. include generate secured ticketKey
        /// </summary>
        /// <param name="ticket">ticket with JoSessionId and JoTicketPackId</param>
        /// <param name="userId">Id for the user who attends the JoSession</param>
        /// <param name="orderId">Id of the order passed to buy this ticket</param>
        /// <returns>JoTicket created with data given in params and database</returns>
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

        /// <summary>
        /// generate the QRcode for all tickets
        /// using NuGet package QRCoder https://www.nuget.org/packages/QRCoder/
        /// </summary>
        /// <param name="joTicketList">list of JoTicket to generate</param>
        /// <param name="user">user who attends the JoSession</param>
        /// <returns>list of QRcode generated</returns>
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

        /// <summary>
        /// create a PDF for each ticket, containing ticket informations and QR code. Use QuestPDF library
        /// </summary>
        /// <param name="joTicketList">list of JoTicket to generate</param>
        /// <param name="user">user who attends the JoSession</param>
        /// <param name="qrCodeList">list of QRCode generated from JoTicket data, same order than joTicketList</param>
        /// <returns>list of PDF filepaths, one for each ticket. Files saved on wwwroot</returns>
        private List<string> GenerateTicketPDF(List<JoTicket> joTicketList, ApplicationUser user, List<byte[]> qrCodeList)
        {
            // get wwwroot path
            string rootPath = this._env.WebRootPath;

            List<string> ticketPDF = new List<string>();
            for (int i = 0; i < joTicketList.Count; i++)
            {
                try
                {
                JoTicket ticket = joTicketList[i];
                
                    // get Ticket data
                    JoSession joSession = (from s in _applicationDbContext.JoSessions
                                       where s.JoSessionId == ticket.JoSessionId
                                       select s).FirstOrDefault();
                    JoTicketPack joTicketPack = (from p in _applicationDbContext.JoTicketPacks
                                             where p.JoTicketPackId == ticket.JoTicketPackId
                                             select p).FirstOrDefault();

                // create page layout
                var pdf = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(50);
                            page.Size(PageSizes.A4);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(18));
                            page.DefaultTextStyle(x => x.FontColor(Colors.Grey.Darken4));

                            page.Header()
                                .AlignCenter()
                                .Column(column =>
                                {
                                    column.Item().Text(text =>
                                    {
                                        text.Span("Olympic Games 2024").SemiBold().FontSize(24).FontColor(Colors.Grey.Darken4);
                                        text.Span(" PA").SemiBold().FontSize(24).FontColor("#00A0FE");
                                        text.Span("R").SemiBold().FontSize(24).FontColor(Colors.Grey.Darken4);
                                        text.Span("IS ").SemiBold().FontSize(24).FontColor("#F61732");
                                    });
                                    column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Darken4);
                                    column.Item().Text("");
                                });

                            page.Content()
                        .Column(x =>
                        {
                            x.Item().Text(text =>
                            {
                                text.Span("Session: ").FontSize(24);
                                text.Span(joSession.JoSessionName).FontSize(24);

                            });
                            x.Item().Text(text =>
                            {
                                text.Span("Place: ");
                                text.Span(joSession.JoSessionPlace);

                            });
                            x.Item().Text(text =>
                            {
                                text.Span("Date: ");
                                text.Span(joSession.JoSessionDate.ToString("f", CultureInfo.CreateSpecificCulture("en-US")));

                            });
                            x.Item().Text(text =>
                            {
                                text.Span("Pack: ");
                                text.Span(joTicketPack.JoTicketPackName + " (" + joTicketPack.NbAttendees + " attendees)");

                            });
                            x.Item().Text(text =>
                            {
                                text.Span("Main attendee name: ");
                                text.Span(Utilities.Utilities.CapitalizeFirstLetter(user.Name) + " " + Utilities.Utilities.CapitalizeFirstLetter(user.Lastname));

                            });
                            x.Item().Image(qrCodeList[i]);
                        });
                        });
                    });
                // create PDF file
                var filePath = rootPath + "/ticket" + i + ".pdf";
                pdf.GeneratePdf(filePath);
                ticketPDF.Add(filePath);
                }
                catch (Exception e) when (Utilities.Utilities.LogException(e)) { }
            }

            return ticketPDF;
        }

        /// <summary>
        /// update JoSessionNbTotalBooked : substract nb of places on ticket
        /// </summary>
        /// <param name="ticket">ticket to substract to JoSession. To get JoSessionId and NbAttendees</param>
        /// <returns>return true if operation is well done</returns>
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
            catch (Exception e) when (Utilities.Utilities.LogException(e)) { return false; }
        }


        /// <summary>
        /// function to get a cookie value
        /// </summary>
        /// <param name="cookieName">name of the cookie</param>
        /// <returns>cookie value</returns>
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
