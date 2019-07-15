using GourmeJunk.Data.Models;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.SubCategories
{
    public class SubCategoryViewModel : IMapFrom<SubCategory>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }
    }
}
