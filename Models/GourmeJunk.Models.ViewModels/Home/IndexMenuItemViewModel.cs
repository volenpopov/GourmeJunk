using GourmeJunk.Data.Models;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.Home
{
    public class IndexMenuItemViewModel : IMapFrom<MenuItem>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        public string SubCategoryName { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }
    }
}
