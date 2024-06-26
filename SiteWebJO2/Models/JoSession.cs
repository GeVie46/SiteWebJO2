﻿
namespace SiteWebJO2.Models
{
    public class JoSession
    {
        public int JoSessionId { get; set; }
        public string JoSessionName { get; set; } = "DefaultName";
        public DateTime JoSessionDate { get; set; }
        public string JoSessionPlace { get; set; } = "DefaultPlace";
        public int JoSessionNbTotalAttendees { get; set; } = 0;
        public int JoSessionNbTotalBooked { get; set; } = 0;
        public string JoSessionDescription { get; set; } = "";
        public string JoSessionImage { get; set; } = "~/images/default.jpg";
        public decimal JoSessionPrice { get; set; } = 0;

        public ICollection<JoTicket> JoTickets { get; set; }
    }
}
