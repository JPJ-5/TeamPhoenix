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
            string gigName = ":)";
            DateTime dateOfGig = new DateTime(2024, 4, 19, 18, 0, 7);
            bool visibility = true;
            string description = ":)";
            string location = "(:";
            string pay = ":)";

            //bool gigmake = Acc.CreateGig(username, gigName, dateOfGig, visibility, description, location, pay);
            //Assert.IsTrue(gigmake);
        }
    }
}