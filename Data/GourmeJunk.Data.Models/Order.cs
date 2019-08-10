using GourmeJunk.Data.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class Order : BaseModel<string>
    {
        public DateTime OrderDate { get; set; }

        public decimal OrderTotalOriginal { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual GourmeJunkUser User { get; set; }
    }
}
