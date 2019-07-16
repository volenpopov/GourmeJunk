using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryEditViewModel
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.SubCategory.NameDisplay)]
        public string Name { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = ModelConstants.Category.NameDisplay)]
        public string CategoryId { get; set; }        

        public string StatusMessage { get; set; }
        public string[] SubCategoryList { get; set; }
    }
}
