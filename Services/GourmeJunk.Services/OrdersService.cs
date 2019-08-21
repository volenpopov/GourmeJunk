using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels;
using GourmeJunk.Models.ViewModels.Orders;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Order = GourmeJunk.Data.Models.Order;

namespace GourmeJunk.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> ordersRepository;
        private readonly IDeletableEntityRepository<MenuItem> menuItemsRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;

        public OrdersService(
            IRepository<Order> ordersRepository,
            IDeletableEntityRepository<MenuItem> menuItemsRepository,
            IRepository<ShoppingCart> shoppingCartsRepository)
        {
            this.ordersRepository = ordersRepository;
            this.menuItemsRepository = menuItemsRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
        }

        public OrderSummaryViewModel GetOrderSummaryViewModel(OrderSummaryInputModel model,
           string[] itemsIds, string[] itemsNames, string[] itemsPrices, string[] itemsCount, string pickupName)
        {
            var orderMenuItemsViewModels = new List<OrderItemViewModel>();

            for (int index = 0; index < itemsIds.Length; index++)
            {
                var orderMenuItemViewModel = new OrderItemViewModel
                {
                    Id = itemsIds[index],
                    Name = itemsNames[index],
                    Price = decimal.Parse(itemsPrices[index]),
                    Count = int.Parse(itemsCount[index])
                };

                orderMenuItemsViewModels.Add(orderMenuItemViewModel);
            }

            var orderSummaryViewModel = new OrderSummaryViewModel
            {
                UserId = model.UserId,
                PickupName = pickupName,
                CouponName = model.CouponName,
                OrderTotalOriginal = model.OrderTotalOriginal,
                OrderTotal = model.OrderTotal,
                OrderItems = orderMenuItemsViewModels
            };

            return orderSummaryViewModel;
        }

        public async Task<string> CreateOrderAsync(OrderInputModel model,
            string[] itemsIds, string[] itemsCount, string stripeEmail, string stripeToken)
        {
            var order = new Order
            {
                UserId = model.UserId,
                OrderTotalOriginal = model.OrderTotalOriginal,
                OrderTotal = model.OrderTotal,
                PickupName = model.PickupName,
                PickUpDateAndTime = DateTime.Parse($"{model.PickupDate} {model.PickupTime}"),
                CouponName = model.CouponName,
                PhoneNumber = model.PhoneNumber,
                Comments = model.Comments
            };

            var orderMenuItems = new List<OrderMenuItems>();

            for (int index = 0; index < itemsIds.Length; index++)
            {
                var orderMenuItem = new OrderMenuItems
                {
                    Order = order,
                    MenuItemId = itemsIds[index],
                    Count = int.Parse(itemsCount[index])
                };

                orderMenuItems.Add(orderMenuItem);
            }

            order.OrderMenuItems = orderMenuItems;

            await this.ordersRepository.AddAsync(order);

            if (stripeToken != null)
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Source = stripeToken
                });

                var chargeAmount = 0m;

                if (model.OrderTotal != default(decimal))
                {
                    chargeAmount = model.OrderTotal * 100;
                }
                else
                {
                    chargeAmount = model.OrderTotalOriginal * 100;
                }

                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = (long)chargeAmount,
                    Description = ServicesDataConstants.Stripe.STRIPE_ORDER_DESCRIPTION
                        + order.Id.Substring(order.Id.Length - 5, 5),
                    Currency = ServicesDataConstants.Stripe.STRIPE_CURRENCY,
                    CustomerId = customer.Id
                });

                order.TransactionId = charge.BalanceTransactionId;

                if (charge.Status.ToLower() == ServicesDataConstants.Stripe.STRIPE_CHARGE_STATUS_SUCCEEDED)
                {
                    order.PaymentStatus = PaymentStatus.Approved;
                }
                else
                {
                    order.PaymentStatus = PaymentStatus.Rejected;
                    order.OrderStatus = OrderStatus.Cancelled;
                }
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Rejected;
                order.OrderStatus = OrderStatus.Cancelled;
            }

            await this.ordersRepository.SaveChangesAsync();

            return order.Id;
        }

        public async Task<OrderConfirmViewModel> GetOrderConfirmViewModelAsync(string orderId, string userId)
        {
            var order = await this.GetOrderByIdAndUserIdAsync(orderId, userId);

            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Include(cart => cart.ShoppingCartMenuItems)
                .ThenInclude(cartItem => cartItem.MenuItem)
                .SingleOrDefaultAsync(cart => cart.UserId == userId);

            if (shoppingCart == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_USER_CART, userId));
            }

            var orderConfirmViewModel = new OrderConfirmViewModel
            {
                Id = order.Id,
                PickupName = order.PickupName,
                PhoneNumber = order.PhoneNumber,
                OrderTotalOriginal = order.OrderTotalOriginal,
                OrderTotal = order.OrderTotal,
                PickUpDateAndTime = order.PickUpDateAndTime,
                Comments = order.Comments,
                CouponName = order.CouponName,
                Status = order.OrderStatus.ToString()
            };

            var orderItemsViewModels = new List<OrderItemViewModel>();

            foreach (var item in shoppingCart.ShoppingCartMenuItems)
            {
                var orderItemViewModel = new OrderItemViewModel
                {
                    Id = item.MenuItemId,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };

                orderItemsViewModels.Add(orderItemViewModel);
            }

            orderConfirmViewModel.OrderItems = orderItemsViewModels;

            return orderConfirmViewModel;
        }

        public async Task<OrdersHistoryListViewModel> GetOrdersHistoryListViewModelAsync(string userId, int productPage)
        {
            var ordersHistoryListViewModel = new OrdersHistoryListViewModel();

            var orders = await this.ordersRepository
                .All()
                .Include(order => order.User)
                .Include(order => order.OrderMenuItems)
                .Where(order => order.UserId == userId)
                .ToArrayAsync();

            foreach (var order in orders)
            {
                var orderHistoryViewModel = new OrderHistoryViewModel
                {
                    Id = order.Id,
                    Email = order.User.Email,
                    PickupName = order.PickupName,
                    PickupTime = order.PickUpDateAndTime.ToString("dd/MM/yyyy HH:mm"),
                    OrderTotal = order.OrderTotal != default(decimal)
                        ? order.OrderTotal.ToString("C")
                        : order.OrderTotalOriginal.ToString("C"),
                    Status = order.OrderStatus.ToString(),
                    TotalItems = order.OrderMenuItems.Count
                };

                ordersHistoryListViewModel.Orders.Add(orderHistoryViewModel);
            }

            var totalItems = ordersHistoryListViewModel.Orders.Count();

            ordersHistoryListViewModel.Orders = ordersHistoryListViewModel.Orders
                .OrderBy(order => order.PickupTime)
                .Skip((productPage - 1) * ServicesDataConstants.PAGE_SIZE)
                .Take(ServicesDataConstants.PAGE_SIZE)
                .ToArray();

            ordersHistoryListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = ServicesDataConstants.PAGE_SIZE,
                TotalItems = totalItems,
                UrlParam = ServicesDataConstants.PAGINATION_URL_PARAM
            };

            return ordersHistoryListViewModel;
        }

        public async Task<string> GetOrderStatusAsync(string orderId, string userId)
        {
            var order = await this.GetOrderByIdAndUserIdAsync(orderId, userId);

            return order.OrderStatus.ToString();
        }

        private async Task<Order> GetOrderByIdAndUserIdAsync(string orderId, string userId)
        {
            var order = await this.ordersRepository
                .All()
                .Include(ordr => ordr.User)
                .SingleOrDefaultAsync(ordr => ordr.Id == orderId && ordr.UserId == userId);

            if (order == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), orderId));
            }

            return order;
        }
    }
}
