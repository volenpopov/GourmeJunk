using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels;
using GourmeJunk.Models.ViewModels.Orders;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = GourmeJunk.Data.Models.Order;

namespace GourmeJunk.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> ordersRepository;
        private readonly IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsService;
        private readonly IRepository<GourmeJunkUser> usersRepository;
        private readonly IEmailSender emailSender;

        public OrdersService(
            IRepository<Order> ordersRepository,
            IDeletableEntityRepository<ShoppingCartMenuItems> shoppingCartMenuItemsService,
            IRepository<GourmeJunkUser> usersRepository,
            IEmailSender emailSender)
        {
            this.ordersRepository = ordersRepository;
            this.shoppingCartMenuItemsService = shoppingCartMenuItemsService;
            this.usersRepository = usersRepository;
            this.emailSender = emailSender;
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

                var orderIdShort = order.Id.Substring(order.Id.Length - ServicesDataConstants.ORDER_ID_SHORT_LENGTH, ServicesDataConstants.ORDER_ID_SHORT_LENGTH);

                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = (long)chargeAmount,
                    Description = ServicesDataConstants.Stripe.STRIPE_ORDER_DESCRIPTION
                        + orderIdShort,
                    Currency = ServicesDataConstants.Stripe.STRIPE_CURRENCY,
                    CustomerId = customer.Id
                });

                order.TransactionId = charge.BalanceTransactionId;

                if (charge.Status.ToLower() == ServicesDataConstants.Stripe.STRIPE_CHARGE_STATUS_SUCCEEDED)
                {
                    order.PaymentStatus = PaymentStatus.Approved;

                    var userEmail = this.GetUserEmail(model.UserId);                     

                    await this.emailSender.SendEmailAsync(userEmail, 
                        ServicesDataConstants.Email.EMAIL_SUBJECT_ORDER_CREATED,
                        string.Format(ServicesDataConstants.Email.EMAIL_CONTENT_ORDER_SUBMITTED, orderIdShort));
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

            var userShoppingCartMenuItems = await this.shoppingCartMenuItemsService
                .All()
                .Include(cartItem => cartItem.ShoppingCart)
                .Where(cartItem => cartItem.ShoppingCart.UserId == model.UserId)
                .ToArrayAsync();

            this.shoppingCartMenuItemsService.HardDeleteRange(userShoppingCartMenuItems);

            await this.ordersRepository.SaveChangesAsync();

            return order.Id;
        }

        public async Task<OrderFullInfoViewModel> GetOrderFullInfoViewModelAsync(string orderId, string userId)
        {
            var order = new Order();

            if (userId != null)
            {
                order = await this.GetOrderByIdAndUserIdAsync(orderId, userId);
            }
            else
            {
                order = await this.GetOrderByIdAsync(orderId);
            }

            var OrderFullInfoViewModel = new OrderFullInfoViewModel
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

            foreach (var item in order.OrderMenuItems)
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

            OrderFullInfoViewModel.OrderItems = orderItemsViewModels;

            return OrderFullInfoViewModel;
        }

        public async Task<OrdersListViewModel> GetOrdersHistoryListViewModelAsync(string userId, int productPage)
        {
            var ordersHistoryListViewModel = new OrdersListViewModel();

            var orders = await this.ordersRepository
                .All()
                .Include(order => order.User)
                .Include(order => order.OrderMenuItems)
                .Where(order => order.UserId == userId)
                .ToArrayAsync();

            foreach (var order in orders)
            {
                var orderHistoryViewModel = new OrderViewModel
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
                UrlParam = ServicesDataConstants.ORDER_HISTORY_PAGINATION_URL_PARAM
            };

            return ordersHistoryListViewModel;
        }

        public async Task<string> GetOrderStatusAsync(string orderId, string userId)
        {
            var order = await this.GetOrderByIdAndUserIdAsync(orderId, userId);

            return order.OrderStatus.ToString();
        }

        public async Task<ManageOrdersListViewModel> GetManageOrdersListViewModelAsync(int productPage)
        {
            var manageOrdersListViewModel = new ManageOrdersListViewModel();

            var orders = await this.ordersRepository
                .All()
                .Where(order => order.OrderStatus != OrderStatus.Delivered
                    && order.OrderStatus != OrderStatus.Cancelled
                    && order.OrderStatus != OrderStatus.Ready)
                .Include(order => order.OrderMenuItems)
                .ThenInclude(orderMenuItem => orderMenuItem.MenuItem)
                .ToArrayAsync();

            foreach (var order in orders)
            {
                var manageOrderViewModel = new ManageOrderViewModel
                {
                    Id = order.Id,
                    PickupTime = order.PickUpDateAndTime.ToString("dd/MM/yyyy HH:mm"),
                    Comments = order.Comments,
                    Status = order.OrderStatus.ToString(),
                };

                foreach (var item in order.OrderMenuItems)
                {
                    var manageOrderItemViewModel = new OrderItemViewModel
                    {
                        Name = item.MenuItem.Name,
                        Count = item.Count
                    };

                    manageOrderViewModel.OrderItems.Add(manageOrderItemViewModel);
                }

                manageOrdersListViewModel.Orders.Add(manageOrderViewModel);
            }

            var totalItems = manageOrdersListViewModel.Orders.Count();

            manageOrdersListViewModel.Orders = manageOrdersListViewModel.Orders
                .OrderBy(order => order.PickupTime)
                .Skip((productPage - 1) * ServicesDataConstants.PAGE_SIZE)
                .Take(ServicesDataConstants.PAGE_SIZE)
                .ToArray();

            manageOrdersListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = ServicesDataConstants.PAGE_SIZE,
                TotalItems = totalItems,
                UrlParam = ServicesDataConstants.MANAGE_ORDER_PAGINATION_URL_PARAM
            };

            return manageOrdersListViewModel;
        }

        public async Task UpdateOrderStatusToCookingAsync(string orderId)
        {
            var order = await this.GetOrderByIdAsync(orderId);

            order.OrderStatus = OrderStatus.Cooking;

            await this.ordersRepository.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusToReadyAsync(string orderId)
        {
            var order = await this.GetOrderByIdAsync(orderId);

            order.OrderStatus = OrderStatus.Ready;

            await this.ordersRepository.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusToCancelledAsync(string orderId, string userId)
        {
            var order = await this.GetOrderByIdAsync(orderId);

            order.OrderStatus = OrderStatus.Cancelled;           

            await this.emailSender.SendEmailAsync(order.User.Email,
                        ServicesDataConstants.Email.EMAIL_SUBJECT_ORDER_CANCELLED,
                        string.Format(ServicesDataConstants.Email.EMAIL_CONTENT_ORDER_CANCELLED, 
                            orderId.Substring(orderId.Length - ServicesDataConstants.ORDER_ID_SHORT_LENGTH, ServicesDataConstants.ORDER_ID_SHORT_LENGTH)));

            await this.ordersRepository.SaveChangesAsync();
        }
        
        public async Task UpdateOrderStatusToDeliveredAsync(string orderId)
        {
            var order = await this.GetOrderByIdAsync(orderId);

            order.OrderStatus = OrderStatus.Delivered;

            await this.ordersRepository.SaveChangesAsync();
        }

        public async Task<OrdersListViewModel> GetOrdersListViewModelAsync(int productPage, string userId)
        {
            var ordersListViewModel = new OrdersListViewModel();

            var orders = await this.ordersRepository
                 .All()
                 .Include(order => order.User)
                 .Include(order => order.OrderMenuItems)
                 .Where(order => order.UserId == userId)
                 .ToArrayAsync();

            ordersListViewModel = PopulateOrdersListViewModel(orders);

            var totalItems = ordersListViewModel.Orders.Count();

            ordersListViewModel.Orders = ordersListViewModel.Orders
                .OrderBy(order => order.PickupTime)
                .Skip((productPage - 1) * ServicesDataConstants.PAGE_SIZE)
                .Take(ServicesDataConstants.PAGE_SIZE)
                .ToArray();

            ordersListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = ServicesDataConstants.PAGE_SIZE,
                TotalItems = totalItems,
                UrlParam = ServicesDataConstants.ORDER_HISTORY_PAGINATION_URL_PARAM
            };

            return ordersListViewModel;
        }

        public async Task<OrdersListViewModel> GetOrdersPickupListViewModelAsync(int productPage, string searchEmail, string searchPhone, string searchName)
        {
            var ordersListViewModel = new OrdersListViewModel();

            var orders = await this.ordersRepository
                 .All()                 
                 .Where(order => order.OrderStatus == OrderStatus.Ready)
                 .Include(order => order.User)
                 .Include(order => order.OrderMenuItems)
                 .ToArrayAsync();

            ordersListViewModel = PopulateOrdersListViewModel(orders);
            
            var paramsBuilder = new StringBuilder();

            paramsBuilder.Append(ServicesDataConstants.ORDERS_PICKUP_PAGINATION_URL_PARAM);

            paramsBuilder.Append(ServicesDataConstants.SEARCH_NAME_PARAM);
            if (searchName != null)
            {
                paramsBuilder.Append(searchName);
            }

            paramsBuilder.Append(ServicesDataConstants.SEARCH_EMAIL_PARAM);
            if (searchEmail != null)
            {
                paramsBuilder.Append(searchEmail);
            }

            paramsBuilder.Append(ServicesDataConstants.SEARCH_PHONE_PARAM);
            if (searchPhone != null)
            {
                paramsBuilder.Append(searchPhone);
            }

            if (searchName != null || searchEmail != null || searchPhone != null)
            {
                if (searchName != null)
                {
                    ordersListViewModel.Orders = ordersListViewModel.Orders
                        .Where(order => order.PickupName.ToLower().Contains(searchName.ToLower()))
                        .ToArray();
                }   
                else
                {
                    if (searchEmail != null)
                    {
                        ordersListViewModel.Orders = ordersListViewModel.Orders
                        .Where(order => order.Email.ToLower().Contains(searchEmail.ToLower()))
                        .ToArray();
                    }
                    else
                    {
                        ordersListViewModel.Orders = ordersListViewModel.Orders
                        .Where(order => order.PhoneNumber.Contains(searchPhone))
                        .ToArray();
                    }
                }
            }

            var totalItems = ordersListViewModel.Orders.Count();

            ordersListViewModel.Orders = ordersListViewModel.Orders
            .OrderBy(order => order.PickupTime)
            .Skip((productPage - 1) * ServicesDataConstants.PAGE_SIZE)
            .Take(ServicesDataConstants.PAGE_SIZE)
            .ToArray();

            ordersListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = ServicesDataConstants.PAGE_SIZE,
                TotalItems = totalItems,
                UrlParam = paramsBuilder.ToString()
            };

            return ordersListViewModel;
        }

        private async Task<Order> GetOrderByIdAsync(string orderId)
        {
            var order = await this.ordersRepository
                .All()
                .Include(ordr => ordr.User)
                .Include(ordr => ordr.OrderMenuItems)
                .ThenInclude(orderItem => orderItem.MenuItem)
                .SingleOrDefaultAsync(ordr => ordr.Id == orderId);

            if (order == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), orderId));
            }

            return order;
        }

        private async Task<Order> GetOrderByIdAndUserIdAsync(string orderId, string userId)
        {
            var order = await this.ordersRepository
                .All()
                .Include(ordr => ordr.User)
                .Include(ordr => ordr.OrderMenuItems)
                .ThenInclude(orderItem => orderItem.MenuItem)
                .SingleOrDefaultAsync(ordr => ordr.Id == orderId && ordr.UserId == userId);

            if (order == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Order), orderId));
            }

            return order;
        }

        private static OrdersListViewModel PopulateOrdersListViewModel(Order[] orders)
        {
            var ordersListViewModel = new OrdersListViewModel();

            foreach (var order in orders)
            {
                var orderViewModel = new OrderViewModel
                {
                    Id = order.Id,
                    Email = order.User.Email,
                    PickupName = order.PickupName,
                    PhoneNumber = order.PhoneNumber,
                    PickupTime = order.PickUpDateAndTime.ToString("dd/MM/yyyy HH:mm"),
                    OrderTotal = order.OrderTotal != default(decimal)
                        ? order.OrderTotal.ToString("C")
                        : order.OrderTotalOriginal.ToString("C"),
                    Status = order.OrderStatus.ToString(),
                    TotalItems = order.OrderMenuItems.Count
                };

                ordersListViewModel.Orders.Add(orderViewModel);
            }

            return ordersListViewModel;
        }

        private string GetUserEmail(string userId)
        {
            var user = this.usersRepository
                .All()
                .SingleOrDefault(usr => usr.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, ServicesDataConstants.USER, userId));
            }

            return user.Email;
        }
    }
}
