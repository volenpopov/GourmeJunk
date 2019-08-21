using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Models.ViewModels.ShoppingCarts;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;
        private readonly IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository;
        private readonly IDeletableEntityRepository<MenuItem> menuItemsRepository;
        private readonly IDeletableEntityRepository<Coupon> couponsRepository;

        public ShoppingCartService(
            IRepository<ShoppingCart> shopingCartsRepository,
            IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsRepository,
            IDeletableEntityRepository<MenuItem> menuItemsRepository,
            IDeletableEntityRepository<Coupon> couponsRepository)
        {
            this.shoppingCartsRepository = shopingCartsRepository;
            this.shoppingCartMenuItemsRepository = shoppingCartMenuItemsRepository;
            this.menuItemsRepository = menuItemsRepository;
            this.couponsRepository = couponsRepository;
        }

        public async Task<ShoppingCartMenuItemDetailsViewModel> GetShoppingCartMenuItemDetailsViewModelAsync(string menuItemId)
        {
            var menuItemModel = await this.menuItemsRepository
                .All()
                .To<MenuItemViewModelExtended>()
                .SingleOrDefaultAsync(item => item.Id == menuItemId);

            if (menuItemModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), menuItemId));
            }

            var shoppingCartModel = new ShoppingCartMenuItemDetailsViewModel
            {
                MenuItem = menuItemModel                
            };

            return shoppingCartModel;
        }

        public async Task UpdateShoppingCartAsync(string menuItemId, int itemCount, ClaimsIdentity claimsIdentity)
        {
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCart = await this.GetShoppingCartByUserId(userId);

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

        public async Task<ShoppingCartViewModel> GetShoppingCartViewModelAsync(string userId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            var orderTotal = 0m;
            var menuItemsModels = new List<ShoppingCartMenuItemViewModel>();            

            foreach (var item in shoppingCart.ShoppingCartMenuItems)
            {
                var menuItemModel = new ShoppingCartMenuItemViewModel
                {
                    Id = item.MenuItem.Id,
                    Name = item.MenuItem.Name,                    
                    Price = item.MenuItem.Price,
                    Image = item.MenuItem.Image,
                    Count = item.Count
                };

                if (item.MenuItem.Description != null)
                {
                    if (item.MenuItem.Description.Length > ServicesDataConstants.CART_INDEX_MENUITEM_DESCRIPTION_MAX_CHARS)
                    {
                        menuItemModel.Description = item.MenuItem.Description
                            .Substring(0, ServicesDataConstants.CART_INDEX_MENUITEM_DESCRIPTION_MAX_CHARS - 1) + "...";
                    }
                    else
                    {
                        menuItemModel.Description = item.MenuItem.Description;
                    }
                }

                orderTotal += item.MenuItem.Price * item.Count;

                menuItemsModels.Add(menuItemModel);
            }

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                UserId = userId,
                OrderTotalOriginal = orderTotal,
                MenuItems = menuItemsModels
            };

            return shoppingCartViewModel;
        }

        public async Task<ShoppingCartViewModel> ApplyCouponToCartAsync(ShoppingCartViewModel model)
        {
            var coupon = await this.couponsRepository
                .All()
                .SingleOrDefaultAsync(cpn => cpn.Name == model.CouponName);

            if (coupon == null)
            {
                model.StatusMessage = string.Format(ServicesDataConstants.COUPON_NOT_FOUND_ERROR, model.CouponName);
                return model;
            }

            var discountedOrderTotal = 0m;

            var isPercentageType = (int) coupon.CouponType == 0;
            
            if (isPercentageType)
            {
                discountedOrderTotal = model.OrderTotalOriginal - ((model.OrderTotalOriginal * coupon.Discount)/100);
            }
            else
            {
                discountedOrderTotal = model.OrderTotalOriginal - coupon.Discount;
            }

            model.OrderTotal = discountedOrderTotal;

            return model;
        }

        public async Task IncrementItemQuantityAsync(string userId, string menuItemId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            shoppingCart.ShoppingCartMenuItems
                .SingleOrDefault(cartItem => cartItem.MenuItemId == menuItemId && cartItem.ShoppingCartId == shoppingCart.Id)
                .Count++;

            await this.shoppingCartMenuItemsRepository.SaveChangesAsync();
        }

        public async Task DecrementItemQuantityAsync(string userId, string menuItemId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            var shoppingCartMenuItem = await this.shoppingCartMenuItemsRepository
                .All()
                .SingleOrDefaultAsync(cartItem => cartItem.MenuItemId == menuItemId && cartItem.ShoppingCartId == shoppingCart.Id);
           
            shoppingCartMenuItem.Count--;

            await this.shoppingCartMenuItemsRepository.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string userId, string menuItemId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            var shoppingCartMenuItem = await this.shoppingCartMenuItemsRepository
                .All()
                .SingleOrDefaultAsync(cartItem => cartItem.MenuItemId == menuItemId && cartItem.ShoppingCartId == shoppingCart.Id);

            shoppingCart.ShoppingCartMenuItems.Remove(shoppingCartMenuItem);            

            await this.shoppingCartMenuItemsRepository.SaveChangesAsync();
        }

        public async Task<int> GetUserIndividualItemsCountAsync(string userId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            var userIndividualItemsCount = this.shoppingCartMenuItemsRepository
                .AllAsNoTracking()
                .Where(shoppingCartMenuItem => shoppingCartMenuItem.ShoppingCartId == shoppingCart.Id)
                .Count();

            return userIndividualItemsCount;
        }

        public async Task<bool> CheckIfItemIsLastAndIsToBeRemovedAsync(string userId, string menuItemId)
        {
            var shoppingCart = await this.GetShoppingCartByUserId(userId);

            var shoppingCartMenuItem = await this.shoppingCartMenuItemsRepository
                .All()
                .SingleOrDefaultAsync(cartItem => cartItem.MenuItemId == menuItemId && cartItem.ShoppingCartId == shoppingCart.Id);

            return shoppingCartMenuItem.Count == 1;
        }

        private async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Include(cart => cart.ShoppingCartMenuItems)
                .ThenInclude(cartItem => cartItem.MenuItem)
                .SingleOrDefaultAsync(cart => cart.UserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId };

                await this.shoppingCartsRepository.AddAsync(shoppingCart);
            }

            return shoppingCart;
        }
    }
}
