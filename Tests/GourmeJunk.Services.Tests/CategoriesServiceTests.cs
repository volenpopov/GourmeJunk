using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class CategoriesServiceTests : BaseServiceTests
    {
        private const string TEST_CATEGORY_NAME = "Beverages";
        private const string SECOND_TEST_CATEGORY_NAME = "Desserts";
        private const string TEST_MARKED_DELETED_CATEGORY_NAME = "Salads";

        private const string TEST_BEVERAGES_SUBCATEGORY_NAME = "Alcoholic";
        private const string SECOND_TEST_BEVERAGES_SUBCATEGORY_NAME = "NonAlcoholic";

        private ICategoriesService categoriesServiceMock
            => this.ServiceProvider.GetRequiredService<ICategoriesService>();

        [Fact]
        public async Task GetAllCategoriesViewModelsAsync_ReturnsAllCategoriesViewModels()
        {
            await this.AddTestingCategoriesToDb();

            var expected = new CategoryViewModel[]
            {
                new CategoryViewModel
                {
                    Name = TEST_CATEGORY_NAME
                },
                new CategoryViewModel
                {
                    Name = SECOND_TEST_CATEGORY_NAME
                },
            };

            var actual = await this.categoriesServiceMock.GetAllCategoriesViewModelsAsync();

            Assert.IsType<CategoryViewModel[]>(actual);
            Assert.Equal(expected.Length, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Name, elem2.Name);
                });            
        }        

        [Fact]
        public async Task GetAllCategoriesViewModelsAsync_ReturnsNone_WhenNoCategoriesExist()
        {
            var actual = await this.categoriesServiceMock.GetAllCategoriesViewModelsAsync();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task CheckIfCategoryExistsAsync_ReturnsTrueForExistingCategory()
        {
            await this.AddTestingCategoriesToDb();

            var actual = await this.categoriesServiceMock.CheckIfCategoryExistsAsync(TEST_CATEGORY_NAME);

            Assert.True(actual, "CheckIfCategoryExistsAsync failed - returns false for existing category.");
        }

        [Fact]
        public async Task CheckIfCategoryExistsAsync_ReturnsFalseForNonExistingCategory()
        {
            await this.AddTestingCategoriesToDb();

            var actual = await this.categoriesServiceMock.CheckIfCategoryExistsAsync(string.Empty);

            Assert.False(actual, "CheckIfCategoryExistsAsync failed - returns true for non-existing category.");
        }

        [Fact]
        public async Task CreateCategoryAsync_AddsCategoryToDb()
        {
            await this.categoriesServiceMock.CreateCategoryAsync(
                new CategoryCreateInputModel
                {
                    Name = TEST_CATEGORY_NAME
                });

            var expectedCount = 1;
            var actualCount = this.DbContext.Categories.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task CreateCategoryAsync_SuccessfullyCreatesNonExistingCategory()
        {
            await this.categoriesServiceMock.CreateCategoryAsync(
               new CategoryCreateInputModel
               {
                   Name = TEST_CATEGORY_NAME
               });

            var expected = TEST_CATEGORY_NAME;
            var actual = this.DbContext.Categories.FirstAsync().Result.Name;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateCategoryAsync_RestoresDeletedCategory_WhenANewOneWithTheSameNameIsGivenAsInput()
        {
            await this.AddTestingCategoriesToDb();

            var expectedCount = 2;

            var actual = await this.DbContext.Categories.FirstAsync();
            actual.IsDeleted = true;

            await this.categoriesServiceMock.CreateCategoryAsync(
                new CategoryCreateInputModel { Name = TEST_CATEGORY_NAME });

            var actualCount = this.DbContext.Categories.Count();

            Assert.False(actual.IsDeleted);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetCategoryModelByIdAsync_SuccessfullyReturnsModelById()
        {
            await this.AddTestingCategoriesToDb();

            var id = this.DbContext.Categories.Skip(1).FirstAsync().Result.Id;

            var expected = await this.DbContext.Categories.Skip(1).FirstAsync();
            var actual = await this.categoriesServiceMock.GetCategoryModelByIdAsync<CategoryViewModel>(id);

            Assert.IsType<CategoryViewModel>(actual);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public async Task GetCategoryModelByIdAsync_ThrowsNullReferenceException_IfCategoryNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.categoriesServiceMock.GetCategoryModelByIdAsync<CategoryViewModel>(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), string.Empty), exception.Message);
        }

        [Fact]
        public async Task EditCategoryAsync_ThrowsNullReferenceException_IfCurrentCategoryIsNotFound()
        {
            await this.AddTestingCategoriesToDb();

            var nonExistingCategory = new CategoryEditInputModel
            {
                Id = string.Empty
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.categoriesServiceMock.EditCategoryAsync(nonExistingCategory));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), nonExistingCategory.Id), exception.Message);
        }

        [Fact]
        public async Task EditCategoryAsync_SuccessfullyEditsCategory()
        {
            await this.AddTestingCategoriesToDb();

            var category = await this.DbContext.Categories.FirstAsync();

            var expectedName = "Edited";
            var editModel = new CategoryEditInputModel { Id = category.Id, Name = expectedName };

            await this.categoriesServiceMock.EditCategoryAsync(editModel);

            var actualName = category.Name;

            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ThrowsNullReferenceException_IfCategoryIsNotFound()
        {
            await this.AddTestingCategoriesToDb();

            var nonExistingCategory = new CategoryEditInputModel
            {
                Id = string.Empty
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.categoriesServiceMock.EditCategoryAsync(nonExistingCategory));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), nonExistingCategory.Id), exception.Message);
        }

        [Fact]
        public async Task GetCategoryNameByIdAsync_ReturnsProperCategoryName()
        {
            await this.AddTestingCategoriesToDb();

            var category = await this.DbContext.Categories.FirstAsync();
            var expectedName = category.Name;

            var actualName = await this.categoriesServiceMock.GetCategoryNameByIdAsync(category.Id);

            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public async Task GetCategoryNameByIdAsync_ThrowsNullReferenceException_IfCategoryIsNotFound()
        {
            await this.AddTestingCategoriesToDb();

            var nonExistingCategory = new CategoryEditInputModel
            {
                Id = string.Empty
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.categoriesServiceMock.EditCategoryAsync(nonExistingCategory));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), nonExistingCategory.Id), exception.Message);
        }

        [Fact]
        public async Task CheckContainsSubCategoryAsync_ReturnsTrueForExistingSubcategory()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            var category = await this.DbContext.Categories.FirstAsync();
            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var actual = await this.categoriesServiceMock.CheckContainsSubCategoryAsync(category.Id, subcategory.Id);

            Assert.True(actual);
        }

        [Fact]
        public async Task CheckContainsSubCategoryAsync_ReturnsFalseForNonExistingSubcategory()
        {
            await this.AddTestingCategoriesToDb();

            var category = await this.DbContext.Categories.FirstAsync();

            var actual = await this.categoriesServiceMock.CheckContainsSubCategoryAsync(category.Id, string.Empty);

            Assert.False(actual);
        }

        [Fact]
        public async Task CheckContainsSubCategoryAsync_ThrowsNullReferenceException_IfCategoryIsNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.categoriesServiceMock.CheckContainsSubCategoryAsync(string.Empty, string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), string.Empty), exception.Message);
        }

        private async Task AddTestingCategoriesToDb()
        {
            await this.DbContext.Categories.AddAsync(
                new Category
                {
                    Name = TEST_CATEGORY_NAME
                });

            await this.DbContext.Categories.AddAsync(
                new Category
                {
                    Name = SECOND_TEST_CATEGORY_NAME
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingCategoriesAndSubcategoriesToDb()
        {
            await this.AddTestingCategoriesToDb();

            var category = await this.DbContext.Categories.FirstAsync();

            await this.DbContext.SubCategories.AddAsync(
                new SubCategory
                {
                    Category = category,
                    Name = TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            await this.DbContext.SubCategories.AddAsync(
                new SubCategory
                {
                    Category = category,
                    Name = SECOND_TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            await this.DbContext.SaveChangesAsync();
        }
    }
}
