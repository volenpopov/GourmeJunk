﻿using GourmeJunk.Models.Common;
using GourmeJunk.Models.ViewModels.Categories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryCreateViewModel
    {
        [Display(Name = ModelConstants.SubCategory.NAME_DISPLAY)]
        public string Name { get; set; }

        [Display(Name = ModelConstants.Category.NAME_DISPLAY)]
        public string CategoryId { get; set; }
        public IEnumerable<CategoryViewModel> CategoryList { get; set; }

        public string StatusMessage { get; set; }
    }
}
