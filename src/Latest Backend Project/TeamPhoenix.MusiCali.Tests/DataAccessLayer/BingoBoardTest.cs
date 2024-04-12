using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class BingoBoardTest
    {
        [TestMethod]
        public void RetrieveGigsTest()
        {
            ushort numberofgigs = 4;
            string username = "testuser";
            List<GigSummary> gigs;

            gigs = _dao.ViewGigSummary(numberofgigs, username);
            

            Assert.IsNotNull(gigs);

        }
    }
}
