using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class ManageOrdersListViewModel
    {
        public ManageOrdersListViewModel()
        {
            this.Orders = new HashSet<ManageOrderViewModel>();
        }

        public ICollection<ManageOrderViewModel> Orders { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
