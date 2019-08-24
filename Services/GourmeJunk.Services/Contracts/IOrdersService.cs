using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels.Orders;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IOrdersService
    {
        OrderSummaryViewModel GetOrderSummaryViewModel(OrderSummaryInputModel model,
            string[] itemsIds, string[] itemsNames, string[] itemsPrices, string[] itemsCount, string pickupName);

        Task<string> CreateOrderAsync(OrderInputModel model, 
            string[] itemsIds, string[] itemsCount, string stripeEmail, string stripeToken);

        Task<OrderFullInfoViewModel> GetOrderFullInfoViewModelAsync(string orderId, string userId);

        Task<OrdersListViewModel> GetOrdersHistoryListViewModelAsync(string userId, int productPage);

        Task<string> GetOrderStatusAsync(string orderId, string userId);

        Task<ManageOrdersListViewModel> GetManageOrdersListViewModelAsync(int productPage);

        Task UpdateOrderStatusToCookingAsync(string orderId);

        Task UpdateOrderStatusToReadyAsync(string orderId);

        Task UpdateOrderStatusToCancelledAsync(string orderId, string userId);

        Task UpdateOrderStatusToDeliveredAsync(string orderId);

        Task<OrdersListViewModel> GetOrdersListViewModelAsync(int productPage, string userId);

        Task<OrdersListViewModel> GetOrdersPickupListViewModelAsync(int productPage, string searchEmail, string searchPhone, string searchName);
    }
}
