using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.BingoBoard;
using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;
using rU = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;

namespace TeamPhoenix.MusiCali.Services
{
    public class BingoBoard
    {
        public static GigSet? ViewMultGigSummary(int numberOfGigs, string username, int offset = 0)
        {
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                GigSet? gigs = _dao.ViewGigSummary(numberOfGigs, username, offset);

                if (gigs!.GigSummaries!.Count == 0)
                {
                    userHash = rU.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = "Failed to retrieve gigs";
                    _loggerCreation.CreateLog(userHash, level, category, context);
                    return null;
                }
                if (gigs.GigSummaries.Count == numberOfGigs)
                {
                    userHash = rU.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = $"{numberOfGigs} gigs successfully retrieved from database";
                    _loggerCreation.CreateLog(userHash, level, category, context);
                }
                else
                {
                    userHash = rU.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = $"{gigs.GigSummaries.Count} gigs successfully retrieved from database, but {numberOfGigs} were requested";
                    _loggerCreation.CreateLog(userHash, level, category, context);
                }

                return gigs;
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