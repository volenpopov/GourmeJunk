using GourmeJunk.Models.ViewModels.SubCategories;
using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemEditViewModel : MenuItemCreateViewModel
    {
        public string Id { get; set; }

        public IEnumerable<SubCategoryBaseViewModel> SubCategories { get; set; }
    }
}
