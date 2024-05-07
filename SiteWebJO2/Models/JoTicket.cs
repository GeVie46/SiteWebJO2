namespace SiteWebJO2.Models
{

    /*
    * class JoTicket : save all tickets sold
    * Property JoTicketKey : secured ticket key
    * Property JoTicketStatus : to know if attendees can accessed to session (true) or not (false, already check tickets)
    */
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

    /*
     * Class used for sending cookie data from client side to server side
     */
    public class JoTicketSimplified (int joTicketPackId, int joSessionId)
    {
        public int JoTicketPackId { get; set; } = joTicketPackId;
        public int JoSessionId { get; set; } = joSessionId;

    }

    /*
     * Class used for display shopping cart
     */
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
}
