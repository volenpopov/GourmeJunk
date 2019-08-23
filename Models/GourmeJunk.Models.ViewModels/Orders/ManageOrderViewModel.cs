using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class ManageOrderViewModel
    {
        public ManageOrderViewModel()
        {
            this.OrderItems = new HashSet<OrderItemViewModel>();
        }

        public string Id { get; set; }

        public string PickupTime { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public ICollection<OrderItemViewModel> OrderItems { get; set; }
    }
}
