using GourmeJunk.Models.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IUsersService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();

        Task LockUserAsync(string userId);

        Task UnLockUserAsync(string userId);
    }
}
