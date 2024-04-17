using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace SiteWebJO2.Models
{
    /*
     * Class JoTicketPack, define offers proposed to user
     * Property JoTicketPackStatus : indicates if pack is on use (true) or obsolete (false)
     */
    public class JoTicketPack
    {
        public int JoTicketPackId { get; set; }

        [MaxLength(255)]
        public String JoTicketPackName { get; set; } = "DefaultName";
        public int NbAttendees { get; set; } = 1;
        public decimal ReductionRate { get; set;}
        public bool JoTicketPackStatus { get; set; } = true;

        public ICollection<JoTicket> JoTickets { get; set; }
    }
}
