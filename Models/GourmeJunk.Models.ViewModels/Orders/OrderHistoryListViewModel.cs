using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class OrdersHistoryListViewModel
    {
        public OrdersHistoryListViewModel()
        {
            this.Orders = new List<OrderHistoryViewModel>();
        }

        public IList<OrderHistoryViewModel> Orders { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
