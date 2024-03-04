using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Services
{
    public class LogoutService
    {
        private readonly LogoutRepository _logoutRepository;

        public LogoutService(LogoutRepository logoutRepository)
        {
            _logoutRepository = logoutRepository;
        }

        public async Task<bool> LogoutUserAsync(string userHash)
        {
            return await _logoutRepository.LogUserLogoutAsync(userHash);
        }
    }
}
