using System.Data;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services
{
    public class CollabSearchService
    {
        private readonly CollabSearchDataAccess _collabSearchDataAccess;

        public CollabSearchService(CollabSearchDataAccess collabSearchDataAccess)
        {
            _collabSearchDataAccess = collabSearchDataAccess;
        }

        public async Task<DataTable> SearchUsersAsync(string username)
        {
            return await _collabSearchDataAccess.SearchUsersAsync(username);
        }

        public async Task UpdateUserVisibilityAsync(string username, string visibility)
        {
            await _collabSearchDataAccess.UpdateUserVisibilityAsync(username, visibility);
        }
    }
}

