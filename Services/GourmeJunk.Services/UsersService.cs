using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Common;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Users;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<GourmeJunkUser> usersRepository;

        public UsersService(IDeletableEntityRepository<GourmeJunkUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var usersViewModels = await this.usersRepository
                .AllAsNoTracking()                
                .To<UserViewModel>()
                .Where(usr => usr.Email != GlobalConstants.ADMINISTRATOR__EMAIL)
                .ToArrayAsync();

            return usersViewModels;
        }
    }
}
