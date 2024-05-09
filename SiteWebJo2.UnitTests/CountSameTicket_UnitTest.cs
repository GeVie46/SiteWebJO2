using SiteWebJO2.Models;
using SiteWebJO2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteWebJo2.UnitTests
{
    [TestClass]
    public class CountSameTicket_UnitTest
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                return new[]
                {
                    // test 1 : shopping cart empty
                    new object[] {
                        new JoTicketSimplified(2, 12),
                        new JoTicketSimplified[0],
                        0 },
                    // test 2 : 1 same ticket in shopping cart
                    new object[] {
                        new JoTicketSimplified(2, 12),
                        new JoTicketSimplified[]{new JoTicketSimplified(2, 12), },
                        1 },
                    // test 3 : 2 same tickets in shopping cart
                    new object[] {
                        new JoTicketSimplified(2, 12),
                        new JoTicketSimplified[]{new JoTicketSimplified(2, 12), new JoTicketSimplified(2, 12), new JoTicketSimplified(4, 32),new JoTicketSimplified(2, 32),new JoTicketSimplified(4, 12),},
                        2 },

                };
            }
        }

        // check tickets counter
        [TestMethod]
        [DynamicData(nameof(TestData))]
        public void Utilities_CountSameTicket(JoTicketSimplified ticket, JoTicketSimplified[] cart, int expected)
        {
            // Arrange
            // done in TestData initialization

            // Act
            int actual = Utilities.CountSameTicket(ticket, cart);

            // Assert
            Assert.AreEqual(expected, actual, "The method should return " + expected + " but return " + actual);
    }
}
}
