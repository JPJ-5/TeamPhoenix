using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;

namespace TeamPhoenix.MusiCali.Services
{
    public class BingoBoard
    {
        public static List<GigSummary>? ViewMultGigSummary(ushort numberOfGigs, string username)
        {
            try
            {
                return _dao.ViewGigSummary(numberOfGigs, username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving gig information: {ex.Message}");
                throw;
            }

            return null;
        }
    }
}
