using GourmeJunk.Models.Common;
using System.ComponentModel;

namespace GourmeJunk.Models.ViewModels.Categories
{
    public class CategoryCreateViewModel
    {
        [DisplayName(ModelConstants.Category.NAME_DISPLAY)]
        public string Name { get; set; }

        public string StatusMessage { get; set; }
    }
}
