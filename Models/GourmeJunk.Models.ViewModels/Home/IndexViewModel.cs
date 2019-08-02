using GourmeJunk.Models.ViewModels.Categories;
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
