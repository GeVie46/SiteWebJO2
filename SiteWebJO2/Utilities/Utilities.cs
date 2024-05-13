using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Primitives;
using SiteWebJO2.Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace SiteWebJO2.Utilities
{
    public class Utilities
    {
        /*
         * method GenerateSecureKey(): Generate a 128-bit salt using a sequence of cryptographically strong random bytes.
         * code from
         * https://learn.microsoft.com/fr-fr/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-8.0
         */
        public static byte[] GenerateSecureKey()
        {
            byte[] key = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            return key;
        }


        // function to count number of same ticket
        // same function as javascript one
        public static int CountSameTicket(JoTicketSimplified ticket, JoTicketSimplified[] cart)
        {
            int countSameticket = 0;
            foreach (var t in cart)
            {
                if (JsonSerializer.Serialize(ticket) == JsonSerializer.Serialize(t))
                {
                    countSameticket++;
                }
            }
            return countSameticket;
        }

        // function to put first letter in upper case
        public static string CapitalizeFirstLetter (string text)
        {
            return Char.ToUpper(text[0]) + text.Substring(1);
        }
        

    }
}
