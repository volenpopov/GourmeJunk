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

        public MenuItemsService(IDeletableEntityRepository<MenuItem> menuItems)
        {
            this.menuItems = menuItems;
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
    }
}
