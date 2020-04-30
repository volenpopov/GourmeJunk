using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels.Orders;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class OrdersServiceTests : BaseServiceTests
    {
        private IOrdersService ordersServiceMock
           => this.ServiceProvider.GetRequiredService<IOrdersService>();

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

        private const decimal TEST_ORDER_ORDERTOTALORIGINAL = 100m;
        private const decimal TEST_ORDER_ORDERTOTAL = 90m;
        private const string TEST_ORDER_PICKUP_NAME = "Pickup Guy";
        private const string TEST_ORDER_PICKUP_DATE = "20/12/2020";
        private const string TEST_ORDER_PICKUP_TIME = "15:00";
        private const string TEST_ORDER_PHONE_NUMBER = "12345678";
        private const string TEST_ORDER_COMMENTS = "No comments.";

        private const int TEST_PRODUCT_PAGE = 1;

        [Fact]
        public async Task GetOrderSummaryViewModel_()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();
            await this.AddTestingCouponsToDb();

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();

            //BIGDEAL: 10% discount
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var shoppingCartMenuItems = await this.DbContext.ShoppingCartMenuItems.ToArrayAsync();
            var orderTotalOriginal = CalculateOrderTotal(shoppingCartMenuItems);
            var orderTotalDiscounted = orderTotalOriginal - (orderTotalOriginal * (coupon.Discount/100));

            var orderSummaryInputModel = new OrderSummaryInputModel
            {
                UserId = user.Id,
                CouponName = coupon.Name,
                OrderTotalOriginal = orderTotalOriginal,
                OrderTotal = orderTotalDiscounted                
            };

            var itemsIds = await this.DbContext.MenuItems.Select(item => item.Id).ToArrayAsync();
            var itemsNames = await this.DbContext.MenuItems.Select(item => item.Name).ToArrayAsync();
            var itemsPrices = await this.DbContext.MenuItems.Select(item => item.Price.ToString()).ToArrayAsync();
            var itemsCount = await this.DbContext.ShoppingCartMenuItems.Select(cartItem => cartItem.Count.ToString()).ToArrayAsync();

            var pickupName = TEST_ORDER_PICKUP_NAME;

            var actual = this.ordersServiceMock.GetOrderSummaryViewModel(
                orderSummaryInputModel, itemsIds, itemsNames, itemsPrices, itemsCount, pickupName);

            Assert.NotNull(actual);
            Assert.IsType<OrderSummaryViewModel>(actual);

            Assert.Equal(user.Id, actual.UserId);
            Assert.Equal(coupon.Name, actual.CouponName);
            Assert.Equal(orderTotalOriginal, actual.OrderTotalOriginal);
            Assert.Equal(orderTotalDiscounted, actual.OrderTotal);
            Assert.Equal(pickupName, actual.PickupName);

            Assert.Collection(actual.OrderItems,
                elem1 =>
                {
                    Assert.Equal(itemsIds.First(), elem1.Id);
                    Assert.Equal(itemsNames.First(), elem1.Name);
                    Assert.Equal(itemsPrices.First(), elem1.Price.ToString());
                    Assert.Equal(itemsCount.First(), elem1.Count.ToString());
                },
                elem2 =>
                {
                    Assert.Equal(itemsIds.Last(), elem2.Id);
                    Assert.Equal(itemsNames.Last(), elem2.Name);
                    Assert.Equal(itemsPrices.Last(), elem2.Price.ToString());
                    Assert.Equal(itemsCount.Last(), elem2.Count.ToString());
                });
        }        

        [Fact]
        public async Task CreateOrderAsync_ProperlyCreatesOrder()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();
            await this.AddTestingCouponsToDb();

            //Pesho
            var user = await this.DbContext.Users.FirstAsync();

            //BIGDEAL: 10% discount
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var shoppingCartMenuItems = await this.DbContext.ShoppingCartMenuItems.ToArrayAsync();
            var orderTotalOriginal = CalculateOrderTotal(shoppingCartMenuItems);
            var orderTotalDiscounted = orderTotalOriginal - (orderTotalOriginal * (coupon.Discount / 100));

            var orderSummaryInputModel = new OrderInputModel
            {
                UserId = user.Id,
                CouponName = coupon.Name,
                OrderTotalOriginal = orderTotalOriginal,
                OrderTotal = orderTotalDiscounted,
                PickupName = TEST_ORDER_PICKUP_NAME,
                PhoneNumber = TEST_ORDER_PHONE_NUMBER,
                PickupDate = TEST_ORDER_PICKUP_DATE,
                PickupTime = TEST_ORDER_PICKUP_TIME,
                Comments = TEST_ORDER_COMMENTS                
            };

            var itemsIds = await this.DbContext.MenuItems.Select(item => item.Id).OrderByDescending(x => x).ToArrayAsync();            
            var itemsCount = await this.DbContext.ShoppingCartMenuItems.Select(cartItem => cartItem.Count.ToString()).ToArrayAsync();

            var actualOrderId = await this.ordersServiceMock.CreateOrderAsync(
                orderSummaryInputModel, itemsIds, itemsCount, null, null);

            var actualOrder = await this.DbContext.Orders.FirstAsync();

            Assert.NotNull(actualOrderId);
            Assert.IsType<string>(actualOrderId);

            Assert.Equal(user.Id, actualOrder.UserId);
            Assert.Equal(coupon.Name, actualOrder.CouponName);
            Assert.Equal(orderTotalOriginal, actualOrder.OrderTotalOriginal);
            Assert.Equal(orderTotalDiscounted, actualOrder.OrderTotal);

            //Because of not using Stripe in the unit tests, the order is always with status cancelled here:
            Assert.Equal(OrderStatus.Cancelled, actualOrder.OrderStatus);

            Assert.Equal(DateTime.UtcNow.ToShortDateString(), actualOrder.OrderDate.ToShortDateString());
            Assert.Equal(orderSummaryInputModel.PickupName, actualOrder.PickupName);
            Assert.Equal(DateTime.Parse($"{orderSummaryInputModel.PickupTime} {orderSummaryInputModel.PickupDate}"), actualOrder.PickUpDateAndTime);
            Assert.Equal(orderSummaryInputModel.PhoneNumber, actualOrder.PhoneNumber);
            Assert.Equal(orderSummaryInputModel.Comments, actualOrder.Comments);
            Assert.Collection(actualOrder.OrderMenuItems,
                elem1 =>
                {
                    Assert.Equal(shoppingCartMenuItems.First().Count, elem1.Count);
                    Assert.Equal(shoppingCartMenuItems.First().MenuItemId, elem1.MenuItemId);
                },
                elem2 =>
                {
                    Assert.Equal(shoppingCartMenuItems.Last().Count, elem2.Count);
                    Assert.Equal(shoppingCartMenuItems.Last().MenuItemId, elem2.MenuItemId);
                });
        }

        [Fact]
        public async Task CreateOrderAsync_DeletesShoppingCartItemsAfterOrderIsCreatedAndPlaced()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingShoppingCartMenuItemsToDb();
            await this.AddTestingCouponsToDb();

            var user = await this.DbContext.Users.FirstAsync();

            var orderSummaryInputModel = new OrderInputModel
            {
                UserId = user.Id,
                PickupDate = TEST_ORDER_PICKUP_DATE,
                PickupTime = TEST_ORDER_PICKUP_TIME
            };

            var shoppingCartMenuItems = this.DbContext.ShoppingCartMenuItems;

            Assert.NotEmpty(shoppingCartMenuItems);

            await this.ordersServiceMock.CreateOrderAsync(orderSummaryInputModel, new string[0], null, null, null);
            
            Assert.Empty(shoppingCartMenuItems);
        }

        [Fact]
        public async Task GetOrderFullInfoViewModelAsync_ReturnsValidModelWhenSearchedOnlyByOrderId()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();

            var actual = await this.ordersServiceMock.GetOrderFullInfoViewModelAsync(order.Id, null);

            Assert.NotNull(actual);
            Assert.IsType<OrderFullInfoViewModel>(actual);

            Assert.Equal(order.Id, actual.Id);
            Assert.Equal(order.OrderTotalOriginal, actual.OrderTotalOriginal);
            Assert.Equal(order.OrderTotal, actual.OrderTotal);
            Assert.Equal(order.OrderStatus.ToString(), actual.Status);
            Assert.Equal(order.CouponName, actual.CouponName);
            Assert.Equal(order.Comments, actual.Comments);
            Assert.Equal(order.PickupName, actual.PickupName);
            Assert.Equal(order.PickUpDateAndTime, actual.PickUpDateAndTime);
            Assert.Equal(order.PhoneNumber, actual.PhoneNumber);

            Assert.Collection(actual.OrderItems,
                elem1 =>
                {
                    Assert.Equal(order.OrderMenuItems.First().MenuItemId, elem1.Id);
                    Assert.Equal(order.OrderMenuItems.First().MenuItem.Name, elem1.Name);
                    Assert.Equal(order.OrderMenuItems.First().MenuItem.Price, elem1.Price);
                    Assert.Equal(order.OrderMenuItems.First().Count, elem1.Count);
                },
                elem2 =>
                {
                    Assert.Equal(order.OrderMenuItems.Last().MenuItemId, elem2.Id);
                    Assert.Equal(order.OrderMenuItems.Last().MenuItem.Name, elem2.Name);
                    Assert.Equal(order.OrderMenuItems.Last().MenuItem.Price, elem2.Price);
                    Assert.Equal(order.OrderMenuItems.Last().Count, elem2.Count);
                });
        }

        [Fact]
        public async Task GetOrderFullInfoViewModelAsync_ReturnsValidModelWhenSearchedByBothOrderIdAndUserId()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.ordersServiceMock.GetOrderFullInfoViewModelAsync(order.Id, user.Id);

            Assert.NotNull(actual);
            Assert.IsType<OrderFullInfoViewModel>(actual);

            Assert.Equal(order.Id, actual.Id);
            Assert.Equal(order.OrderTotalOriginal, actual.OrderTotalOriginal);
            Assert.Equal(order.OrderTotal, actual.OrderTotal);
            Assert.Equal(order.OrderStatus.ToString(), actual.Status);
            Assert.Equal(order.CouponName, actual.CouponName);
            Assert.Equal(order.Comments, actual.Comments);
            Assert.Equal(order.PickupName, actual.PickupName);
            Assert.Equal(order.PickUpDateAndTime, actual.PickUpDateAndTime);
            Assert.Equal(order.PhoneNumber, actual.PhoneNumber);

            Assert.Collection(actual.OrderItems,
                elem1 =>
                {
                    Assert.Equal(order.OrderMenuItems.First().MenuItemId, elem1.Id);
                    Assert.Equal(order.OrderMenuItems.First().MenuItem.Name, elem1.Name);
                    Assert.Equal(order.OrderMenuItems.First().MenuItem.Price, elem1.Price);
                    Assert.Equal(order.OrderMenuItems.First().Count, elem1.Count);
                },
                elem2 =>
                {
                    Assert.Equal(order.OrderMenuItems.Last().MenuItemId, elem2.Id);
                    Assert.Equal(order.OrderMenuItems.Last().MenuItem.Name, elem2.Name);
                    Assert.Equal(order.OrderMenuItems.Last().MenuItem.Price, elem2.Price);
                    Assert.Equal(order.OrderMenuItems.Last().Count, elem2.Count);
                });
        }

        [Fact]
        public async Task GetOrderFullInfoViewModelAsync_ReturnsNullReferenceWhenSearchedOnlyByOrderIdAndOrderIsNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ordersServiceMock.GetOrderFullInfoViewModelAsync(string.Empty, null));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), string.Empty), exception.Message);
        }

        [Fact]
        public async Task GetOrderFullInfoViewModelAsync_ReturnsNullReferenceWhenSearchedByBothOrderIdAndUserIdAndOrderIsNotFound()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();
            var otherUser = await this.DbContext.Users.LastAsync();           

            var firstException = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ordersServiceMock.GetOrderFullInfoViewModelAsync(null, user.Id));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), string.Empty), firstException.Message);

            var secondException = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ordersServiceMock.GetOrderFullInfoViewModelAsync(order.Id, string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), order.Id), secondException.Message);

            var thirdException = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ordersServiceMock.GetOrderFullInfoViewModelAsync(order.Id, otherUser.Id));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), order.Id), thirdException.Message);
        }

        [Fact]
        public async Task GetOrdersHistoryListViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.ordersServiceMock.GetOrdersHistoryListViewModelAsync(user.Id, TEST_PRODUCT_PAGE);

            Assert.NotNull(actual);
            Assert.IsType<OrdersListViewModel>(actual);

            Assert.Equal(order.Id, actual.Orders.First().Id);
            Assert.Equal(user.Email, actual.Orders.First().Email);
            Assert.Equal(order.OrderTotal.ToString("C") ?? order.OrderTotalOriginal.ToString("C"), actual.Orders.First().OrderTotal);
            Assert.Equal(order.PickupName, actual.Orders.First().PickupName);
            Assert.Equal(order.OrderStatus.ToString(), actual.Orders.First().Status);
            Assert.Equal(order.OrderMenuItems.Count, actual.Orders.First().TotalItems);            
        }

        [Fact]
        public async Task GetOrderStatusAsync_ReturnsProperStatus()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.ordersServiceMock.GetOrderStatusAsync(order.Id, user.Id);

            var expected = order.OrderStatus.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetManageOrdersListViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var orderItems = await this.DbContext.OrderMenuItems.OrderBy(x => x.MenuItem.Name).ToArrayAsync();
            var actual = await this.ordersServiceMock.GetManageOrdersListViewModelAsync(TEST_PRODUCT_PAGE);

            Assert.Equal(order.Id, actual.Orders.First().Id);
            Assert.Equal(order.Comments, actual.Orders.First().Comments);
            Assert.Equal(order.PickUpDateAndTime.ToString(ServicesDataConstants.DATETIME_FORMAT), actual.Orders.First().PickupTime);
            Assert.Equal(order.OrderStatus.ToString(), actual.Orders.First().Status);

            Assert.Collection(actual.Orders.First().OrderItems,
                elem1 =>
                {
                    Assert.Equal(orderItems.First().MenuItem.Name, elem1.Name);                    
                    Assert.Equal(orderItems.First().Count, elem1.Count);
                },
                elem2 =>
                {
                    Assert.Equal(orderItems.Last().MenuItem.Name, elem2.Name);                    
                    Assert.Equal(orderItems.Last().Count, elem2.Count);
                });
        }

        [Fact]
        public async Task GetManageOrdersListViewModelAsync_ReturnsModelWhenOrdersAreWithStatusCooking()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            order.OrderStatus = OrderStatus.Cooking;            

            await this.DbContext.SaveChangesAsync();

            var actual = await this.ordersServiceMock.GetManageOrdersListViewModelAsync(TEST_PRODUCT_PAGE);

            Assert.NotEmpty(actual.Orders);
        }

        [Fact]
        public async Task GetManageOrdersListViewModelAsync_ReturnsEmptyWhenAllOrdersAreWithStatusDelivered()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            order.OrderStatus = OrderStatus.Delivered;

            await this.DbContext.SaveChangesAsync();

            var actual = await this.ordersServiceMock.GetManageOrdersListViewModelAsync(TEST_PRODUCT_PAGE);

            Assert.Empty(actual.Orders);
        }

        [Fact]
        public async Task GetManageOrdersListViewModelAsync_ReturnsEmptyWhenAllOrdersAreWithStatusReady()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            order.OrderStatus = OrderStatus.Ready;

            await this.DbContext.SaveChangesAsync();

            var actual = await this.ordersServiceMock.GetManageOrdersListViewModelAsync(TEST_PRODUCT_PAGE);

            Assert.Empty(actual.Orders);
        }        

        [Fact]
        public async Task GetManageOrdersListViewModelAsync_ReturnsEmptyWhenAllOrdersAreWithStatusCancelled()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            order.OrderStatus = OrderStatus.Cancelled;

            await this.DbContext.SaveChangesAsync();

            var actual = await this.ordersServiceMock.GetManageOrdersListViewModelAsync(TEST_PRODUCT_PAGE);

            Assert.Empty(actual.Orders);
        }        

        [Fact]
        public async Task UpdateOrderStatusToCookingAsync_SucessfullyUpdatesStatusToCooking()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();

            Assert.True(order.OrderStatus != OrderStatus.Cooking);

            await this.ordersServiceMock.UpdateOrderStatusToCookingAsync(order.Id);

            Assert.True(order.OrderStatus == OrderStatus.Cooking);
        }

        [Fact]
        public async Task UpdateOrderStatusToReadyAsync_SucessfullyUpdatesStatusToReady()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();

            Assert.True(order.OrderStatus != OrderStatus.Ready);

            await this.ordersServiceMock.UpdateOrderStatusToReadyAsync(order.Id);

            Assert.True(order.OrderStatus == OrderStatus.Ready);
        }

        [Fact]
        public async Task UpdateOrderStatusToCancelledAsync_SucessfullyUpdatesStatusToCancelled()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();
            var user = await this.DbContext.Users.FirstAsync();

            Assert.True(order.OrderStatus != OrderStatus.Cancelled);

            await this.ordersServiceMock.UpdateOrderStatusToCancelledAsync(order.Id, user.Id);

            Assert.True(order.OrderStatus == OrderStatus.Cancelled);
        }

        [Fact]
        public async Task UpdateOrderStatusToDeliveredAsync_SucessfullyUpdatesStatusToDelivered()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();
            await this.AddTestingCouponsToDb();
            await this.AddTestingUsersWithShoppingCartToDb();
            await this.AddTestingOrderAndOrderMenuItemsToDb();

            var order = await this.DbContext.Orders.FirstAsync();

            Assert.True(order.OrderStatus != OrderStatus.Delivered);

            await this.ordersServiceMock.UpdateOrderStatusToDeliveredAsync(order.Id);

            Assert.True(order.OrderStatus == OrderStatus.Delivered);
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

        private async Task AddTestingOrderAndOrderMenuItemsToDb()
        {
            var user = await this.DbContext.Users.FirstAsync();
            var coupon = await this.DbContext.Coupons.FirstAsync();
            var menuItems = await this.DbContext.MenuItems.ToArrayAsync();

            await this.DbContext.Orders.AddAsync(
                new Order
                {
                    UserId = user.Id,
                    OrderStatus = OrderStatus.Submitted,
                    OrderDate = DateTime.UtcNow,
                    OrderTotalOriginal = TEST_ORDER_ORDERTOTALORIGINAL,
                    OrderTotal = TEST_ORDER_ORDERTOTAL,
                    CouponName = coupon.Name,
                    PickUpDateAndTime = DateTime.Parse($"{TEST_ORDER_PICKUP_DATE} {TEST_ORDER_PICKUP_TIME}"),
                    PickupName = TEST_ORDER_PICKUP_NAME,
                    PhoneNumber = TEST_ORDER_PHONE_NUMBER,
                    Comments = TEST_ORDER_COMMENTS,
                });

            await this.DbContext.SaveChangesAsync();

            var order = await this.DbContext.Orders.FirstAsync();

            await this.DbContext.OrderMenuItems.AddAsync(
                new OrderMenuItems
                {
                    OrderId = order.Id,
                    MenuItemId = menuItems.First().Id,
                    Count = TEST_USER_TEST_MENUITEM_COUNT
                });

            await this.DbContext.OrderMenuItems.AddAsync(
                new OrderMenuItems
                {
                    OrderId = order.Id,
                    MenuItemId = menuItems.Last().Id,
                    Count = TEST_USER_SECOND_TEST_MENUITEM_COUNT
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddMoreTestingOrdersWithDifferentStatusesAndOrderMenuItems()
        {
            var user = await this.DbContext.Users.FirstAsync();
            var coupon = await this.DbContext.Coupons.FirstAsync();
            var menuItems = await this.DbContext.MenuItems.ToArrayAsync();

            await this.DbContext.Orders.AddAsync(
                new Order
                {
                    UserId = user.Id,
                    OrderStatus = OrderStatus.Cooking,
                    OrderDate = DateTime.UtcNow,
                    OrderTotalOriginal = TEST_ORDER_ORDERTOTALORIGINAL,
                    OrderTotal = TEST_ORDER_ORDERTOTAL,
                    CouponName = coupon.Name,
                    PickUpDateAndTime = DateTime.Parse($"{TEST_ORDER_PICKUP_DATE} {TEST_ORDER_PICKUP_TIME}"),
                    PickupName = TEST_ORDER_PICKUP_NAME,
                    PhoneNumber = TEST_ORDER_PHONE_NUMBER,
                    Comments = TEST_ORDER_COMMENTS,
                });

            await this.DbContext.SaveChangesAsync();

            var order = await this.DbContext.Orders.FirstAsync();

            await this.DbContext.OrderMenuItems.AddAsync(
                new OrderMenuItems
                {
                    OrderId = order.Id,
                    MenuItemId = menuItems.First().Id,
                    Count = TEST_USER_TEST_MENUITEM_COUNT
                });

            await this.DbContext.OrderMenuItems.AddAsync(
                new OrderMenuItems
                {
                    OrderId = order.Id,
                    MenuItemId = menuItems.Last().Id,
                    Count = TEST_USER_SECOND_TEST_MENUITEM_COUNT
                });

            await this.DbContext.SaveChangesAsync();
        }

        private static decimal CalculateOrderTotal(ShoppingCartMenuItems[] shoppingCartMenuItems)
        {
            var orderTotal = 0m;

            foreach (var cartItem in shoppingCartMenuItems)
            {
                orderTotal += cartItem.Count * cartItem.MenuItem.Price;
            }

            return orderTotal;
        }
    }
}
