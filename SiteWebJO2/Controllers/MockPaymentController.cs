
using Microsoft.AspNetCore.Mvc;
using SiteWebJO2.Models;
using System.Numerics;


namespace SiteWebJO2.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class MockPaymentController : Controller
    {

        /// <summary>
        /// entry to Payment API
        /// </summary>
        /// <param name="orderId">order to treat</param>
        /// <param name="orderAmount">amount of order</param>
        /// <param name="clientSiteKey">client key to recognize client id</param>
        /// <returns>view MockPayment/PaymentProcess, to display order informations and ask for payment </returns>
        public IActionResult PaymentProcess(int orderId, decimal orderAmount, string clientSiteKey)
        {

            //identify clientSite
            string clientName = "";
            if(clientSiteKey == Environment.GetEnvironmentVariable("ApiPaymentKey"))
            {
                clientName = "Jo2024Tickets";
            }

            // create new Transaction
            string transactionId = Guid.NewGuid().ToString();
            string status = "Created";
            Transaction transaction = new Transaction(transactionId, orderId, orderAmount, clientName, status);
            
            return View(transaction);
        }


        /// <summary>
        /// user pay successful, call client web site to inform
        /// </summary>
        /// <param name="transaction">transation treated</param>
        /// <returns>redirect to client app, with status 'Success'</returns>
        [HttpPost]
        public IActionResult PaymentOk(Transaction transaction)
        {
            transaction.Status = "Success";
            return RedirectToAction("OrderTreatment", "ShoppingCarts",  new { orderId = transaction.OrderId, orderAmount = transaction.OrderAmount, transactionId = transaction.TransactionId, status = transaction.Status});
        }

        /// <summary>
        /// user pay abort, call client web site to inform
        /// </summary>
        /// <param name="transaction">transation treated</param>
        /// <returns>redirect to client app, with status 'Abort'</returns>
        [HttpPost]
        public IActionResult PaymentAbort(Transaction transaction)
        {
            transaction.Status = "Abort";
            return RedirectToAction("OrderTreatment", "ShoppingCarts", new { orderId = transaction.OrderId, orderAmount = transaction.OrderAmount, transactionId = transaction.TransactionId, status = transaction.Status });
        }
    }


    /// <summary>
    /// class to define object manipulated by MockPayment
    /// </summary>
    public class Transaction
    {
        public string TransactionId { get; set; }
        public int OrderId { get; set; } 
        public decimal OrderAmount { get; set; } 
        public string ClientName { get; set; } 
        public string Status { get; set; }

        public Transaction()
        {
            // Parameterless constructor
        }

        public Transaction (string transactionId, int orderId, decimal orderAmount, string clientName, string status)
        {
            TransactionId =transactionId;
            OrderId= orderId;
            OrderAmount= orderAmount;
            ClientName = clientName;
            Status= status;
        }

    }
}
