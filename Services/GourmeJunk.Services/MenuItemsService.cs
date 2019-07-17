using System.Collections.Generic;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class MenuItemsService : IMenuItemsService
    {
        private readonly IDeletableEntityRepository<MenuItem> menuItems;
        private readonly ICategoriesService categoriesService;

        public MenuItemsService(
            IDeletableEntityRepository<MenuItem> menuItems,
            ICategoriesService categoriesService)
        {
            this.menuItems = menuItems;
            this.categoriesService = categoriesService;
        }

        public async Task<IEnumerable<MenuItemViewModel>> GetAllAsync()
        {
            var menuItemViewModels = await this.menuItems
                .AllAsNoTracking()
                .Include(menuItem => menuItem.Category)
                .Include(menuItem => menuItem.SubCategory)
                .To<MenuItemViewModel>()
                .ToArrayAsync();

            return menuItemViewModels;
        }

        public async Task<MenuItemCreateViewModel> GetMenuItemCreateViewModel()
        {
            var categories = await this.categoriesService.GetAllAsync();

            var menuItemCreateViewModel = new MenuItemCreateViewModel
            {
                Categories = categories,                
            };

            return menuItemCreateViewModel;
        }
    }
}
