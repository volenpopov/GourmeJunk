using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class MenuItem : BaseDeletableModel<string>
    {        
        [Required]
        [MaxLength(DataModelConstants.MenuItemNameMaxLength)]
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public string SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        [Range(typeof(decimal), DataModelConstants.MinPrice, DataModelConstants.MaxPrice)]
        public decimal Price { get; set; }
    }
}
