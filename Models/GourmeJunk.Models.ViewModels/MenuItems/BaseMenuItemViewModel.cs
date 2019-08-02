using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class BaseMenuItemViewModel : IMapFrom<MenuItem>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        [Display(Name = nameof(ModelConstants.Category))]
        public string CategoryId { get; set; }

        [Display(Name = nameof(ModelConstants.SubCategory))]
        public string SubCategoryId { get; set; }
    }
}
