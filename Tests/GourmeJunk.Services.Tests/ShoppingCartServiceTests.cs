using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Models.ViewModels.ShoppingCarts;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class ShoppingCartServiceTests : BaseServiceTests
    {
        private IShoppingCartService shoppingCartServiceMock
            => this.ServiceProvider.GetRequiredService<IShoppingCartService>();

        private const string TEST_MENUITEM_NAME = "Lasagna";
        private const decimal TEST_MENUITEM_PRICE = 10m;

        private const string TEST_IMAGE_URL = @"images\MenuItems\lasagna.jpg";

        private const string SECOND_TEST_MENUITEM_NAME = "Spaghetti";
        private const decimal SECOND_TEST_MENUITEM_PRICE = 7m;

        private const string TEST_CATEGORY_NAME = "Main";
        private const string TEST_SUBCATEGORY_NAME = "TestSubcategory";

        private readonly string TEST_USER_ID = Guid.NewGuid().ToString();
        private const string TEST_USER_FIRSTNAME = "Pesho";
        private const string TEST_USER_LASTNAME = "Peshov";

        private readonly string SECOND_TEST_USER_ID = Guid.NewGuid().ToString();
        private const string SECOND_TEST_USER_FIRSTNAME = "Kiro";
        private const string SECOND_TEST_USER_LASTNAME = "Kirov";

        private const int MENUITEM_DETAILS_STARTING_COUNT = 1;
        private const int TEST_USER_TEST_MENUITEM_COUNT = 2;
        private const int TEST_USER_SECOND_TEST_MENUITEM_COUNT = 1;

        private const string TEST_COUPON_NAME = "BIGDEAL";
        private const CouponType TEST_COUPON_TYPE = CouponType.Percent;
        private const int TEST_COUPON_DISCOUNT = 10;
        private const decimal TEST_COUPON_MIN_ORDER_AMOUNT = 10m;
        private const bool TEST_COUPON_ISACTIVE = true;

        private const string SECOND_TEST_COUPON_NAME = "3OFF";
        private const CouponType SECOND_TEST_COUPON_TYPE = CouponType.Amount;
        private const int SECOND_TEST_COUPON_DISCOUNT = 3;
        private const bool SECOND_TEST_COUPON_ISACTIVE = true;

        private const string THIRD_TEST_COUPON_NAME = "InactivePromo";
        private const CouponType THIRD_TEST_COUPON_TYPE = CouponType.Amount;
        private const int THIRD_TEST_COUPON_DISCOUNT = 100;
        private const bool THIRD_TEST_COUPON_ISACTIVE = false;

        [Fact]
        public async Task GetShoppingCartMenuItemDetailsViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();

            var actual = await this.shoppingCartServiceMock.GetShoppingCartMenuItemDetailsViewModelAsync(menuItem.Id);

            Assert.IsType<ShoppingCartMenuItemDetailsViewModel>(actual);
            Assert.IsType<MenuItemViewModelExtended>(actual.MenuItem);

            Assert.Equal(menuItem.Id, actual.MenuItem.Id);
            Assert.Equal(menuItem.Name, actual.MenuItem.Name);
            Assert.Equal(menuItem.Price, actual.MenuItem.Price);
            Assert.Equal(menuItem.Image, actual.MenuItem.Image);
            Assert.Equal(menuItem.Description, actual.MenuItem.Description);
            Assert.Equal(menuItem.CategoryId, actual.MenuItem.CategoryId);
            Assert.Equal(TEST_CATEGORY_NAME, actual.MenuItem.CategoryName);
            Assert.Equal(menuItem.SubCategoryId, actual.MenuItem.SubCategoryId);
            Assert.Equal(TEST_SUBCATEGORY_NAME, actual.MenuItem.SubCategoryName);
            Assert.Equal(MENUITEM_DETAILS_STARTING_COUNT, actual.Count);

        }

        [Fact]
        public async Task GetShoppingCartMenuItemDetailsViewModelAsync_ThrowsNullReferenceIfItemNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.shoppingCartServiceMock.GetShoppingCartMenuItemDetailsViewModelAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), string.Empty), exception.Message);
        }

        [Fact]
        public async Task UpdateShoppingCartAsync_InitializesACartIfUserDoesntHaveOne()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUserWithoutShoppingCart();

            //Lasagna
            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var itemCount = new Random().Next(1, 10);

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                });

            await this.shoppingCartServiceMock.UpdateShoppingCartAsync(menuItem.Id, itemCount, claimsIdentity);

            var newlyCreatedCart = await this.DbContext.ShoppingCarts.LastAsync();

            Assert.NotEmpty(this.DbContext.ShoppingCarts);
            Assert.NotNull(newlyCreatedCart);
            Assert.NotNull(newlyCreatedCart.UserId);
            Assert.Equal(user.Id, newlyCreatedCart.UserId);
        }

        [Fact]
        public async Task UpdateShoppingCartAsync_InitializesNewShoppingCartMenuItemWhenThisIsTheFirstSuchItemToBeAddedToCart()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();

            //Lasagna
            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var itemCount = new Random().Next(1, 10);

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                });

            var shoppingCartMenuItems = this.DbContext.ShoppingCartMenuItems;

            Assert.Empty(shoppingCartMenuItems);

            await this.shoppingCartServiceMock.UpdateShoppingCartAsync(menuItem.Id, itemCount, claimsIdentity);

            Assert.NotEmpty(shoppingCartMenuItems);
            Assert.Equal(itemCount, shoppingCartMenuItems.First().Count);
            Assert.Equal(menuItem.Id, shoppingCartMenuItems.First().MenuItemId);
            Assert.Equal(user.ShoppingCartId, shoppingCartMenuItems.First().ShoppingCartId);
        }

        [Fact]
        public async Task UpdateShoppingCartAsync_IncreasesTheCountOfAnALreadyExistingMenuItemInCartWhenAddedSameItemAgainToCart()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            //Lasagna
            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var itemCount = new Random().Next(1, 10);

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                });

            //Pesho-Lasagna
            var shoppingCartMenuItem = await this.DbContext.ShoppingCartMenuItems.LastAsync();

            Assert.Equal(TEST_USER_TEST_MENUITEM_COUNT, shoppingCartMenuItem.Count);

            await this.shoppingCartServiceMock.UpdateShoppingCartAsync(menuItem.Id, itemCount, claimsIdentity);

            var expectedCount = TEST_USER_TEST_MENUITEM_COUNT + itemCount;

            Assert.Equal(expectedCount, shoppingCartMenuItem.Count);
        }

        [Fact]
        public async Task GetShoppingCartViewModelAsync_InitializesACartIfUserDoesntHaveOne()
        {
            await this.AddTestingUserWithoutShoppingCart();

            var user = await this.DbContext.Users.FirstAsync();

            var shoppingCarts = this.DbContext.ShoppingCarts;

            Assert.Empty(shoppingCarts);

            await this.shoppingCartServiceMock.GetShoppingCartViewModelAsync(user.Id);
            await this.DbContext.SaveChangesAsync();

            Assert.NotEmpty(shoppingCarts);
            Assert.Equal(user.Id, shoppingCarts.First().UserId);
        }

        [Fact]
        public async Task GetShoppingCartViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();
            var userCart = await this.DbContext.ShoppingCarts.FirstAsync();
            var userCartItems = await this.DbContext.ShoppingCartMenuItems
                .OrderBy(item => item.MenuItem.Name)
                .ToArrayAsync();

            var actual = await this.shoppingCartServiceMock.GetShoppingCartViewModelAsync(user.Id);

            var expectedOrderTotal = 0m;

            foreach (var cartItem in userCartItems)
            {
                expectedOrderTotal += cartItem.Count * cartItem.MenuItem.Price;
            }

            Assert.Equal(user.Id, actual.UserId);
            Assert.Equal(expectedOrderTotal, actual.OrderTotalOriginal);

            Assert.Collection(actual.MenuItems,
                elem1 =>
                {
                    var expectedDescription = GetModifiedDescription(userCartItems);

                    Assert.Equal(userCartItems.First().MenuItem.Id, elem1.Id);
                    Assert.Equal(userCartItems.First().MenuItem.Name, elem1.Name);
                    Assert.Equal(userCartItems.First().MenuItem.Price, elem1.Price);
                    Assert.Equal(userCartItems.First().MenuItem.Image, elem1.Image);
                    Assert.Equal(expectedDescription, elem1.Description);
                    Assert.Equal(userCartItems.First().Count, elem1.Count);
                },
                elem2 =>
                {
                    var expectedDescription = GetModifiedDescription(userCartItems);

                    Assert.Equal(userCartItems.Last().MenuItem.Id, elem2.Id);
                    Assert.Equal(userCartItems.Last().MenuItem.Name, elem2.Name);
                    Assert.Equal(userCartItems.Last().MenuItem.Price, elem2.Price);
                    Assert.Equal(userCartItems.Last().MenuItem.Image, elem2.Image);
                    Assert.Equal(expectedDescription, elem2.Description);
                    Assert.Equal(userCartItems.Last().Count, elem2.Count);
                });
        }

        [Fact]
        public async Task ApplyCouponToCartAsync_ReturnsNotEnoughOrderTotalStatusMessageWhenOrderTotalIsLessThanTheMinCouponAmount()
        {           
            await this.AddTestingCouponsToDb();

            var coupon = await this.DbContext.Coupons.FirstAsync();

            var cartViewModel = new ShoppingCartViewModel
            {
                CouponName = coupon.Name,
                OrderTotalOriginal = default(decimal)
            };

            var actual = await this.shoppingCartServiceMock.ApplyCouponToCartAsync(cartViewModel);

            Assert.Equal(
                string.Format(ServicesDataConstants.ORDER_TOTAL_NOT_COVERING_MIN_COUPON_AMOUNT, coupon.MinimumOrderAmount),
                actual.StatusMessage);
        }

        [Fact]
        public async Task ApplyCouponToCartAsync_ReturnsCouponNotFoundStatusMessageWhenCouponNotFound()
        {
            await this.AddTestingUsersWithShoppingCartToDb();

            var cartViewModel = new ShoppingCartViewModel { CouponName = string.Empty };            

            var actual = await this.shoppingCartServiceMock.ApplyCouponToCartAsync(cartViewModel);

            Assert.Equal(
                string.Format(ServicesDataConstants.COUPON_NOT_FOUND_ERROR, cartViewModel.CouponName),
                actual.StatusMessage);
        }

        [Fact]
        public async Task ApplyCouponToCartAsync_ReturnsCouponNotFoundStatusMessageWhenCouponIsInactive()
        {
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingCouponsToDb();

            var inactiveCoupon = await this.DbContext.Coupons.SingleOrDefaultAsync(coupon => !coupon.IsActive);
            var cartViewModel = new ShoppingCartViewModel { CouponName = inactiveCoupon.Name };

            var actual = await this.shoppingCartServiceMock.ApplyCouponToCartAsync(cartViewModel);

            Assert.Equal(
                string.Format(ServicesDataConstants.COUPON_NOT_FOUND_ERROR, cartViewModel.CouponName),
                actual.StatusMessage);
        }

        [Fact]
        public async Task ApplyCouponToCartAsync_ProperlyAppliesPercentageDiscount()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();
            await this.AddTestingCouponsToDb();

            var coupon = await this.DbContext.Coupons.FirstAsync();

            var cartItems = await this.DbContext.ShoppingCartMenuItems.ToArrayAsync();

            var orderTotal = 0m;

            foreach (var cartItem in cartItems)
            {
                orderTotal += cartItem.Count * cartItem.MenuItem.Price;
            }

            var cartViewModel = new ShoppingCartViewModel
            {
                CouponName = coupon.Name,
                OrderTotalOriginal = orderTotal,
                MenuItems = new List<ShoppingCartMenuItemViewModel>()
                {
                    new ShoppingCartMenuItemViewModel
                    {
                        Price = cartItems.First().MenuItem.Price,
                        Count = cartItems.First().Count                        
                    },
                    new ShoppingCartMenuItemViewModel
                    {
                        Price = cartItems.Last().MenuItem.Price,
                        Count = cartItems.Last().Count
                    },
                },
            };

            var actual = await this.shoppingCartServiceMock.ApplyCouponToCartAsync(cartViewModel);

            var expectedDiscount = orderTotal * coupon.Discount / 100;
            var expectedDiscountedOrderTotal = orderTotal - expectedDiscount;

            Assert.Equal(expectedDiscountedOrderTotal, actual.OrderTotal);
        }

        [Fact]
        public async Task ApplyCouponToCartAsync_ProperlyAppliesAmountDiscount()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();
            await this.AddTestingCouponsToDb();

            var coupon = await this.DbContext.Coupons.Skip(1).Take(1).FirstAsync();

            var cartItems = await this.DbContext.ShoppingCartMenuItems.ToArrayAsync();

            var orderTotal = 0m;

            foreach (var cartItem in cartItems)
            {
                orderTotal += cartItem.Count * cartItem.MenuItem.Price;
            }

            var cartViewModel = new ShoppingCartViewModel
            {
                CouponName = coupon.Name,
                OrderTotalOriginal = orderTotal,
                MenuItems = new List<ShoppingCartMenuItemViewModel>()
                {
                    new ShoppingCartMenuItemViewModel
                    {
                        Price = cartItems.First().MenuItem.Price,
                        Count = cartItems.First().Count
                    },
                    new ShoppingCartMenuItemViewModel
                    {
                        Price = cartItems.Last().MenuItem.Price,
                        Count = cartItems.Last().Count
                    },
                },
            };

            var actual = await this.shoppingCartServiceMock.ApplyCouponToCartAsync(cartViewModel);

            var expectedDiscountedOrderTotal = orderTotal - coupon.Discount;

            Assert.Equal(expectedDiscountedOrderTotal, actual.OrderTotal);
        }

        [Fact]
        public async Task IncrementItemQuantityAsync_SucessfullyIncrementsItemQuantity()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();
            var cartItem = await this.DbContext.ShoppingCartMenuItems.LastAsync();

            Assert.Equal(TEST_USER_TEST_MENUITEM_COUNT, cartItem.Count);

            await this.shoppingCartServiceMock.IncrementItemQuantityAsync(user.Id, menuItem.Id);

            var expectedCount = TEST_USER_TEST_MENUITEM_COUNT + 1;            

            Assert.Equal(expectedCount, cartItem.Count);
        }

        [Fact]
        public async Task DecrementItemQuantityAsync_()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();
            var cartItem = await this.DbContext.ShoppingCartMenuItems
                .SingleOrDefaultAsync(menuItem => menuItem.MenuItem.Name == "Lasagna");

            Assert.Equal(TEST_USER_TEST_MENUITEM_COUNT, cartItem.Count);

            await this.shoppingCartServiceMock.DecrementItemQuantityAsync(user.Id, menuItem.Id);

            var expectedCount = TEST_USER_TEST_MENUITEM_COUNT - 1;

            Assert.Equal(expectedCount, cartItem.Count);
        }

        [Fact]
        public async Task RemoveItemAsync_SucessfullyRemovesItemFromCart()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();
            var cart = await this.DbContext.ShoppingCarts.FirstAsync();
            var cartItem = await this.DbContext.ShoppingCartMenuItems.FirstAsync();

            var initialShoppingCartIndividualItemsCount = 2;

            Assert.Equal(initialShoppingCartIndividualItemsCount, cart.ShoppingCartMenuItems.Count);

            await this.shoppingCartServiceMock.RemoveItemAsync(user.Id, menuItem.Id);

            Assert.Equal(initialShoppingCartIndividualItemsCount - 1, cart.ShoppingCartMenuItems.Count);
        }

        [Fact]
        public async Task GetUserIndividualItemsCountAsync_ReturnsZeroForAUserWithEmptyCart()
        {
            await this.AddTestingUserWithoutShoppingCart();

            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.shoppingCartServiceMock.GetUserIndividualItemsCountAsync(user.Id);

            var expectedCount = 0;

            Assert.Equal(expectedCount, actual);
        }

        [Fact]
        public async Task GetUserIndividualItemsCountAsync_ReturnsValidCount()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.shoppingCartServiceMock.GetUserIndividualItemsCountAsync(user.Id);

            var expectedCount = 2;

            Assert.Equal(expectedCount, actual);
        }

        [Fact]
        public async Task CheckIfItemIsLastAndIsToBeRemovedAsync_ReturnsFalseWhenItemIsNotToBeRemovedFromCart()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.shoppingCartServiceMock.CheckIfItemIsLastAndIsToBeRemovedAsync(user.Id, menuItem.Id);

            Assert.False(actual);
        }

        [Fact]
        public async Task CheckIfItemIsLastAndIsToBeRemovedAsync_ReturnsTrueWhenItemIsLastAndToBeRemovedFromCart()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();

            var menuItem = await this.DbContext.MenuItems.LastAsync();
            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.shoppingCartServiceMock.CheckIfItemIsLastAndIsToBeRemovedAsync(user.Id, menuItem.Id);

            Assert.True(actual);
        }

        private async Task AddTestingMenuItemsWithCategoryToDb()
        {
            await this.DbContext.Categories.AddAsync(
                new Category
                {
                    Name = TEST_CATEGORY_NAME
                });

            await this.DbContext.SaveChangesAsync();

            var category = await this.DbContext.Categories.FirstAsync();

            await this.DbContext.SubCategories.AddAsync(
               new SubCategory
               {
                   Name = TEST_SUBCATEGORY_NAME,
                   CategoryId = category.Id
               });

            await this.DbContext.SaveChangesAsync();

            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            await this.DbContext.MenuItems.AddAsync(
                new MenuItem
                {
                    Name = TEST_MENUITEM_NAME,
                    Price = TEST_MENUITEM_PRICE,
                    Image = TEST_IMAGE_URL,
                    CategoryId = category.Id,
                    SubCategoryId = subcategory.Id
                });

            await this.DbContext.MenuItems.AddAsync(
                new MenuItem
                {
                    Name = SECOND_TEST_MENUITEM_NAME,
                    Price = SECOND_TEST_MENUITEM_PRICE,
                    CategoryId = category.Id
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingUsersWithShoppingCartToDb()
        {
            await this.DbContext.AddAsync(
                new GourmeJunkUser
                {
                    Id = TEST_USER_ID,
                    UserName = TEST_USER_FIRSTNAME.ToLower(),
                    FirstName = TEST_USER_FIRSTNAME,
                    LastName = TEST_USER_LASTNAME,
                    ShoppingCart = new ShoppingCart
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = TEST_USER_ID
                    }
                });

            await this.DbContext.AddAsync(
               new GourmeJunkUser
               {
                   Id = SECOND_TEST_USER_ID,
                   UserName = SECOND_TEST_USER_FIRSTNAME.ToLower(),
                   FirstName = SECOND_TEST_USER_FIRSTNAME,
                   LastName = SECOND_TEST_USER_LASTNAME,
                   ShoppingCart = new ShoppingCart
                   {
                       Id = Guid.NewGuid().ToString(),
                       UserId = SECOND_TEST_USER_ID
                   }
               });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingShoppingCartMenuItemsToDb()
        {
            //Pesho
            var firstUser = await this.DbContext.Users.FirstAsync();
            var firstUserCart = await this.DbContext.ShoppingCarts
                .SingleOrDefaultAsync(cart => cart.UserId == firstUser.Id);

            //Lasagna
            var firstMenuItem = await this.DbContext.MenuItems.FirstAsync();

            //Spaghetti
            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            await this.DbContext.ShoppingCartMenuItems.AddAsync(
                new ShoppingCartMenuItems
                {
                    ShoppingCartId = firstUserCart.Id,
                    MenuItemId = firstMenuItem.Id,
                    //Pesho orders 2 Lasagnas
                    Count = TEST_USER_TEST_MENUITEM_COUNT
                });

            await this.DbContext.ShoppingCartMenuItems.AddAsync(
                new ShoppingCartMenuItems
                {
                    ShoppingCartId = firstUserCart.Id,
                    MenuItemId = secondMenuItem.Id,
                    Count = TEST_USER_SECOND_TEST_MENUITEM_COUNT
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingUserWithoutShoppingCart()
        {
            await this.DbContext.AddAsync(
                new GourmeJunkUser
                {
                    Id = TEST_USER_ID,
                    UserName = TEST_USER_FIRSTNAME.ToLower(),
                    FirstName = TEST_USER_FIRSTNAME,
                    LastName = TEST_USER_LASTNAME
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingCouponsToDb()
        {
            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = TEST_COUPON_NAME,
                    CouponType = TEST_COUPON_TYPE,
                    Discount = TEST_COUPON_DISCOUNT,
                    MinimumOrderAmount = TEST_COUPON_MIN_ORDER_AMOUNT,
                    IsActive = TEST_COUPON_ISACTIVE
                });

            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = SECOND_TEST_COUPON_NAME,
                    CouponType = SECOND_TEST_COUPON_TYPE,
                    Discount = SECOND_TEST_COUPON_DISCOUNT,
                    IsActive = SECOND_TEST_COUPON_ISACTIVE
                });

            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = THIRD_TEST_COUPON_NAME,
                    CouponType = THIRD_TEST_COUPON_TYPE,
                    Discount = THIRD_TEST_COUPON_DISCOUNT,
                    IsActive = THIRD_TEST_COUPON_ISACTIVE
                });

            await this.DbContext.SaveChangesAsync();
        }

        private static string GetModifiedDescription(ShoppingCartMenuItems[] userCartItems)
        {
            var expectedDescription = userCartItems.First().MenuItem.Description;

            if (expectedDescription != null)
            {
                if (expectedDescription.Length > ServicesDataConstants.CART_INDEX_MENUITEM_DESCRIPTION_MAX_CHARS)
                {
                    expectedDescription = userCartItems.First().MenuItem.Description
                        .Substring(0, ServicesDataConstants.CART_INDEX_MENUITEM_DESCRIPTION_MAX_CHARS - 1) + "...";
                }
            }

            return expectedDescription;
        }
    }
}
