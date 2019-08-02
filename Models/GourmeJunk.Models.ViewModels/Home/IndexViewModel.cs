using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Models.ViewModels.MenuItems;
using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<IndexMenuItemViewModel> MenuItems { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        public IEnumerable<IndexCouponViewModel> Coupons { get; set; }
    }
}
