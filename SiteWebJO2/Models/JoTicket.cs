namespace SiteWebJO2.Models
{

    /// <summary>
    /// class JoTicket : save all tickets sold
    /// Property JoTicketKey : secured ticket key
    /// Property JoTicketStatus : to know if attendees can accessed to session (true) or not (false, already check tickets)
    /// </summary>
    public class JoTicket
    {

        public int JoTicketId { get; set; }

        // use string for UserId because Identity Framework set AspNetUsers.Id to string, in database : varchar(255)
        public string ApplicationUserId { get; set; }
        public Byte[] JoTicketKey { get; set; }
        public int JoTicketPackId { get; set; }
        public int JoSessionId { get; set; }
        public bool JoTicketStatus { get; set; } = true;
        public decimal JoTicketPrice { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public JoTicketPack JoTicketPack { get; set; }
        public JoSession JoSession { get; set; }

    }


    /// <summary>
    /// Class used for sending cookie data from client side to server side
    /// </summary>
    /// <param name="joTicketPackId">JoTicketPacks.JoTicketPackId</param>
    /// <param name="joSessionId">JoSessions.JoSessionId</param>
    public class JoTicketSimplified (int joTicketPackId, int joSessionId)
    {
        public int JoTicketPackId { get; set; } = joTicketPackId;
        public int JoSessionId { get; set; } = joSessionId;

    }


    /// <summary>
    /// Ticket displayed in shopping cart
    /// </summary>
    /// <param name="joTicketPackId">JoTicketPacks.JoTicketPackId</param>
    /// <param name="joSessionId">JoSessions.JoSessionId</param>
    /// <param name="joSessionName">JoSessions.JoSessionName</param>
    /// <param name="joSessionDate">JoSessions.JoSessionDate</param>
    /// <param name="joSessionPlace">JoSessions.JoSessionPlace</param>
    /// <param name="joSessionImage">JoSessions.JoSessionImage</param>
    /// <param name="jopackPrice">price of ticket, calculated with function GetJoTicketPackPrice()</param>
    /// <param name="joTicketPackName">JoTicketPacks.JoTicketPackName</param>
    /// <param name="nbAttendees">JoTicketPacks.NbAttendees</param>
    /// <param name="reductionRate">JoTicketPacks.ReductionRate</param>
    public class ShoppingCartTicket (int joTicketPackId, int joSessionId, string joSessionName, DateTime joSessionDate, string joSessionPlace, string joSessionImage, decimal jopackPrice, String joTicketPackName, int nbAttendees, decimal reductionRate)
    {
        public int JoTicketPackId { get; set; } = joTicketPackId;
        public int JoSessionId { get; set; } = joSessionId;
        public string JoSessionName { get; set; } = joSessionName;
        public DateTime JoSessionDate { get; set; } = joSessionDate;
        public string JoSessionPlace { get; set; } = joSessionPlace;
        public string JoSessionImage { get; set; } = joSessionImage;
        public decimal JoPackPrice { get; set; } = jopackPrice;
        public String JoTicketPackName { get; set; } = joTicketPackName;
        public int NbAttendees { get; set; } = nbAttendees;
        public decimal ReductionRate { get; set; } = reductionRate;

    }

    /// <summary>
    /// class to describe the code included in Qr code
    /// </summary>
    /// <param name="ticketKeys">hash of ticket keys</param>
    /// <param name="firstname">firstname of attendee</param>
    /// <param name="lastname">lastnamt of attendee</param>
    public class ScanTicket(string ticketKeys, string firstname, string lastname)
    {
        public string TicketKeys { get; set; } = ticketKeys;
        public string Firstname { get; set; } = firstname;
        public string Lastname { get; set; } = lastname;
    }
}
