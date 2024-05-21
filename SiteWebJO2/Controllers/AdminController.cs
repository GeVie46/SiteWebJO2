using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SiteWebJO2.Services;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Net.WebRequestMethods;

namespace SiteWebJO2.Controllers
{
    [Authorize(Roles ="admin")]
    [AutoValidateAntiforgeryToken]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        //constructor, with dependency injection of dbContext
        public AdminController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        /// <summary>
        /// return view to check access
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckAccess() {
            return View();
        }

        /// <summary>
        /// function to read string of qr code, check qr code authenticity and return ticket data
        /// the QR code string is read by javascript
        /// </summary>
        /// <param name="qrCodeString">the string read on qr code</param>
        /// <param name="ticket">object used to return ticket data</param>
        /// <returns>ticket data</returns>
        [HttpPost]
        public string CheckQrCode([FromBody] ScanTicket scanTicket, object? ticket = null)
        {
            try
            {
                if (scanTicket == null) { return JsonSerializer.Serialize(new { msg="invalid ticket"}); }
                string username = scanTicket.Username;
                if (username.IsNullOrEmpty()) { JsonSerializer.Serialize(new { msg = "invalid ticket" }); }
                ticket = new {};

                // get user corresponding to username
                ApplicationUser user = (from u in _applicationDbContext.Users
                                               where u.UserName == username
                                               select u).FirstOrDefault();
                if (user == null) { return JsonSerializer.Serialize(new { msg = "User unknown" }); }

                // get all tickets of user
                List<JoTicket> tickets = (from t in _applicationDbContext.JoTickets
                                            where t.ApplicationUserId == user.Id
                                            select t).ToList();

                if (tickets.Count==0) { return JsonSerializer.Serialize(new { msg = "No tickets found for this user" }); }

                // compare qr code key to hash of ticketKey+userKey
                tickets.ForEach(t =>
                {
                    // concatenate user key and ticket key and hash
                    byte[] concatKeys = SHA256.HashData(user.Userkey.Concat(t.JoTicketKey).ToArray());

                    if (scanTicket.TicketKeys.Equals(Convert.ToBase64String(concatKeys)))
                    {
                        // ticket authenticated, create an object with all data needed
                        //get data of joSession
                        var js = (from s in _applicationDbContext.JoSessions
                                    where s.JoSessionId == t.JoSessionId
                                    select s).FirstOrDefault();

                        //get data of joTicketPack
                        var jtp = (from p in _applicationDbContext.JoTicketPacks
                                    where p.JoTicketPackId == t.JoTicketPackId
                                    select p).FirstOrDefault();

                        //create object to send data to html page
                        ticket = new
                        {
                            firstname = Utilities.Utilities.CapitalizeFirstLetter(user.Name),
                            lastname = Utilities.Utilities.CapitalizeFirstLetter(user.Lastname),
                            sessionName = js.JoSessionName,
                            sessionPlace = js.JoSessionPlace,
                            sessionDate = js.JoSessionDate,
                            packName = jtp.JoTicketPackName,
                            packNb = jtp.NbAttendees
                        };
                    }
                });
                
                return JsonSerializer.Serialize(ticket);

            }
            catch
            {
                return JsonSerializer.Serialize(new { msg = "Error while processing check" });
            }
        }

    }
}
