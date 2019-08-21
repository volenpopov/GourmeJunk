using GourmeJunk.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class Order : BaseModel<string>
    {
        public Order()
        {
            this.OrderDate = DateTime.UtcNow;
            this.OrderStatus = OrderStatus.Submitted;
            this.OrderMenuItems = new HashSet<OrderMenuItems>();
        }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotalOriginal { get; set; }

        public decimal OrderTotal { get; set; }

        public string CouponName { get; set; }
        
        public OrderStatus OrderStatus { get; set; }

        public DateTime PickUpDateAndTime { get; set; }             

        [Required]
        public string PickupName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }        

        public string Comments { get; set; }

        public string TransactionId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual GourmeJunkUser User { get; set; }

        public virtual ICollection<OrderMenuItems> OrderMenuItems { get; set; }
    }
}
