using GourmeJunk.Data.Models;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.Categories
{
    public class CategoryViewModel : IMapFrom<Category>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
