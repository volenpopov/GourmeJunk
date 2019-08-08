using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.Home
{
    public class ShoppingCartViewModel : IMapFrom<ShoppingCart>
    {
        public ShoppingCartViewModel()
        {
            this.Count = 1;
        }

        public int Count { get; set; }

        public MenuItemViewModelExtended MenuItem { get; set; }
    }
}
