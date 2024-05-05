using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ArtistCalendarDAO
    {
        private readonly IConfiguration configuration;
        private readonly SqlArtistCalendar artistCalendarAccess;

        public ArtistCalendarDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.artistCalendarAccess = new SqlArtistCalendar(this.configuration);
        }

        public bool IsGigDateExist(string username, DateTime dateOfGig)
        {
            return artistCalendarAccess.IsGigDateExist("Gig", username, dateOfGig); //looks at Gig table inside database.
        }
        public bool CreateGig(string username, string gigName, DateTime dateOfGig, bool visibility, string description, string location, string pay)
        {
            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigName", "@GigDateTime", "@GigVisibility", "@Description", "@Location", "@Pay"];
                List<object> parametersValue = [username, gigName, dateOfGig, visibility, description, location, pay];
                string sql = "INSERT INTO Gig (PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay) VALUES (@PosterUsername, @GigName, @GigDateTime, @GigVisibility, @Description, @Location, @Pay)"; // There a way to improve this I think.
                bool CreateGigResult = artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);
                return CreateGigResult;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteGig(string username, DateTime dateOfGig)
        {
            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigDateTime"];
                List<object> parametersValue = [username, dateOfGig];
                string sql = "DELETE FROM Gig WHERE PosterUsername = @PosterUsername AND GigDateTime = @GigDateTime"; // There a way to improve this I think.
                bool deleteGigResult = artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);
                return deleteGigResult;
            }
            catch
            {
                return false;
            }
        }
        public GigView? ViewGig(string username, string usernameOfOwner, DateTime dateOfGig)
        {
            GigView? viewGigResult = null;
            try
            {
                List<string> parametersCategory = ["@GigUsername", "@GigDateTime"];
                List<object> parametersValue = [usernameOfOwner, dateOfGig];
                string sql = "SELECT PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay FROM Gig WHERE PosterUsername = @GigUsername AND GigDateTime = @GigDateTime";
                viewGigResult = artistCalendarAccess.readGigSQL(username, sql, parametersCategory, parametersValue);
                return viewGigResult;
            }
            catch
            {
                viewGigResult = null;
            }
            return viewGigResult;
        }
        public bool EditGig(DateTime oldDateOfGig, string username, string gigName, DateTime newDateOfGig, bool visibility, string description, string location, string pay)
        {
            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigName", "@GigDateTime", "@GigVisibility", "@Description", "@Location", "@Pay", "@OldDateTime"];
                List<object> parametersValue = [username, gigName, newDateOfGig, visibility, description, location, pay, oldDateOfGig];
                string sql = "UPDATE Gig SET GigName = @GigName, GigDateTime = @GigDateTime, GigVisibility = @GigVisibility, Description = @Description, Location = @Location, Pay = @Pay WHERE PosterUsername = @PosterUsername AND GigDateTime = @OldDateTime";
                bool EditGigResult = artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);
                return EditGigResult;
            }
            catch
            {
                return false;
            }
        }
        public bool ChangeGigVisibility(string username, bool visibility)
        {
            try
            {
                List<string> parametersCategory = ["@PosterUsername", "@GigVisibility"];
                List<object> parametersValue = [username, visibility];
                string sql = "UPDATE Gig SET GigVisibility = @GigVisibility WHERE PosterUsername = @PosterUsername";
                bool GigVisibilityResult = artistCalendarAccess.executeSQL(sql, parametersCategory, parametersValue);
                return GigVisibilityResult;
            }
            catch
            {
                return false;
            }
        }

        public string GetUserHash(string username)
        {
            return artistCalendarAccess.GetUserHash(username);
        }
    }
}
