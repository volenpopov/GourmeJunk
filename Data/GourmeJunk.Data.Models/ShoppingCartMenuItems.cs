using GourmeJunk.Data.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class ShoppingCartMenuItems : IAuditableEntity, IDeletableEntity
    {
        [Required]
        public string ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        [Required]
        public string MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }
        
        public int Count { get; set; }
            
        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
