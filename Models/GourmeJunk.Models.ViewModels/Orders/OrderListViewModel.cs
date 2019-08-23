using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class OrdersListViewModel
    {
        public OrdersListViewModel()
        {
            this.Orders = new HashSet<OrderViewModel>();
        }

        public ICollection<OrderViewModel> Orders { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
