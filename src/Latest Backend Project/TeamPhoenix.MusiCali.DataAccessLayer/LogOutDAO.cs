using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class LogOutDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        private RecoverUserDAO recoverUserDAO;
        private MariaDBDAO mariaDBDAO;
        public LogOutDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
            recoverUserDAO = new RecoverUserDAO(this.configuration);
            mariaDBDAO = new MariaDBDAO(this.configuration);
        }
        public async Task<bool> LogUserLogoutAsync(string userName)
        {
            try
            {
                string userInfo = recoverUserDAO.GetUserHash(userName);
                string level = "Info";
                string category = "View";
                string context = "User has Logout";

                // Directly use the logging system to log the user's logout action
                await Task.Run(() => mariaDBDAO.CreateLog(userInfo, level, category, context));
                return true;
            }
            catch (SqlException ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An SQL error occurred: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                return false; // Return false if logging failed
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Return false if logging failed
            }
        }
    }
}