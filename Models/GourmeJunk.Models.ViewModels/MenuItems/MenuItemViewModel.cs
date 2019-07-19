using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemViewModel : IMapFrom<MenuItem>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        [Display(Name = nameof(Category))]
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = nameof(SubCategory))]
        public string SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }
    }
}
