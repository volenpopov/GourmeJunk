using GourmeJunk.Models.Common;
using GourmeJunk.Models.ViewModels.Categories;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemCreateViewModel : BaseMenuItemViewModel
    {        
        public string Description { get; set; }       

        public string Image { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }   

        public string StatusMessage { get; set; }
    }
}
