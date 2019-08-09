using GourmeJunk.Data.Common.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class ShoppingCart : BaseModel<string>
    {
        public ShoppingCart()
        {
            //this.ShoppingCartMenuItems = new HashSet<ShoppingCartMenuItems>();
        }

        [Required]        
        public string UserId { get; set; }
        
        public virtual GourmeJunkUser User { get; set; }

        //public ICollection<ShoppingCartMenuItems> ShoppingCartMenuItems { get; set; }
    }
}
