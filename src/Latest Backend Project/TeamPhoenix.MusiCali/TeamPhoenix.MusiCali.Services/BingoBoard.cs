using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _dao = TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;

namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services
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
