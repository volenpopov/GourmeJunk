using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Models.ViewModels.MenuItems;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IMenuItemsService
    {
        Task<IEnumerable<MenuItemViewModel>> GetAllMenuItemsViewModelsAsync();

        Task<MenuItemCreateViewModel> GetMenuItemCreateViewModelAsync();

        Task<bool> CheckIfMenuItemExistsAsync(string menuItemName);

        Task<bool> CheckIfMenuItemExistsAsync(string menuItemId, string menuItemName);

        Task CreateMenuItemAsync(MenuItemCreateInputModel model, IFormFile formFile);

        Task<MenuItemEditViewModel> GetMenuItemEditViewModelAsync(string id);

        Task EditMenuItemAsync(MenuItemEditInputModel model, IFormFile image);

        Task<MenuItemViewModelExtended> GetMenuItemViewModelExtendedAsync(string menuItemId);

        Task DeleteMenuItemAsync(string menuItemId);

        Task<IEnumerable<IndexMenuItemViewModel>> GetAllIndexMenuItemsModelsAsync();
    }
}
