﻿using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryViewModel : SubCategoryBaseViewModel, IMapFrom<SubCategory>
    {        
        [Display(Name = ModelConstants.Category.NameDisplay)]
        public string CategoryName { get; set; }
    }
}
