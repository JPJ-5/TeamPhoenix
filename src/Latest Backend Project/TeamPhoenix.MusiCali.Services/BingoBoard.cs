using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;

namespace TeamPhoenix.MusiCali.Services
{
    public class BingoBoard
    {
        public static GigSet? ViewMultGigSummary(int numberOfGigs, string username, int offset = 0)
        {
            try
            {
                return _dao.ViewGigSummary(numberOfGigs, username, offset);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving gig information: {ex.Message}");
                throw;
            }
        }

        public static int ReturnGigNum()
        {
            try
            {
                return _dao.ReturnNumOfGigs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving gig table size: {ex.Message}");
                throw;
            }
        }
    }
}