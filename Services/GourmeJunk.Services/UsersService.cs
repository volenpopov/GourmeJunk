﻿using System;
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
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<GourmeJunkUser> usersRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;
        private readonly IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository;

        public UsersService(
            IDeletableEntityRepository<GourmeJunkUser> usersRepository,
            IRepository<ShoppingCart> shoppingCartsRepository,
            IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository)
        {
            this.usersRepository = usersRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
            this.shoppingCartMenuItemsRepository = shoppingCartMenuItemsRepository;
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
                    .All()
                    .Where(shoppingCartMenuItem => shoppingCartMenuItem.ShoppingCartId == shoppingCart.Id)
                    .Count();
            }

            return individualItemsCount;
        }

        public async Task<GourmeJunkUser> GetUserByIdAsync(string userId)
        {
            var user = await this.usersRepository
                .All()
                .SingleOrDefaultAsync(usr => usr.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID,
                    nameof(ServicesDataConstants.USER),
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