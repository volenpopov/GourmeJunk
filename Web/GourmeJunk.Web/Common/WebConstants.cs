namespace GourmeJunk.Web.Common
{
    public class WebConstants
    {
        public const string REGISTER_INPUT_ROLE_FIELD = "inputUserRole";

        public const string CART_FORM_MENUITEM_ID_FIELDNAME = "MenuItem.Id";

        public const string SESSION_NAME_SHOPPING_CART_INDIVIDUAL_ITEMS_COUNT = "ssCartCount";

        public const int SESSION_SHOPPING_CART_INITIAL_INDIVIDUAL_ITEMS_COUNT = 0;

        public const string SESSION_NAME_COUPON_CODE = "ssCouponCode";        

        public class IdentityModels
        {
            public const string USERS_NAME_PATTERN = @"^[a-zA-Z]+$";

            public const string USERS_NAME_PATTERN_ERROR = "Invalid input. Name can contain only letters.";

            public const string ADDRESS_PATTERN = @"^[a-zA-Z0-9-\s-\,-.]+[a-zA-Z-0-9]$";

            public const string ADDRESS_PATTERN_ERROR = @"Invalid input for address.";

            public const string FIRSTNAME_DISPLAY = "First Name";

            public const string LASTNAME_DISPLAY = "Last Name";
        }

        public class Error
        {
            public const string ENTITY_ALREADY_EXISTS = "Error: {0} already exists!";
        }

        public class OrderItem
        {
            public const string ITEM_ID_PROPERTY = "item.Id";
            public const string ITEM_NAME_PROPERTY = "item.Name";
            public const string ITEM_PRICE_PROPERTY = "item.Price";
            public const string ITEM_COUNT_PROPERTY = "item.Count";
        }

        public class Stripe
        {
            public const string STRIPE_SECTION_NAME = "Stripe";

            public const string SECRET_KEY_SECTION_NAME = "SecretKey";            
        }

        public class Facebook
        {
            public const string FB_APPID_SECTION = "Authentication:Facebook:AppId";
            public const string FB_APP_SECRET_SECTION = "Authentication:Facebook:AppSecret";
        }

        public class Google
        {
            public const string GOOGLE_CLIENTID_SECTION = "Authentication:Google:ClientId";
            public const string GOOGLE_CLIENTSECRET_SECTION = "Authentication:Google:ClientSecret";
        }
    }
}
