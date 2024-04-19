using BB = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;
using BBS = TeamPhoenix.MusiCali.Services.BingoBoard;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.BingoBoardTests
{
    [TestClass]
    public class BingoBoardInterestViewTest
    {
        [TestMethod]
        public void indicateGigInterestTest()
        {
            bool didInsert = BB.IndicateInterest("bingoboardtests", 38);
            //Console.WriteLine(gigs[0].Username);
            Assert.IsTrue(didInsert);
        }

        [TestMethod]
        public void isUserAlreadyInterestedTest()
        {
            bool isInserted = BBS.IsUserInterested("bingoboardtests", 38);
            //Console.WriteLine(gigs[0].Username);
            Assert.IsTrue(isInserted);
        }
    }
}
