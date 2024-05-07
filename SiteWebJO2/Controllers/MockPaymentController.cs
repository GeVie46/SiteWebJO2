
using Microsoft.AspNetCore.Mvc;
using System.Numerics;


namespace SiteWebJO2.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class MockPaymentController : Controller
    {

        // entry to Payment API
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

        // user pay successful, call client web site to inform
        [HttpPost]
        public IActionResult PaymentOk(Transaction transaction)
        {
            transaction.Status = "Success";
            return RedirectToAction("");
        }

        // user pay abort, call client web site to inform
        [HttpPost]
        public string PaymentAbort(Transaction transaction)
        {
            transaction.Status = "Abort";
            return "";
        }
    }

    // class to define object manipulated by MockPayment
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
