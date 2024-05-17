using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace SiteWebJO2.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
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

        /// <summary>
        /// function to read string of qr code, check qr code authenticity and return ticket data
        /// the QR code string is read by javascript
        /// </summary>
        /// <param name="qrCodeString">the string read on qr code</param>
        /// <returns>ticket data</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string CheckQrCode([FromBody] ScanTicket scanTicket)
        {
            var ticket = new
            {
                ticketString = "testString"
            };

            if (scanTicket == null)
            {
                return null;
            }

            // get data

            //return ticket data if ticket authenticated

            // return null if ticket not authenticated
            return JsonSerializer.Serialize(ticket);
        }


    }
}
