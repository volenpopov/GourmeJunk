using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Models.ViewModels.MenuItems;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class MenuItemsServiceTests : BaseServiceTests
    {
        private IMenuItemsService menuItemsServiceMock
            => this.ServiceProvider.GetRequiredService<IMenuItemsService>();

        private const string TEST_MENUITEM_NAME = "Lasagna";
        private const decimal TEST_MENUITEM_PRICE = 10m;

        private const string TEST_IMAGE_URL = @"images\MenuItems\lasagna.jpg";

        private const string SECOND_TEST_MENUITEM_NAME = "Spaghetti";
        private const decimal SECOND_TEST_MENUITEM_PRICE = 7m;

        private const string NEW_MENUITEM_NAME = "Pizza";
        private const decimal NEW_MENUITEM_PRICE = 5m;
        private const string NEW_MENUITEM_DESCRIPTION = "Best pizza ever.";

        private const string TEST_CATEGORY_NAME = "Main";
        private const string TEST_SUBCATEGORY_NAME = "TestSubcategory";

        private const string MENUITEM_DEFAULT_IMAGE = @"\images\MenuItems\img_default.jpg";

        [Fact]
        public async Task GetAllMenuItemsViewModelsAsync_ReturnsValidModels()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var firstMenuItem = await this.DbContext.MenuItems.FirstAsync();
            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            var actual = await this.menuItemsServiceMock.GetAllMenuItemsViewModelsAsync();

            var expectedCount = this.DbContext.MenuItems.Count();

            Assert.IsType<MenuItemViewModel[]>(actual);
            Assert.Equal(expectedCount, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(firstMenuItem.Id, elem1.Id);
                    Assert.Equal(firstMenuItem.Name, elem1.Name);
                    Assert.Equal(firstMenuItem.Price, elem1.Price);
                    Assert.Equal(firstMenuItem.CategoryId, elem1.CategoryId);
                    Assert.Equal(firstMenuItem.SubCategoryId, elem1.SubCategoryId);
                },
                elem2 =>
                {
                    Assert.Equal(secondMenuItem.Id, elem2.Id);
                    Assert.Equal(secondMenuItem.Name, elem2.Name);
                    Assert.Equal(secondMenuItem.Price, elem2.Price);
                    Assert.Equal(secondMenuItem.CategoryId, elem2.CategoryId);
                    Assert.Equal(secondMenuItem.SubCategoryId, elem2.SubCategoryId);
                });
        }

        [Fact]
        public async Task GetAllMenuItemsViewModelsAsync_ReturnsZeroWhenNoMenuItemsExist()
        {
            var actual = await this.menuItemsServiceMock.GetAllMenuItemsViewModelsAsync();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMenuItemCreateViewModelAsync_ReturnsValidModels()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var expected = await this.DbContext.Categories
                .Select(category => new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name
                })
                .ToArrayAsync();

            var actual = await this.menuItemsServiceMock.GetMenuItemCreateViewModelAsync();

            Assert.IsType<MenuItemCreateViewModel>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected.First().Id, actual.Categories.First().Id);
            Assert.Equal(expected.First().Name, actual.Categories.First().Name);
        }

        [Fact]
        public async Task GetMenuItemCreateViewModelAsync_ReturnsEmptyModelWhenThereAreNoExistingCategories()
        {
            var expected = new CategoryViewModel[] { };

            var actual = await this.menuItemsServiceMock.GetMenuItemCreateViewModelAsync();

            Assert.IsType<MenuItemCreateViewModel>(actual);
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Categories);
        }

        [Fact]
        public async Task CheckIfMenuItemExistsAsync_ReturnsTrueForExistingItem()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var firstMenuItem = await this.DbContext.MenuItems.FirstAsync();
            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            var firstActual = await this.menuItemsServiceMock.CheckIfMenuItemExistsAsync(firstMenuItem.Name);

            Assert.True(firstActual);

            //Renaming 2nd item to the first one, so now both items have the same name:
            secondMenuItem.Name = firstMenuItem.Name;
            await this.DbContext.SaveChangesAsync();

            var secondActual = await this.menuItemsServiceMock.CheckIfMenuItemExistsAsync(firstMenuItem.Id, firstMenuItem.Name);

            Assert.True(secondActual);
        }

        [Fact]
        public async Task CheckIfMenuItemExistsAsync_ReturnsFalseForNonExistingItem()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var firstMenuItem = await this.DbContext.MenuItems.FirstAsync();
            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            var firstActual = await this.menuItemsServiceMock.CheckIfMenuItemExistsAsync(string.Empty);
            var secondActual = await this.menuItemsServiceMock.CheckIfMenuItemExistsAsync(string.Empty, string.Empty);

            secondMenuItem.Name = firstMenuItem.Name;
            await this.DbContext.SaveChangesAsync();

            var thirdActual = await this.menuItemsServiceMock.CheckIfMenuItemExistsAsync(firstMenuItem.Id, string.Empty);

            Assert.False(firstActual);
            Assert.False(secondActual);
            Assert.False(thirdActual);
        }

        [Fact]
        public async Task CreateMenuItemAsync_SuccessfullyCreatesNewItemWithDefaultImage()
        {
            await this.DbContext.Categories.AddAsync(
                new Category
                {
                    Name = TEST_CATEGORY_NAME
                });

            await this.DbContext.SaveChangesAsync();

            var category = await this.DbContext.Categories.FirstAsync();

            var menuItemInputModel = new MenuItemCreateInputModel
            {
                Name = TEST_CATEGORY_NAME,
                Price = TEST_MENUITEM_PRICE,
                CategoryId = category.Id
            };

            await this.menuItemsServiceMock.CreateMenuItemAsync(menuItemInputModel, null);

            var actual = await this.DbContext.MenuItems.FirstAsync();

            Assert.NotEmpty(this.DbContext.MenuItems);
            Assert.Equal(menuItemInputModel.Name, actual.Name);
            Assert.Equal(menuItemInputModel.Price, actual.Price);
            Assert.Equal(menuItemInputModel.CategoryId, actual.CategoryId);
            Assert.Equal(MENUITEM_DEFAULT_IMAGE, actual.Image);
        }

        [Fact]
        public async Task GetMenuItemEditViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            var category = await this.DbContext.Categories.FirstAsync();
            var subCategory = await this.DbContext.SubCategories.FirstAsync();

            var expectedCategories = new CategoryViewModel[]
            {
                new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name
                }
            };

            var actual = await this.menuItemsServiceMock.GetMenuItemEditViewModelAsync(menuItem.Id);

            Assert.IsType<MenuItemEditViewModel>(actual);
            Assert.Equal(menuItem.Name, actual.Name);
            Assert.Equal(menuItem.Description, actual.Description);
            Assert.Equal(menuItem.Price, actual.Price);
            Assert.Equal(menuItem.Image, actual.Image);
            Assert.Equal(menuItem.CategoryId, actual.CategoryId);
            Assert.Equal(menuItem.SubCategoryId, actual.SubCategoryId);

            Assert.Collection(actual.Categories,
                elem1 =>
                {
                    Assert.Equal(expectedCategories.First().Id, elem1.Id);
                    Assert.Equal(expectedCategories.First().Name, elem1.Name);
                });

            Assert.Equal(subCategory.Id, actual.SubCategories.First().Id);
            Assert.Equal(subCategory.Name, actual.SubCategories.First().Name);
        }

        [Fact]
        public async Task GetMenuItemEditViewModelAsync_ThrowsNullReferenceIfItemNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.menuItemsServiceMock.GetMenuItemEditViewModelAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), string.Empty), exception.Message);
        }

        [Fact]
        public async Task EditMenuItemAsync_SucessfullyEditsMenuItemWithoutImageAndSetsItImageToDefault()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();
            
            //Delete image
            menuItem.Image = null;
            await this.DbContext.SaveChangesAsync();

            var category = await this.DbContext.Categories.FirstAsync();
            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var menuItemEditInputModel = new MenuItemEditInputModel
            {
                Id = menuItem.Id,
                Name = NEW_MENUITEM_NAME,
                Description = NEW_MENUITEM_DESCRIPTION,
                Price = NEW_MENUITEM_PRICE,
                CategoryId = category.Id,
                SubCategoryId = subcategory.Id             
            };

            await this.menuItemsServiceMock.EditMenuItemAsync(menuItemEditInputModel, null);

            Assert.Equal(NEW_MENUITEM_NAME, menuItem.Name);
            Assert.Equal(NEW_MENUITEM_PRICE, menuItem.Price);
            Assert.Equal(NEW_MENUITEM_DESCRIPTION, menuItem.Description);
            Assert.Equal(category.Id, menuItem.CategoryId);
            Assert.Equal(subcategory.Id, menuItem.SubCategoryId);
            Assert.Equal(MENUITEM_DEFAULT_IMAGE, menuItem.Image);
        }

        [Fact]
        public async Task EditMenuItemAsync_DeletesCurrentItemAndRestoresPreviouslyDeletdItemWhenPreviouslyDeletedItemIsGivenAsInput()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();

            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            //Delete 2nd item
            secondMenuItem.IsDeleted = true;
            secondMenuItem.DeletedOn = DateTime.UtcNow;

            await this.DbContext.SaveChangesAsync();

            var category = await this.DbContext.Categories.FirstAsync();
            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var menuItemEditInputModel = new MenuItemEditInputModel
            {
                Id = menuItem.Id,
                Name = SECOND_TEST_MENUITEM_NAME,
                Description = NEW_MENUITEM_DESCRIPTION,
                Price = NEW_MENUITEM_PRICE,
                CategoryId = category.Id,
                SubCategoryId = subcategory.Id
            };

            await this.menuItemsServiceMock.EditMenuItemAsync(menuItemEditInputModel, null);

            Assert.True(menuItem.IsDeleted);
            Assert.NotNull(menuItem.DeletedOn);

            Assert.False(secondMenuItem.IsDeleted);
            Assert.Null(secondMenuItem.DeletedOn);

            Assert.Equal(SECOND_TEST_MENUITEM_NAME, secondMenuItem.Name);
            Assert.Equal(NEW_MENUITEM_DESCRIPTION, secondMenuItem.Description);
            Assert.Equal(NEW_MENUITEM_PRICE, secondMenuItem.Price);
            Assert.Equal(category.Id, secondMenuItem.CategoryId);
            Assert.Equal(subcategory.Id, secondMenuItem.SubCategoryId);
        }


        [Fact]
        public async Task EditMenuItemAsync_ThrowsNullReferenceIfItemNotFound()
        {
            var menuItemEditInputModel = new MenuItemEditInputModel
            { Id = string.Empty };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
               this.menuItemsServiceMock.EditMenuItemAsync(menuItemEditInputModel, null));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), menuItemEditInputModel.Id), exception.Message);
        }

        [Fact]
        public async Task GetMenuItemViewModelExtendedAsync_ReturnsValidModel()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var firstMenuItem = await this.DbContext.MenuItems.FirstAsync();
            firstMenuItem.SubCategoryId = subcategory.Id;

            var secondMenuItem = await this.DbContext.MenuItems.LastAsync();

            await this.DbContext.SaveChangesAsync();

            var firstActual = await this.menuItemsServiceMock.GetMenuItemViewModelExtendedAsync(firstMenuItem.Id);
            var secondActual = await this.menuItemsServiceMock.GetMenuItemViewModelExtendedAsync(secondMenuItem.Id);

            Assert.IsType<MenuItemViewModelExtended>(firstActual);
            Assert.Equal(firstMenuItem.Id, firstActual.Id);
            Assert.Equal(firstMenuItem.Name, firstActual.Name);
            Assert.Equal(firstMenuItem.Description, firstActual.Description);
            Assert.Equal(firstMenuItem.Price, firstActual.Price);
            Assert.Equal(firstMenuItem.Image, firstActual.Image);
            Assert.Equal(firstMenuItem.CategoryId, firstActual.CategoryId);
            Assert.Equal(firstMenuItem.SubCategoryId, firstActual.SubCategoryId);
            Assert.Equal(TEST_CATEGORY_NAME, firstActual.CategoryName);
            Assert.Equal(TEST_SUBCATEGORY_NAME, firstActual.SubCategoryName);

            Assert.Null(secondActual.SubCategoryName);
        }

        [Fact]
        public async Task GetMenuItemViewModelExtendedAsync_ThrowsNullReferenceIfItemNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
               this.menuItemsServiceMock.GetMenuItemViewModelExtendedAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), string.Empty), exception.Message);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_SuccessfullyMarksItemAsDeletedAndSetsImageToNull()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var menuItem = await this.DbContext.MenuItems.FirstAsync();

            Assert.False(menuItem.IsDeleted);
            Assert.Null(menuItem.DeletedOn);
            Assert.NotNull(menuItem.Image);

            await this.menuItemsServiceMock.DeleteMenuItemAsync(menuItem.Id);

            Assert.True(menuItem.IsDeleted);
            Assert.NotNull(menuItem.DeletedOn);
            Assert.Null(menuItem.Image);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_ThrowsNullReferenceWhenItemNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
               this.menuItemsServiceMock.DeleteMenuItemAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(MenuItem), string.Empty), exception.Message);
        }

        [Fact]
        public async Task GetAllIndexMenuItemsModelsAsync_ReturnsValidModels()
        {
            await this.AddTestingMenuItemsWithCategoryToDb();

            var menuItems = await this.DbContext.MenuItems.ToArrayAsync();

            var actual = await this.menuItemsServiceMock.GetAllIndexMenuItemsModelsAsync();

            Assert.IsType<IndexMenuItemViewModel[]>(actual);
            Assert.NotEmpty(actual);
            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(menuItems.First().Id, elem1.Id);
                    Assert.Equal(menuItems.First().Name, elem1.Name);
                    Assert.Equal(menuItems.First().Price, elem1.Price);
                    Assert.Equal(menuItems.First().Description, elem1.Description);
                    Assert.Equal(TEST_CATEGORY_NAME, elem1.CategoryName);
                    Assert.Null(elem1.SubCategoryName);
                    Assert.Equal(menuItems.First().Image, elem1.Image);
                },
                elem2 =>
                {
                    Assert.Equal(menuItems.Last().Id, elem2.Id);
                    Assert.Equal(menuItems.Last().Name, elem2.Name);
                    Assert.Equal(menuItems.Last().Price, elem2.Price);
                    Assert.Equal(menuItems.Last().Description, elem2.Description);
                    Assert.Equal(TEST_CATEGORY_NAME, elem2.CategoryName);
                    Assert.Null(elem2.SubCategoryName);
                    Assert.Null(elem2.Image);
                });
        }

        [Fact]
        public async Task GetAllIndexMenuItemsModelsAsync_ReturnsNoneWhenNoItemsExist()
        {
            var actual = await this.menuItemsServiceMock.GetAllIndexMenuItemsModelsAsync();

            Assert.Empty(actual);
        }

        private async Task AddTestingMenuItemsWithCategoryToDb()
        {
            await this.DbContext.Categories.AddAsync(
                new Category
                {
                    Name = TEST_CATEGORY_NAME
                });

            await this.DbContext.SaveChangesAsync();

            var category = await this.DbContext.Categories.FirstAsync();

            await this.DbContext.SubCategories.AddAsync(
               new SubCategory
               {
                   Name = TEST_SUBCATEGORY_NAME,
                   CategoryId = category.Id
               });
            
            await this.DbContext.MenuItems.AddAsync(
                new MenuItem
                {
                    Name = TEST_MENUITEM_NAME,
                    Price = TEST_MENUITEM_PRICE,
                    Image = TEST_IMAGE_URL,
                    CategoryId = category.Id
                });

            await this.DbContext.MenuItems.AddAsync(
                new MenuItem
                {
                    Name = SECOND_TEST_MENUITEM_NAME,
                    Price = SECOND_TEST_MENUITEM_PRICE,
                    CategoryId = category.Id
                });

            await this.DbContext.SaveChangesAsync();
        }        
    }
}
