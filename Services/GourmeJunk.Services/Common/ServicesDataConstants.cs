namespace GourmeJunk.Services.Common
{
    public class ServicesDataConstants
    {
        public const string WWWROOT = "wwwroot";

        public const string MENUITEMS_IMGS_PATH = WWWROOT + @"\images\MenuItems";

        public const string MENUITEM_DEFAULT_IMG_PATH = @"\images\MenuItems\img_default.jpg";

        public const string NULL_REFERENCE_ID = "{0} with Id \"{1}\" not found.";

        public const string NULL_REFERENCE_USER_CART = "User with Id \"{0}\" doesn't have a shopping cart.";

        public const string NULL_REFERENCE_USER_EMAIL = "User with email \"{0}\" doesn't exist.";

        public const string NO_SUBCATEGORY_SELECTED_DEFAULT_VALUE = "-1";

        public const string JPG_EXTENSION = ".jpg";

        public const string PNG_EXTENSION = ".png";

        public const string INVALID_IMG_TYPE = "Image type \"{0}\" is not supported. Valid types are: \".jpg\" and \".png\"";

        public const string SQL_MODIFY_DELETABLE_ENTITIES_SUBCATEGORIES = @"UPDATE SubCategories
                                                              SET 
                                                               IsDeleted = 1, 
                                                               DeletedOn = GETUTCDATE()
                                                              WHERE CategoryId = {0}";

        public const string SQL_MODIFY_DELETABLE_ENTITIES_MENUITEMS = @"UPDATE MenuItems
                                                              SET 
                                                               IsDeleted = 1, 
                                                               DeletedOn = GETUTCDATE()
                                                              WHERE CategoryId = {0}";

        public const string USER = "User";

        public const int LOCKOUT_YEARS = 1000;

        public const int CART_INDEX_MENUITEM_DESCRIPTION_MAX_CHARS = 100;

        public const string COUPON_NOT_FOUND_ERROR = "Error: Coupon with name \"{0}\" was not found. Please try again.";

        public const int PAGE_SIZE = 2;

        public const string PAGINATION_URL_PARAM = "/Order/OrderHistory?productPage=:";

        public class Stripe
        {
            public const string STRIPE_ORDER_DESCRIPTION = "Order Id: ";

            public const string STRIPE_CURRENCY = "USD";

            public const string STRIPE_CHARGE_STATUS_SUCCEEDED = "succeeded";
        }
    }
}
