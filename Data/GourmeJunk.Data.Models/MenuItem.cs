using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class MenuItem : BaseDeletableModel<string>
    {        
        [Required]
        [MaxLength(DataModelConstants.MENUITEM_NAME_MAX_LENGTH)]
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public string SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        [Range(typeof(decimal), DataModelConstants.MIN_PRICE, DataModelConstants.MAX_PRICE)]
        public decimal Price { get; set; }
    }
}
