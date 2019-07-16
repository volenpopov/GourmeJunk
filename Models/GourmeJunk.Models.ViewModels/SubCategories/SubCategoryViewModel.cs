using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryViewModel : IMapFrom<SubCategory>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.SubCategory.NameDisplay)]
        public string Name { get; set; }

        [Display(Name = ModelConstants.Category.NameDisplay)]
        public string CategoryName { get; set; }
    }
}
