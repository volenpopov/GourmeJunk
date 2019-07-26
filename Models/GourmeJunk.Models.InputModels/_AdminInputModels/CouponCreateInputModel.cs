﻿using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class CouponCreateInputModel
    {
        [Required]
        [RegularExpression(ModelConstants.Coupon.NAME_PATTERN, ErrorMessage = ModelConstants.Coupon.NAME_PATTERN_ERROR)]
        [StringLength(ModelConstants.Coupon.NAME_MAX_LENGTH, MinimumLength = ModelConstants.Coupon.NAME_MIN_LENGTH, ErrorMessage = ModelConstants.NAME_LENGTH_ERROR)]
        public string Name { get; set; }

        public CouponType CouponType { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE)]
        public decimal Discount { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        [Display(Name = ModelConstants.Coupon.MIN_ORDER_AMOUNT_DISPLAY)]
        public decimal? MinimumOrderAmount { get; set; }

        [Display(Name = ModelConstants.Coupon.IS_ACTIVE_DISPLAY)]
        public bool IsActive { get; set; }

        public string StatusMessage { get; set; }
    }
}
