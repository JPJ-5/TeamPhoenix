using BB = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;


namespace TeamPhoenix.MusiCali.Tests.BingoBoardTests
{
    [TestClass]
    public class BingoBoardGigViewTest
    {
        [TestMethod]
        public void GigLoadTest()
        {
            GigSet? gigs = BB.ViewGigSummary(20, "bingoboardtests", 0);
            //Console.WriteLine(gigs[0].Username);
            Assert.IsNotNull(gigs);
        }

        [TestMethod]
        public void GigNumTest()
        {
            int numOfGigs = BB.ReturnNumOfGigs();
            Assert.IsNotNull(numOfGigs);
        }
    }
}
