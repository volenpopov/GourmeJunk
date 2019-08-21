using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IUsersService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();

        Task<GourmeJunkUser> GetUserByIdAsync(string userId);

        Task LockUserAsync(string userId);

        Task UnLockUserAsync(string userId);

        Task<int> GetUserIndividualItemsCount(string email);

        Task<string> GetUserFirstLastNameByIdAsync(string userId);
    }
}
