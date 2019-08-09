using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;
        private readonly IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository;
        private readonly IDeletableEntityRepository<MenuItem> menuItemsRepository;

        public ShoppingCartService(
            IRepository<ShoppingCart> shopingCartsRepository,
            IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository,
            IDeletableEntityRepository<MenuItem> menuItemsRepository)
        {
            this.shoppingCartsRepository = shopingCartsRepository;
            this.shoppingCartMenuItemsRepository = shoppingCartMenuItemsRepository;
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

        public async Task UpdateShoppingCartAsync(string menuItemId, int itemCount, ClaimsIdentity user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .SingleOrDefaultAsync(cart => cart.UserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId };

                await this.shoppingCartsRepository.AddAsync(shoppingCart);
            }

            var shoppingCartMenuItem = await this.shoppingCartMenuItemsRepository
                .All()
                .SingleOrDefaultAsync(cartItem => cartItem.ShoppingCartId == shoppingCart.Id 
                                      && cartItem.MenuItemId == menuItemId);

            if (shoppingCartMenuItem == null)
            {
                shoppingCartMenuItem = new ShoppingCartMenuItems
                {
                    ShoppingCart = shoppingCart,
                    MenuItemId = menuItemId,
                    Count = itemCount
                };

                await this.shoppingCartMenuItemsRepository.AddAsync(shoppingCartMenuItem);
            }
            else
            {
                shoppingCartMenuItem.Count += itemCount;
            }

            await this.shoppingCartMenuItemsRepository.SaveChangesAsync();
        }
    }
}
