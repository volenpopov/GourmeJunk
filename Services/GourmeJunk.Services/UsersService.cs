using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Common;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Users;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<GourmeJunkUser> usersRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;
        private readonly IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository;
        private readonly UserManager<GourmeJunkUser> userManager;

        public UsersService(
            IDeletableEntityRepository<GourmeJunkUser> usersRepository,
            IRepository<ShoppingCart> shoppingCartsRepository,
            IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository,
            UserManager<GourmeJunkUser> userManager)
        {
            this.usersRepository = usersRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
            this.shoppingCartMenuItemsRepository = shoppingCartMenuItemsRepository;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsersViewModels()
        {
            var usersViewModels = await this.usersRepository
                .AllAsNoTracking()                
                .To<UserViewModel>()
                .Where(usr => usr.Email != GlobalConstants.ADMINISTRATOR__EMAIL)
                .ToArrayAsync();

            foreach (var userModel in usersViewModels)
            {
                var user = await this.userManager.FindByIdAsync(userModel.Id);
                var roleName = this.userManager.GetRolesAsync(user).Result.FirstOrDefault();

                userModel.Authorization = roleName;
            }

            return usersViewModels;
        }

        public async Task LockUserAsync(string userId)
        {
            var user = await this.GetUserByIdAsync(userId);

            user.LockoutEnd = DateTime.UtcNow.AddYears(ServicesDataConstants.LOCKOUT_YEARS);

            await this.usersRepository.SaveChangesAsync();
        }

        public async Task UnLockUserAsync(string userId)
        {
            var user = await this.GetUserByIdAsync(userId);

            user.LockoutEnd = DateTime.UtcNow;

            await this.usersRepository.SaveChangesAsync();
        }

        public async Task<int> GetUserIndividualItemsCount(string email)
        {
            var individualItemsCount = 0;

            var userId = await this.GetUserIdByEmailAsync(email);

            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .SingleOrDefaultAsync(cart => cart.UserId == userId);

            if (shoppingCart != null)
            {
                individualItemsCount = this.shoppingCartMenuItemsRepository
                    .AllAsNoTracking()
                    .Where(shoppingCartMenuItem => shoppingCartMenuItem.ShoppingCartId == shoppingCart.Id)
                    .Count();
            }

            return individualItemsCount;
        }

        public async Task<string> GetUserFirstLastNameByIdAsync(string userId)
        {
            var user = await this.GetUserByIdAsync(userId);

            return $"{user.FirstName} {user.LastName}";
        }

        public async Task<GourmeJunkUser> GetUserByIdAsync(string userId)
        {
            var user = await this.usersRepository
                .All()
                .SingleOrDefaultAsync(usr => usr.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID,
                    ServicesDataConstants.USER,
                    userId));
            }

            return user;
        }

        private async Task<string> GetUserIdByEmailAsync(string email)
        {
            var user = await this.usersRepository
                .All()
                .SingleOrDefaultAsync(usr => usr.Email == email);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_USER_EMAIL, email));
            }

            return user.Id;
        }        
    }
}
