using SiteWebJO2.Controllers;
using SiteWebJO2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SiteWebJo2.UnitTests
{
    [TestClass]
    public class GetOnUseJoTicketPacks_UnitTest
    {
        [TestMethod]
        public void JoTicketPackShouldBeDisplayed()
        {
            //create OnUse JoTicketPack
            List<JoTicketPack> joTicketPacks = [
                new JoTicketPack { JoTicketPackName="Test TicketPack OnUse", NbAttendees=3, ReductionRate=0.2m, JoTicketPackStatus=true}
                ];
            IQueryable<JoTicketPack> joTicketPacksQueryable = joTicketPacks.AsQueryable();

            //apply method to test
            IQueryable<JoTicketPack> result = JoSessionsController.GetOnUseJoTicketPacks(joTicketPacksQueryable);

            //check if test passes
            // use JavaScriptSerializer() to compare objects
            Assert.AreEqual(JsonSerializer.Serialize(joTicketPacksQueryable), JsonSerializer.Serialize(result), "the pack " + joTicketPacks[0].JoTicketPackName + "should be in the result, but the program filter it out");
        }

        [TestMethod]
        public void JoTicketPackShouldBeNOTDisplayed()
        {
            //create Obsolete JoTicketPack
            List<JoTicketPack> joTicketPacks = [
                new JoTicketPack { JoTicketPackName="Test TicketPack OBSOLETE", NbAttendees=3, ReductionRate=0.2m, JoTicketPackStatus=false}
                ];
            IQueryable<JoTicketPack> joTicketPacksQueryable = joTicketPacks.AsQueryable();

            //apply method to test
            IQueryable<JoTicketPack> result = JoSessionsController.GetOnUseJoTicketPacks(joTicketPacksQueryable);

            //check if test passes
            // use JavaScriptSerializer() to compare objects
            Assert.AreNotEqual(JsonSerializer.Serialize(joTicketPacksQueryable), JsonSerializer.Serialize(result), "the pack " + joTicketPacks[0].JoTicketPackName + "should NOT be in the result, but the program keeps it");
        }
    }
}
