using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Primitives;
using SiteWebJO2.Models;
using System.Text.Json;

namespace SiteWebJO2.Utilities
{
    public class Utilities
    {

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
        

    }
}
