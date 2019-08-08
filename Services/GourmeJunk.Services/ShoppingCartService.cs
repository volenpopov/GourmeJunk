﻿using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GourmeJunk.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> shopingCartsRepository;
        private readonly IDeletableEntityRepository<MenuItem> menuItemsRepository;

        public ShoppingCartService(
            IRepository<ShoppingCart> shopingCartsRepository,
            IDeletableEntityRepository<MenuItem> menuItemsRepository)
        {
            this.shopingCartsRepository = shopingCartsRepository;
            this.menuItemsRepository = menuItemsRepository;
        }

        public async Task<ShoppingCartViewModel> GetShoppingCartViewModelAsync(string menuItemId)
        {
            var menuItemModel = await this.menuItemsRepository
                .All()
                .To<MenuItemViewModelExtended>()
                .SingleOrDefaultAsync(item => item.Id == menuItemId);

            if (menuItemModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), menuItemId));
            }

            var shoppingCartModel = new ShoppingCartViewModel
            {
                MenuItem = menuItemModel                
            };

            return shoppingCartModel;
        }
    }
}
