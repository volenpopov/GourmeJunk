namespace GourmeJunk.Data.Common.Models
{
    public class DataModelConstants
    {
        public const int CATEGORY_NAME_MAX_LENGTH = 25;

        public const int SUBCATEGORY_NAME_MAX_LENGTH = 25;

        public const int MENUITEM_NAME_MAX_LENGTH = 50;

        public const int COUPON_NAME_MAX_LENGTH = 25;

        public const string MIN_PRICE = "0.01";

        public const string MAX_PRICE = "79228162514264337593543950335";

        public const int CART_MIN_ITEMS_COUNT = 1;

        public const string CART_ITEMS_COUNT_RANGE_ERRORMSG = "Please enter a value greater than or equal to {0}.";
    }
}
