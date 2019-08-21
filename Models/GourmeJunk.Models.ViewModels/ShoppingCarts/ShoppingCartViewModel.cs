using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.ShoppingCarts
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel()
        {
            this.MenuItems = new HashSet<ShoppingCartMenuItemViewModel>();
        }

        public string UserId { get; set; }

        public string CouponName { get; set; }

        public decimal OrderTotalOriginal { get; set; }

        public decimal OrderTotal { get; set; }

        public string StatusMessage { get; set; }

        public IEnumerable<ShoppingCartMenuItemViewModel> MenuItems { get; set; }        
    }
}
