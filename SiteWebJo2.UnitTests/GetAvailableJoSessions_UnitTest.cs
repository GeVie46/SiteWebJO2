using SiteWebJO2.Data;
using SiteWebJO2.Models;
using SiteWebJO2.Controllers;
using System.Text.Json;
using System.Collections.Generic;

namespace SiteWebJo2.UnitTests
{

    [TestClass]
    public class GetAvailableJoSessions_UnitTest
    {

        // check that sold out JO sessions are not displayed
        [TestMethod]
        public void JoSession_ShouldBeDisplayed()
        {
            // create available Jo session
            List<JoSession> joSessionList =
            [
                new JoSession {
                    JoSessionName = "Test session (available)", JoSessionDate = new DateTime(2024, 8, 1, 11, 0, 0), JoSessionPlace = "Versailles, Château de Versailles", JoSessionNbTotalAttendees = 10000, JoSessionNbTotalBooked = 5000, JoSessionDescription = "", JoSessionImage = @"", JoSessionPrice = 50
                }
,
            ];
            IQueryable<JoSession> joSessionListQueryable = joSessionList.AsQueryable();


            //apply method to test
            IQueryable<JoSession> result = JoSessionsController.GetAvailableJoSessions(joSessionListQueryable);

            //check if test passes
            // use JavaScriptSerializer() to compare objects
            Assert.AreEqual(JsonSerializer.Serialize(joSessionListQueryable), JsonSerializer.Serialize(result), "the session " + joSessionList[0].JoSessionName + " should be in the result, but the program filter it out");
        }

        [TestMethod]
        public void JoSession_ShouldBeNotDisplayed()
        {
            // create UNavailable Jo session
            List<JoSession> joSessionList =
            [
                new JoSession {
                        JoSessionName = "Test session (UNavailable)", JoSessionDate = new DateTime(2024, 8, 1, 11, 0, 0), JoSessionPlace = "Versailles, Château de Versailles", JoSessionNbTotalAttendees = 10000, JoSessionNbTotalBooked = 10000, JoSessionDescription = "", JoSessionImage = @"", JoSessionPrice = 50
                    }
            ];
            IQueryable<JoSession> joSessionListQueryable = joSessionList.AsQueryable();


            //apply method to test
            IQueryable<JoSession> result = JoSessionsController.GetAvailableJoSessions(joSessionListQueryable);

            //check if test passes
            // use JavaScriptSerializer() to compare objects
            Assert.AreNotEqual(JsonSerializer.Serialize(joSessionListQueryable), JsonSerializer.Serialize(result), "the session " + joSessionList[0].JoSessionName + " should NOT be in the result, but the program keeps it");

        }
    }
}
