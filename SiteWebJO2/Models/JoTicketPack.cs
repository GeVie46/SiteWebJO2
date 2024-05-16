using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace SiteWebJO2.Models
{

    /// <summary>
    /// Class JoTicketPack, define offers proposed to user
    /// Property JoTicketPackStatus : indicates if pack is on use (true) or obsolete (false)
    /// </summary>
    public class JoTicketPack
    {
        public int JoTicketPackId { get; set; }

        [MaxLength(255)]
        [DisplayName("Pack name")]
        public String JoTicketPackName { get; set; } = "DefaultName";

        [DisplayName("Attendees nb")]
        public int NbAttendees { get; set; } = 1;

        [DisplayName("Reduction rate")]
        public decimal ReductionRate { get; set;}

        [DisplayName("Currently used")]
        public bool JoTicketPackStatus { get; set; } = true;

        
        public ICollection<JoTicket> JoTickets { get; set; }
    }


    /// <summary>
    /// class to display JoTicketPack with count of nb sold
    /// </summary>
    public class DisplayedJoTicketPack
    {
        public int JoTicketPackId { get; set; }

        [MaxLength(255)]
        [DisplayName("Pack name")]
        public String JoTicketPackName { get; set; } = "DefaultName";

        [DisplayName("Attendees nb")]
        public int NbAttendees { get; set; } = 1;

        [DisplayName("Reduction rate")]
        public decimal ReductionRate { get; set; }

        [DisplayName("Currently used")]
        public bool JoTicketPackStatus { get; set; } = true;

        [DisplayName("Packs sold")]
        public int NbPacksSold { get; set; } = 0;

    }
}
