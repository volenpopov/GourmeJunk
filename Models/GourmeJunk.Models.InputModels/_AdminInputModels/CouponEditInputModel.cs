using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class CouponEditInputModel : CouponCreateInputModel
    {
        [Required]
        public string Id { get; set; }
    }
}
