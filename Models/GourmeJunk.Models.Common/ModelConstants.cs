namespace GourmeJunk.Models.Common
{
    public class ModelConstants
    {
        public const string ONLY_CHARACTERS_PATTERN = "^[a-zA-Z]+$";
        public const string ONLY_CHARACTERS_ERROR = "Input must contain only characters";

        public const string ONLY_DIGITS_PATTERN = "[0-9]+";
        public const string ONLY_DIGITS_ERROR = "Input must contain only digits";

        public const string NAME_LENGTH_ERROR = "Name length must be between {2} and {1} symbols";

        public const string MUST_SELECT_CATEGORY = "Category must be selected.";

        public const string MIN_PRICE = "0.01";
        public const string MAX_PRICE = "79228162514264337593543950335";        
        public const string PRICE_ERROR = "Invalid Price.";

        public const int MIN_ITEM_COUNT = 1;
        public const int MAX_ITEM_COUNT = int.MaxValue;

        public class Category
        {
            public const int NAME_MIN_LENGTH = 3;
            public const int NAME_MAX_LENGTH = 25;

            public const string NAME_DISPLAY = "Category Name";
        }

        public class SubCategory
        {
            public const int NAME_MIN_LENGTH = 3;
            public const int NAME_MAX_LENGTH = 25;

            public const string NAME_DISPLAY = "SubCategory Name";
        }

        public class MenuItem
        {
            public const int NAME_MIN_LENGTH = 3;
            public const int NAME_MAX_LENGTH = 50;
            public const int DESCRIPTION_MAX_LENGTH = 300;

            public const string NAME_PATTERN = @"^[a-zA-Z][a-zA-Z-\s]+[a-zA-Z]$";
            public const string NAME_PATTERN_ERROR = "Name can contain only characters.";
            public const string DESCRIPTION_LENGTH_ERROR = "Description is too long.";
        } 
        
        public class Coupon
        {
            public const string NAME_DISPLAY = "Coupon Name";
            public const string MIN_ORDER_AMOUNT_DISPLAY = "Minimum Order Amount";
            public const string IS_ACTIVE_DISPLAY = "Active";

            public const int NAME_MIN_LENGTH = 3;
            public const int NAME_MAX_LENGTH = 25;

            public const string NAME_PATTERN = @"^[a-zA-Z0-9][a-zA-Z0-9-\%\$\s]+[a-zA-Z0-9]$";
            public const string NAME_PATTERN_ERROR = "Name can contain only characters.";

            public const string DISCOUNT_ERROR = "Invalid Discount.";
        }

        public class User
        {
            public const string FIRST_NAME_DISPLAY = "First Name";
            public const string LAST_NAME_DISPLAY = "Last Name";
        }

        public class Order
        {
            public const string PICKUPNAME_DISPLAY = "Pickup Name";
            public const string PICKUPTIME_DISPLAY = "Pickup Time";
            public const string ORDERTOTAL_DISPLAY = "Order Total";
            public const string TOTALITEMS_DISPLAY = "Total Items";
        }
    }
}
