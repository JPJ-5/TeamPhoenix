using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ArtistCalendarDAL
    {
        private readonly SqlArtistCalendar artistCalendarAccess = new SqlArtistCalendar();

        public bool IsGigDateExist(string username, DateTime dateOfGig)
        {
            return artistCalendarAccess.IsGigDateExist("Gig", username, dateOfGig); //looks at Gig table inside database.
        }
        public bool CreateGig(string username, string gigName, DateTime dateOfGig, bool visibility, string description, string location, string pay)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigName", "@GigDateTime", "@GigVisibility", "@Description", "@Location", "@Pay"];
                List<object> parametersValue = [username, gigName, dateOfGig, visibility, description, location, pay];
                string sql = "INSERT INTO Gig (PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay) VALUES (@PosterUsername, @GigName, @GigDateTime, @GigVisibility, @Description, @Location, @Pay)"; // There a way to improve this I think.
                artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);

                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was successfully added";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
                return true;
            }
            catch
            {
                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was unsuccessfully added";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
            }

            return false;
        }

        public bool DeleteGig(string username, DateTime dateOfGig)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigDateTime"];
                List<object> parametersValue = [username, dateOfGig];
                string sql = "DELETE FROM Gig WHERE PosterUsername = @PosterUsername AND GigDateTime = @GigDateTime"; // There a way to improve this I think.
                artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);

                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was successfully deleted";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
                return true;
            }
            catch
            {
                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was unsuccessfully deleted";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
            }

            return false;
        }
        public GigView? ViewGig(string username, string usernameOfOwner, DateTime dateOfGig)
        {
            string level;
            string category;
            string context;
            string userHash;

            GigView? viewGigResult = null;
            try
            {
                List<string> parametersCategory = ["@GigUsername", "@GigDateTime"];
                List<object> parametersValue = [usernameOfOwner, dateOfGig];
                string sql = "SELECT PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay FROM Gig WHERE PosterUsername = @GigUsername AND GigDateTime = @GigDateTime";
                viewGigResult = artistCalendarAccess.readGigSQL(username, sql, parametersCategory, parametersValue);

                // maybe SqlArtistCalendar class should have this bit to create a log since there's both success and failed gigs.
                level = "Info";
                category = "View";
                context = "Gig was successfully viewed";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
                return viewGigResult;
            }
            catch
            {
                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was unsuccessfully viewed";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
            }
            return viewGigResult;
        }
        public bool EditGig(DateTime oldDateOfGig, string username, string gigName, DateTime newDateOfGig, bool visibility, string description, string location, string pay)
        {
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigName", "@GigDateTime", "@GigVisibility", "@Description", "@Location", "@Pay", "@OldDateTime"];
                List<object> parametersValue = [username, gigName, newDateOfGig, visibility, description, location, pay, oldDateOfGig];
                string sql = "UPDATE Gig SET GigName = @GigName, GigDateTime = @GigDateTime, GigVisibility = @GigVisibility, Description = @Description, Location = @Location, Pay = @Pay WHERE PosterUsername = @PosterUsername AND GigDateTime = @OldDateTime";
                artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);

                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was successfully edited";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
                return true;
            }
            catch
            {
                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gig was unsuccessfully edited";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
            }

            return false;
        }
        public bool ChangeGigVisibility(string username, bool visibility)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigVisibility"];
                List<object> parametersValue = [username, visibility];
                string sql = "UPDATE Gig SET GigVisibility = @GigVisibility WHERE PosterUsername = @PosterUsername";
                artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);

                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gigs' visibility was successfully changed";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
                return true;
            }
            catch
            {
                // maybe SqlArtistCalendar class should have this bit to create a log since I think this might be repeated.
                level = "Info";
                category = "View";
                context = "Gigs' visibility was unsuccessfully changed";
                userHash = artistCalendarAccess.GetUserHash(username);
                //loggerCreation.CreateLog(userHash, level, category, context);
            }

            return false;
        }
    }
}
