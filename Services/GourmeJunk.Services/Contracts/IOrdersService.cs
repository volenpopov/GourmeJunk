using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IOrdersService
    {
        OrderSummaryViewModel GetOrderSummaryViewModel(OrderSummaryInputModel model,
            string[] itemsIds, string[] itemsNames, string[] itemsPrices, string[] itemsCount, string pickupName);

        Task<string> CreateOrderAsync(OrderInputModel model, 
            string[] itemsIds, string[] itemsCount, string stripeEmail, string stripeToken);

        Task<OrderConfirmViewModel> GetOrderConfirmViewModelAsync(string orderId, string userId);

        Task<OrdersHistoryListViewModel> GetOrdersHistoryListViewModelAsync(string userId, int productPage);

        Task<string> GetOrderStatusAsync(string orderId, string userId);
    }
}
