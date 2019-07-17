using GourmeJunk.Data.Models;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.MenuItems
{
    public class MenuItemViewModel : IMapFrom<MenuItem>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }
    }
}
