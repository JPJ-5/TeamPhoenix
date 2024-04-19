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

        public static bool IsUserInterested(string username, int gigID)
        {
            string level;
            string category;
            string context;
            string userHash;
            try
            {
                List<string>? interestedUsers = _dao.UserInterestList(username, gigID);
                userHash = rU.GetUserHash(username);
                level = "Info";
                category = "View";
                context = "User Interest list Successfully Checked";
                _loggerCreation.CreateLog(userHash, level, category, context);
                if (interestedUsers != null && interestedUsers.Count > 0)
                {
                    return interestedUsers.Contains(username);
                }
                return false;
            }
            catch(Exception ex)
            {
                userHash = rU.GetUserHash(username);
                level = "Info";
                category = "View";
                context = "Error retrieving user interest list";
                _loggerCreation.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error retrieving username from interest table: {ex.Message}");
                throw;
            }
        }

        public static BingoBoardInterestMessage addUserInterest(string username, int gigID)
        {
            string level;
            string category;
            string context;
            string userHash;
            try
            {
                bool isUserInterested = IsUserInterested( username, gigID);
                if(isUserInterested)
                {
                    return new BingoBoardInterestMessage("User already interested", false);
                }
                bool putUserInGig = _dao.IndicateInterest( username, gigID );
                if(putUserInGig)
                {
                    BingoBoardInterestMessage bbIntMsg = new("User successfully interested", putUserInGig);
                    return bbIntMsg;
                }

                BingoBoardInterestMessage bbIntMsgErr = new("Database Error", putUserInGig);
                return bbIntMsgErr;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user to interest table: {ex.Message}");

                throw;
            }
        }
    }
}