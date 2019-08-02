using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Models.ViewModels.SubCategories;
using System.Collections.Generic;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemEditViewModel : BaseMenuItemViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }        

        public IEnumerable<SubCategoryBaseViewModel> SubCategories { get; set; }

        public string StatusMessage { get; set; }
    }
}
