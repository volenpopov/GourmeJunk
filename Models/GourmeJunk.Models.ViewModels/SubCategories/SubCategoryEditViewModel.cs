using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryEditViewModel : SubCategoryBaseViewModel
    {        
        public string CategoryName { get; set; }

        [Display(Name = ModelConstants.Category.NAME_DISPLAY)]
        public string CategoryId { get; set; }        

        public string StatusMessage { get; set; }
        public string[] SubCategoryList { get; set; }
    }
}
