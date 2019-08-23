using System;
using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class OrderFullInfoViewModel
    {
        public OrderFullInfoViewModel()
        {
            this.OrderItems = new HashSet<OrderItemViewModel>();
        }

        public string Id { get; set; }

        public string PickupName { get; set; }

        public string PhoneNumber { get; set; }

        public decimal OrderTotalOriginal { get; set; }

        public decimal OrderTotal { get; set; }

        public DateTime PickUpDateAndTime { get; set; }

        public string Comments { get; set; }

        public string CouponName { get; set; }

        public string Status { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }
    }
}
