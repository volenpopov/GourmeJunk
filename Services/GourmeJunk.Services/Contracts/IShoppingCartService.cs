using GourmeJunk.Models.ViewModels.Home;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartViewModel> GetShoppingCartViewModelAsync(string menuItemId);
    }
}
