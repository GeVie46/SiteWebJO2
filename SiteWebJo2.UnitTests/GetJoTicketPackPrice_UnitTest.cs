using SiteWebJO2.Controllers;
using SiteWebJO2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWebJo2.UnitTests
{
    [TestClass]
    public class GetJoTicketPackPrice_UnitTest
    {
        [TestMethod]
        // check that joTicketPack price is well calculated
        public void GetJoTicketPackPrice_Test() {
            //create a JoSession
            JoSession joSession = new JoSession
            {
                JoSessionName = "Test session",
                JoSessionDate = new DateTime(2024, 8, 1, 11, 0, 0),
                JoSessionPlace = "Versailles, Château de Versailles",
                JoSessionNbTotalAttendees = 10000,
                JoSessionNbTotalBooked = 5000,
                JoSessionDescription = "",
                JoSessionImage = @"",
                JoSessionPrice = 100
            };

            //create a JoTicketPack
            JoTicketPack joTicketPack = new JoTicketPack { JoTicketPackName = "Football team", NbAttendees = 11, ReductionRate = 0.20m, JoTicketPackStatus = true };

            //apply method to test
            decimal result = JoSessionsController.GetJoTicketPackPrice(joSession.JoSessionPrice, joTicketPack.NbAttendees, joTicketPack.ReductionRate);

            //check if test passes
            Assert.AreEqual(880.00m, result, "the price should be 880.00 but is " + result);

        }
    }
}
