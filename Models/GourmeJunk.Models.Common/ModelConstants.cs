namespace GourmeJunk.Models.Common
{
    public class ModelConstants
    {
        public const string ONLY_CHARACTERS_PATTERN = "^[a-zA-Z]+$";
        public const string ONLY_CHARACTERS_ERROR = "Input must contain only characters";

        public const string NAME_LENGTH_ERROR = "Name length must be between {2} and {1} symbols";

        public const string MUST_SELECT_CATEGORY = "Category must be selected.";

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

            public const string MIN_PRICE = "0.01";
            public const string MAX_PRICE = "79228162514264337593543950335";
            public const string PRICE_ERROR = "Invalid Price.";            
        } 
        
        public class Coupon
        {
            public const string NAME_DISPLAY = "Coupon Name";
            public const string MIN_ORDER_AMOUNT_DISPLAY = "Minimum Order Amount";
            public const string IS_ACTIVE_DISPLAY = "Active";
        }
    }
}
