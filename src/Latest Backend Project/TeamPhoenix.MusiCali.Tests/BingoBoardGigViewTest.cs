using BB = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;


namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class BingoBoardGigViewTest
    {
        [TestMethod]
        public void GigLoadTest()
        {
            List<GigSummary>? gigs = BB.ViewGigSummary(20, "bingoboardtests");
            Console.WriteLine(gigs);
            Assert.IsNotNull(gigs);
        }
    }
}
