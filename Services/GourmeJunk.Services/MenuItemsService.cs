﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class MenuItemsService : IMenuItemsService
    {
        private readonly IDeletableEntityRepository<MenuItem> menuItemsRepository;
        private readonly ICategoriesService categoriesService;
        private readonly ISubCategoriesService subCategoriesService;

        public MenuItemsService(
            IDeletableEntityRepository<MenuItem> menuItemsRepository,
            ICategoriesService categoriesService,
            ISubCategoriesService subCategoriesService)
        {
            this.menuItemsRepository = menuItemsRepository;
            this.categoriesService = categoriesService;
            this.subCategoriesService = subCategoriesService;
        }

        public async Task<IEnumerable<MenuItemViewModel>> GetAllAsync()
        {
            var menuItemViewModels = await this.menuItemsRepository
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

        public async Task<bool> CheckIfMenuItemExistsAsync(string menuItemName)
        {
            return await this.menuItemsRepository
                .AllAsNoTracking()
                .AnyAsync(menuItem => menuItem.Name == menuItemName);
        }

        public async Task CreateMenuItemAsync(MenuItemCreateInputModel model, IFormFile image)
        {
            var menuItem = await this.menuItemsRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(item => item.Name == model.Name);

            if (menuItem == null)
            {
                menuItem = this.InitializeMenuItem(model);

                await this.menuItemsRepository.AddAsync(menuItem);

                if (image != null)
                {
                    menuItem.Image = this.UploadImage(image, menuItem.Id);
                }
                else
                {
                    menuItem.Image = ServicesDataConstants.MENUITEM_DEFAULT_IMG_PATH;
                }                                   
            }
            else if (menuItem.IsDeleted)
            {
                this.menuItemsRepository.Undelete(menuItem);
            }

            await this.menuItemsRepository.SaveChangesAsync();
        }

        public async Task<MenuItemEditViewModel> GetMenuItemEditViewModelAsync(string menuItemId)
        {
            var menuItem = await this.GetMenuItemByIdAsync(menuItemId);

            var categories = await this.categoriesService.GetAllAsync();
            var subCategories = await this.subCategoriesService.GetSubCategoriesOfACategoryAsync(menuItem.CategoryId);

            var menuItemEditViewModel = new MenuItemEditViewModel
            {
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Image = menuItem.Image,
                Categories = categories,
                SubCategories = subCategories,
                CategoryId = menuItem.CategoryId,
                SubCategoryId = menuItem.SubCategoryId                
            };

            return menuItemEditViewModel;
        }

        private async Task<MenuItem> GetMenuItemByIdAsync(string menuItemId)
        {
            var menuItem = await this.menuItemsRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(item => item.Id == menuItemId);

            if (menuItem == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), menuItemId));
            }

            return menuItem;
        }

        private MenuItem InitializeMenuItem(MenuItemCreateInputModel model)
        {
            return new MenuItem
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                SubCategoryId = model.SubCategoryId == ServicesDataConstants.NO_SUBCATEGORY_SELECTED_DEFAULT_VALUE
                        ? null
                        : model.SubCategoryId
            };
        }

        private string UploadImage(IFormFile image, string menuItemId)
        {
            var extension = Path.GetExtension(image.FileName);

            if (extension != ServicesDataConstants.JPG_EXTENSION && extension != ServicesDataConstants.PNG_EXTENSION)
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.INVALID_IMG_TYPE, extension));
            }

            var fullPath = Path.Combine(ServicesDataConstants.MENUITEMS_IMGS_PATH, $"img_{menuItemId + extension}");
            
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(fileStream);                        
            }

            //SKIP "wwwroot"
            var imgPath = string.Join("", fullPath.Skip(7));

            return imgPath;
        }
    }
}
