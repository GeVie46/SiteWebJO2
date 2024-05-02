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

    public class JoTicketSimplified
    {
        public int joTicketPackId { get; set; }
        public int joSessionId { get; set; }
    }
}
