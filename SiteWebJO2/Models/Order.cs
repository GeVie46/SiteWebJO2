using System.ComponentModel.DataAnnotations;

namespace SiteWebJO2.Models
{
    /*
     * class Order : to save all orders done on web site
     * Property TransactionId : given by payment API whan payment is successful
     */
    public class Order (int orderId, string applicationUserId, DateTime orderDate, string orderStatus, decimal orderAmount, string transactionId)
    {
        public int OrderId { get; set; } = orderId;
        public string ApplicationUserId { get; set; } = applicationUserId;
        public DateTime OrderDate { get; set; } = orderDate;
        [MaxLength(255)]
        public string OrderStatus { get; set; } = orderStatus;
        public decimal OrderAmount { get; set; } = orderAmount;
        public string TransactionId { get; set; } = transactionId;

        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<JoTicket> JoTickets { get; set; }

    }

}
