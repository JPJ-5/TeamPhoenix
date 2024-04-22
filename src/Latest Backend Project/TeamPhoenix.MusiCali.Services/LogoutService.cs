using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Services
{
    public class LogoutService
    {
        private LogOutDAO logOutDAO;
        private readonly IConfiguration configuration;

        public LogoutService(IConfiguration configuration)
        {
            this.configuration = configuration;
            logOutDAO = new LogOutDAO(configuration);
        }

        public async Task<bool> LogoutUserAsync(string userHash)
        {
            return await logOutDAO.LogUserLogoutAsync(userHash);
        }
    }
}
