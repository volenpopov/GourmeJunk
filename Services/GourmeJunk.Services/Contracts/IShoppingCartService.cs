using GourmeJunk.Models.ViewModels.Home;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartViewModel> GetShoppingCartViewModelAsync(string menuItemId);

        Task UpdateShoppingCartAsync(string menuItemId, int itemCount, ClaimsIdentity user);
    }
}
