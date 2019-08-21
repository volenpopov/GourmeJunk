using System;
using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class OrderSummaryViewModel
    {
        public OrderSummaryViewModel()
        {
            this.OrderItems = new HashSet<OrderItemViewModel>();
        }

        public string UserId { get; set; }

        public string PickupName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime PickupDate { get; set; }

        public DateTime PickupTime { get; set; }

        public string Comments { get; set; }

        public string CouponName { get; set; }

        public decimal OrderTotalOriginal { get; set; }

        public decimal OrderTotal { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }
    }
}
