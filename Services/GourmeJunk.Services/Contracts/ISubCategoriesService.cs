﻿using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Models.ViewModels.SubCategories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ISubCategoriesService
    {
        Task<IEnumerable<SubCategoryViewModel>> GetAllSubCategoriesViewModelsAsync();

        Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryId);

        SubCategoryCreateViewModel GetSubCategoryCreateViewModelAsync(IEnumerable<CategoryViewModel> categoryList);

        Task CreateSubCategoryAsync(SubCategoryCreateInputModel model);

        Task<IEnumerable<SubCategoryBaseViewModel>> GetSubCategoriesOfACategoryAsync(string categoryId);

        Task<SubCategoryEditViewModel> GetSubCategoryEditViewModelAsync(string subCategoryId);

        Task EditSubCategoryAsync(SubCategoryEditInputModel model);

        Task<SubCategoryViewModel> GetSubCategoryViewModel(string subCategoryId);

        Task DeleteSubCategoryAsync(string subCategoryId);
    }
}
