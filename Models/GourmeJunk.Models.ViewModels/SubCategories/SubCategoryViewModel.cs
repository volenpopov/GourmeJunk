using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryViewModel : SubCategoryBaseViewModel
    {        
        [Display(Name = ModelConstants.Category.NAME_DISPLAY)]
        public string CategoryName { get; set; }
    }
}
