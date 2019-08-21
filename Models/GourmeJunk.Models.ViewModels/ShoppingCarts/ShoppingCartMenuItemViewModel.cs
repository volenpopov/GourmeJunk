using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.ShoppingCarts
{
    public class ShoppingCartMenuItemDetailsViewModel : IMapFrom<ShoppingCart>
    {
        public ShoppingCartMenuItemDetailsViewModel()
        {
            this.Count = 1;
        }

        public int Count { get; set; }

        public MenuItemViewModelExtended MenuItem { get; set; }
    }
}
