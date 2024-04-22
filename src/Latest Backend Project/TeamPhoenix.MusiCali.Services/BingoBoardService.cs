using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Logging;

namespace TeamPhoenix.MusiCali.Services
{
    public class BingoBoardService
    {
        private readonly IConfiguration configuration;
        private BingoBoardDAO bingoBoardDAO;
        private RecoverUserDAO recoverUserDAO;
        private LoggerService loggerService;
        public BingoBoardService(IConfiguration configuration)
        {
            this.configuration = configuration;
            bingoBoardDAO = new BingoBoardDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            loggerService = new LoggerService(configuration);
        }

        public GigSet? ViewMultGigSummary(int numberOfGigs, string username, int offset = 0)
        {
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                GigSet? gigs = bingoBoardDAO.ViewGigSummary(numberOfGigs, username, offset);
                int gigSummarySize = gigs!.GigSummaries!.Count;

                if (gigSummarySize == 0)
                {
                    userHash = recoverUserDAO.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = "Failed to retrieve gigs";
                    loggerService.CreateLog(userHash, level, category, context);
                    return null;
                }

                    if (gigs.GigSummaries.Count == numberOfGigs)
                {
                    userHash = recoverUserDAO.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = $"{numberOfGigs} gigs successfully retrieved from database";
                    loggerService.CreateLog(userHash, level, category, context);
                }
                else
                {
                    userHash = recoverUserDAO.GetUserHash(username);
                    level = "Info";
                    category = "View";
                    context = $"{gigs.GigSummaries.Count} gigs successfully retrieved from database, but {numberOfGigs} were requested";
                    loggerService.CreateLog(userHash, level, category, context);
                }

                return gigs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving gig information: {ex.Message}");
                throw;
            }
        }

        public int ReturnGigNum()
        {
            try
            {
                return bingoBoardDAO.ReturnNumOfGigs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving gig table size: {ex.Message}");
                throw;
            }
        }

        public bool IsUserInterested(string username, int gigID)
        {
            string level;
            string category;
            string context;
            string userHash;
            try
            {
                List<string>? interestedUsers = bingoBoardDAO.UserInterestList(username, gigID);
                userHash = recoverUserDAO.GetUserHash(username);
                level = "Info";
                category = "View";
                context = "User Interest list Successfully Checked";
                loggerService.CreateLog(userHash, level, category, context);
                if (interestedUsers != null && interestedUsers.Count > 0)
                {
                    return interestedUsers.Contains(username);
                }
                return false;
            }
            catch(Exception ex)
            {
                userHash = recoverUserDAO.GetUserHash(username);
                level = "Info";
                category = "View";
                context = "Error retrieving user interest list";
                loggerService.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error retrieving username from interest table: {ex.Message}");
                throw;
            }
        }

        public BingoBoardInterestMessage addUserInterest(string username, int gigID)
        {
            string level;
            string category;
            string context;
            string userHash = recoverUserDAO.GetUserHash(username);
            try
            {
                bool isUserInterested = IsUserInterested( username, gigID);
                if(isUserInterested)
                {
                    level = "Info";
                    category = "View";
                    context = "User attempted to add to interest list while already present";
                    loggerService.CreateLog(userHash, level, category, context);
                    return new BingoBoardInterestMessage("User already interested", false);
                }
                bool putUserInGig = bingoBoardDAO.IndicateInterest( username, gigID );
                if(putUserInGig)
                {
                    BingoBoardInterestMessage bbIntMsg = new("User successfully interested", true);
                    level = "Info";
                    category = "View";
                    context = "User successfully added to interest list";
                    loggerService.CreateLog(userHash, level, category, context);
                    return bbIntMsg;
                }

                BingoBoardInterestMessage bbIntMsgErr = new("Database Error", false);
                level = "Info";
                category = "View";
                context = "Unknown database error";
                loggerService.CreateLog(userHash, level, category, context);
                return bbIntMsgErr;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user to interest table: {ex.Message}");
                level = "Info";
                category = "View";
                context = "Unknown database error while registering to user interest table";
                loggerService.CreateLog(userHash, level, category, context);
                throw;
            }
        }
    }
}