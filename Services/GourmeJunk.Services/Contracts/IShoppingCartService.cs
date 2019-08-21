using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Models.ViewModels.ShoppingCarts;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartMenuItemDetailsViewModel> GetShoppingCartMenuItemDetailsViewModelAsync(string menuItemId);

        Task UpdateShoppingCartAsync(string menuItemId, int itemCount, ClaimsIdentity user);

        Task<ShoppingCartViewModel> GetShoppingCartViewModelAsync(string userId);

        Task<ShoppingCartViewModel> ApplyCouponToCartAsync(ShoppingCartViewModel model);

        Task IncrementItemQuantityAsync(string userId, string shoppingCartId);

        Task DecrementItemQuantityAsync(string userId, string shoppingCartId);

        Task RemoveItemAsync(string userId, string shoppingCartId);

        Task<int> GetUserIndividualItemsCountAsync(string userId);

        Task<bool> CheckIfItemIsLastAndIsToBeRemovedAsync(string userId, string menuItemId);
    }
}
