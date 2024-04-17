using Acc = TeamPhoenix.MusiCali.DataAccessLayer.ArtistCalendar;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            GigCreationModel gig = new GigCreationModel();
            string username = "bingoboardtests";
            string gigName = "second Gig";
            DateTime dateOfGig = new DateTime(2024, 4, 21, 11, 0, 0);
            bool visibility = true;
            string description = "second gig test";
            string location = "2121 fish street";
            string pay = "$2 dolars";

            //bool gigmake = Acc.CreateGig(username, gigName, dateOfGig, visibility, description, location, pay);
            //Assert.IsTrue(gigmake);
        }
    }
}