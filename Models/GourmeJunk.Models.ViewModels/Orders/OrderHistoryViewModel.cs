using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.Orders
{
    public class OrderHistoryViewModel
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.Order.PICKUPNAME_DISPLAY)]
        public string PickupName { get; set; }

        public string Email { get; set; }

        [Display(Name = ModelConstants.Order.PICKUPTIME_DISPLAY)]
        public string PickupTime { get; set; }

        [Display(Name = ModelConstants.Order.ORDERTOTAL_DISPLAY)]
        public string OrderTotal { get; set; }

        [Display(Name = ModelConstants.Order.TOTALITEMS_DISPLAY)]
        public int TotalItems { get; set; }

        public string Status { get; set; }
    }
}
