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

        public IActionResult Dashboard()
        {
            return View();
        }


        /// <summary>
        /// return view to check access
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckAccess() {
            return View();
        }

        //public string CheckQrCode([FromBody] ScanTicket scanTicket)
        //{
        //    return CheckQrCode(scanTicket, null);
        //}

        /// <summary>
        /// function to read string of qr code, check qr code authenticity and return ticket data
        /// the QR code string is read by javascript
        /// </summary>
        /// <param name="qrCodeString">the string read on qr code</param>
        /// <returns>ticket data</returns>
        [HttpPost]
        public string CheckQrCode([FromBody] ScanTicket scanTicket, object? ticket = null)
        {
            try
            {
                string username = scanTicket.Username;
                ticket = new
                {
                    firstname = "",
                    lastname = "",
                    sessionName = "",
                    sessionPlace = "",
                    sessionDate = "",
                    packName = "",
                    packNb = ""
                };

                // get user corresponding to username
                ApplicationUser user = (from u in _applicationDbContext.Users
                                               where u.UserName == username
                                               select u).FirstOrDefault();

                // exception : no user found

                // get all tickets of user
                List<JoTicket> tickets = (from t in _applicationDbContext.JoTickets
                                            where t.ApplicationUserId == user.Id
                                            select t).ToList();

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
                        //ticket.sessionName = js.JoSessionName;

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

                // return null if ticket not authenticated 
                //return null;
            }
            catch
            {
                // return null if error
                return null;
            }
        }


    }
}
