using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Primitives;
using SiteWebJO2.Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace SiteWebJO2.Utilities
{
    public class Utilities
    {

        /// <summary>
        /// Generate a 128-bit salt using a sequence of cryptographically strong random bytes.
        /// code from https://learn.microsoft.com/fr-fr/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-8.0
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSecureKey()
        {
            byte[] key = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            return key;
        }

        /// <summary>
        /// function to count number of same ticket. same function as javascript one
        /// </summary>
        /// <param name="ticket">the ticket to count</param>
        /// <param name="cart">the tickets array to count in</param>
        /// <returns>the number of ticket in tickets array</returns>
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

        /// <summary>
        /// function to put first letter of a word in upper case
        /// </summary>
        /// <param name="text">the word to treat</param>
        /// <returns>the same word with first letter in upper case</returns>
        public static string CapitalizeFirstLetter (string text)
        {
            return Char.ToUpper(text[0]) + text.Substring(1);
        }


        /// <summary>
        /// to log all exceptions in development environment. Call it with "try {...} catch (Exception e) when(LogException(e)) {}
        /// code from https://learn.microsoft.com/fr-fr/dotnet/csharp/fundamentals/exceptions/exception-handling
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool LogException(Exception e)
        {
            Console.WriteLine($"\tIn the log routine. Caught {e.GetType()}");
            Console.WriteLine($"\tMessage: {e.Message}");
            return false;
        }
    }
}
