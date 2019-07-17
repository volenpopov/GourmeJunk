using GourmeJunk.Models.Common;
using GourmeJunk.Models.ViewModels.Categories;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemCreateViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        [Display(Name = nameof(ModelConstants.Category))]
        public string CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        [Display(Name = nameof(ModelConstants.SubCategory))]
        public string SubCategoryId { get; set; }       

        public string StatusMessage { get; set; }
    }
}
