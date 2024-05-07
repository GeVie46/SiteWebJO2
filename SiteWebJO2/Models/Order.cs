using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteWebJO2.Models
{
    /*
     * class Order : to save all orders done on web site
     * Property TransactionId : given by payment API whan payment is successful
     */
    public class Order
    {
        public int OrderId { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime OrderDate { get; set; }
        [MaxLength(255)]
        public String OrderStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public string TransactionId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<JoTicket> JoTickets { get; set; }

    }

}
