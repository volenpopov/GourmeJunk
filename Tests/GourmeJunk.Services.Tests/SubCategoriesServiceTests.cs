using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.SubCategories;
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
    public class SubCategoriesServiceTests : BaseServiceTests
    {
        private ISubCategoriesService subCategoriesServiceMock
            => this.ServiceProvider.GetRequiredService<ISubCategoriesService>();

        private ICategoriesService categoriesServiceMock
            => this.ServiceProvider.GetRequiredService<ICategoriesService>();

        private const string TEST_BEVERAGES_SUBCATEGORY_NAME = "Alcoholic";
        private const string SECOND_TEST_BEVERAGES_SUBCATEGORY_NAME = "NonAlcoholic";
        private const string THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME = "Tea";

        private const string TEST_CATEGORY_NAME = "Beverages";
        private const string SECOND_TEST_CATEGORY_NAME = "Desserts";

        [Fact]
        public async Task GetAllSubCategoriesViewModelsAsync_ReturnsAllSubCategoriesViewModels()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            var expected = new SubCategoryViewModel[]
            {
                new SubCategoryViewModel
                {
                    CategoryName = TEST_CATEGORY_NAME,
                    Name = TEST_BEVERAGES_SUBCATEGORY_NAME
                },
                new SubCategoryViewModel
                {
                    CategoryName = TEST_CATEGORY_NAME,
                    Name = SECOND_TEST_BEVERAGES_SUBCATEGORY_NAME
                }
            };

            var actual = await this.subCategoriesServiceMock.GetAllSubCategoriesViewModelsAsync();

            Assert.IsType<SubCategoryViewModel[]>(actual);
            Assert.Equal(expected.Length, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].CategoryName, elem1.CategoryName);
                    Assert.Equal(expected[0].Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].CategoryName, elem2.CategoryName);
                    Assert.Equal(expected[1].Name, elem2.Name);
                });
        }

        [Fact]
        public async Task GetAllSubCategoriesViewModelsAsync_ReturnsNone_WhenNoSubCategoriesExist()
        {
            var actual = await this.subCategoriesServiceMock.GetAllSubCategoriesViewModelsAsync();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task CheckIfCategorySubCategoryPairExistsAsync_ReturnsTrueForExistingPair()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            var alcoholicSubcategory = await this.DbContext.SubCategories.FirstAsync();
            var nonAlcoholicSubcategory = await this.DbContext.SubCategories.LastAsync();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Beverages - Alcoholic
            var firstActual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(alcoholicSubcategory.Name, category.Id);

            //Beverages - NonAlcoholic
            var secondActual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(nonAlcoholicSubcategory.Name, category.Id);

            Assert.True(firstActual);
            Assert.True(secondActual);
        }

        [Fact]
        public async Task CheckIfCategorySubCategoryPairExistsAsync_ReturnsFalseForNonExistingPair()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Alcoholic
            var subCategory = await this.DbContext.SubCategories.FirstAsync();

            //Desserts
            var category = await this.DbContext.Categories.LastAsync();

            //Desserts - Alcoholic
            var actual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(subCategory.Name, category.Id);

            var secondActual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(subCategory.Name, string.Empty);
            var thirdActual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(string.Empty, category.Id);
            var fourthActual = await this.subCategoriesServiceMock.CheckIfCategorySubCategoryPairExistsAsync(string.Empty, string.Empty);

            Assert.False(actual);
            Assert.False(secondActual);
            Assert.False(thirdActual);
            Assert.False(fourthActual);
        }

        [Fact]
        public async Task GetSubCategoryCreateViewModelAsync_ReturnsValidModel()
        {
            var categoriesViewModels = await this.categoriesServiceMock.GetAllCategoriesViewModelsAsync();

            var firstActual = this.subCategoriesServiceMock.GetSubCategoryCreateViewModelAsync(categoriesViewModels);
            var secondActual = this.subCategoriesServiceMock.GetSubCategoryCreateViewModelAsync(null);

            Assert.IsType<SubCategoryCreateViewModel>(firstActual);
            Assert.Equal(categoriesViewModels, firstActual.CategoryList);
            Assert.Null(secondActual.CategoryList);
        }

        [Fact]
        public async Task CreateSubCategoryAsync_SuccessfullyAddsSubCategoryToDb()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Creating non-existing pair Beverages-Tea
            await this.subCategoriesServiceMock.CreateSubCategoryAsync(
                new SubCategoryCreateInputModel
                {
                    CategoryId = category.Id,
                    Name = THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            var expectedCount = 3;
            var actualCount = this.DbContext.SubCategories.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task CreateSubCategoryAsync_SuccessfullyCreatesNonExistingPairSubcategoryCategory()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Creating non-existing pair Beverages-Tea
            await this.subCategoriesServiceMock.CreateSubCategoryAsync(
                new SubCategoryCreateInputModel
                {
                    CategoryId = category.Id,
                    Name = THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            var actualSubcategory = await this.DbContext.SubCategories.LastAsync();

            var expectedSubcategoryName = THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME;
            var actualSubcategoryName = actualSubcategory.Name;

            var expectedCategoryId = category.Id;
            var actualCategoryId = actualSubcategory.CategoryId;

            Assert.Equal(expectedSubcategoryName, actualSubcategoryName);
            Assert.Equal(expectedCategoryId, actualCategoryId);
            Assert.Equal(category, actualSubcategory.Category);
        }

        [Fact]
        public async Task CreateSubCategoryAsync_RestoresPreviouslyDeletedPair_WhenGivenAnExistingDeletedPairSubcategoryCategoryAsInput()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Delete Beverages-Alcoholic pair
            var subCategory = await this.DbContext.SubCategories.FirstAsync();
            subCategory.IsDeleted = true;

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Recreating Beverages-Alcoholic pair
            await this.subCategoriesServiceMock.CreateSubCategoryAsync(
                new SubCategoryCreateInputModel
                {
                    CategoryId = category.Id,
                    Name = TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            var expectedCount = 2;
            var actualCount = this.DbContext.SubCategories.Count();

            Assert.False(subCategory.IsDeleted);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task CreateSubCategoryAsync_DoesntCreatePairSubcategoryCategory_WhenGivenAnExistingPair()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Trying to add an already existing pair: Beverages-Alcoholic
            await this.subCategoriesServiceMock.CreateSubCategoryAsync(
                new SubCategoryCreateInputModel
                {
                    CategoryId = category.Id,
                    Name = TEST_BEVERAGES_SUBCATEGORY_NAME
                });

            var expectedCount = 2;
            var actualCount = this.DbContext.SubCategories.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetSubCategoryEditViewModelAsync_ReturnsValidModel()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var actual = await this.subCategoriesServiceMock.GetSubCategoryEditViewModelAsync(subcategory.Id);

            Assert.IsType<SubCategoryEditViewModel>(actual);
            Assert.Equal(subcategory.Id, actual.Id);
            Assert.Equal(subcategory.Name, actual.Name);
            Assert.Equal(subcategory.CategoryId, actual.CategoryId);
            Assert.Equal(subcategory.Category.Name, actual.CategoryName);
        }

        [Fact]
        public async Task GetSubCategoryEditViewModelAsync_ThrowsNullReference_WhenSubcategoryNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.subCategoriesServiceMock.GetSubCategoryEditViewModelAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(SubCategory), string.Empty), exception.Message);
        }

        [Fact]
        public async Task EditSubCategoryAsync_SuccessfullyEditsExistingSubcategory()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages-Alcoholic
            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var editInputModel = new SubCategoryEditInputModel
            {
                Id = subcategory.Id,
                //Renaming to Beverages-Tea
                Name = THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME
            };

            await this.subCategoriesServiceMock.EditSubCategoryAsync(editInputModel);

            Assert.Equal(THIRD_TEST_BEVERAGES_SUBCATEGORY_NAME, subcategory.Name);
        }

        [Fact]        
        public async Task EditSubCategoryAsync_SuccessfullyDeletesCurrentPairAndUndeletesTheNewDeletedPairGivenAsInput()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            //Beverages-Alcoholic
            var firstSubcategory = await this.DbContext.SubCategories.FirstAsync();

            //Beverages-NonAlcoholic
            var secondSubcategory = await this.DbContext.SubCategories.LastAsync();
            secondSubcategory.IsDeleted = true;

            await this.DbContext.SaveChangesAsync();

            var editInputModel = new SubCategoryEditInputModel
            {
                //Trying to edit Beverages-Alcoholic to Beverages-NonAlcoholic, 
                //which exists already in DB and is currently marked as deleted
                Id = firstSubcategory.Id,
                CategoryId = category.Id,
                Name = SECOND_TEST_BEVERAGES_SUBCATEGORY_NAME                
            };

            await this.subCategoriesServiceMock.EditSubCategoryAsync(editInputModel);

            Assert.False(secondSubcategory.IsDeleted);
            Assert.True(firstSubcategory.IsDeleted);
        }

        [Fact]
        public async Task EditSubCategoryAsync_ThrowsNullReference_WhenSubcategoryNotFound()
        {
            var editInputModel = new SubCategoryEditInputModel
            {
                Id = string.Empty
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.subCategoriesServiceMock.EditSubCategoryAsync(editInputModel));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(SubCategory), string.Empty), exception.Message);
        }

        [Fact]
        public async Task GetSubCategoriesOfACategoryAsync_ReturnsValidSubcategories()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Beverages
            var category = await this.DbContext.Categories.FirstAsync();

            var expected = this.DbContext.SubCategories.ToArray();

            var actual = await this.subCategoriesServiceMock.GetSubCategoriesOfACategoryAsync(category.Id);

            Assert.IsType<SubCategoryBaseViewModel[]>(actual);
            Assert.Collection(actual,
                elem1 => 
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                });
        }

        [Fact]
        public async Task GetSubCategoriesOfACategoryAsync_ReturnsEmptyWhenNothingfound()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Desserts - doesnt have subcategories
            var category = await this.DbContext.Categories.LastAsync();

            var actual = await this.subCategoriesServiceMock.GetSubCategoriesOfACategoryAsync(category.Id);

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetSubCategoryViewModel_ReturnsValidModel()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            //Alcoholic
            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            var actual = await this.subCategoriesServiceMock.GetSubCategoryViewModel(subcategory.Id);

            Assert.IsType<SubCategoryViewModel>(actual);
            Assert.Equal(subcategory.Id, actual.Id);
            Assert.Equal(subcategory.Name, actual.Name);
            Assert.Equal(subcategory.Category.Name, actual.CategoryName);
        }

        [Fact]
        public async Task GetSubCategoryViewModel_ReturnsNotFoundException_WhenSubcategoryNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.subCategoriesServiceMock.GetSubCategoryViewModel(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(SubCategory), string.Empty), exception.Message);
        }

        [Fact]
        public async Task DeleteSubCategoryAsync_SuccessfullyDeletesSubcategory()
        {
            await this.AddTestingCategoriesAndSubcategoriesToDb();

            var subcategory = await this.DbContext.SubCategories.FirstAsync();

            await this.subCategoriesServiceMock.DeleteSubCategoryAsync(subcategory.Id);

            Assert.True(subcategory.IsDeleted);
        }

        [Fact]
        public async Task DeleteSubCategoryAsync_ReturnsNotFoundException_WhenSubcategoryNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.subCategoriesServiceMock.DeleteSubCategoryAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(SubCategory), string.Empty), exception.Message);
        }        

        private async Task AddTestingCategoriesAndSubcategoriesToDb()
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
