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

        public const string COUPON_NOT_FOUND_ERROR = "Error: Coupon with name \"{0}\" wOrder/ManageOrderas not found. Please try again.";

        public const string ORDER_TOTAL_NOT_COVERING_MIN_COUPON_AMOUNT = "Error: Minimum order amount for this coupon is: ${0:f2}.";

        public const int PAGE_SIZE = 3;

        public const string ORDER_HISTORY_PAGINATION_URL_PARAM = "/Order/OrderHistory?productPage=:";

        public const string MANAGE_ORDER_PAGINATION_URL_PARAM = "/Order/ManageOrders?productPage=:";

        public const string ORDERS_PICKUP_PAGINATION_URL_PARAM = "/Order/OrderPickup?productPage=:";

        public const string SEARCH_NAME_PARAM = "&searchName=";

        public const string SEARCH_EMAIL_PARAM = "&searchEmail=";

        public const string SEARCH_PHONE_PARAM = "&searchPhone=";

        public const int ORDER_ID_SHORT_LENGTH = 5;

        public const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm";

        public const string CURRENCY_FORMAT = "C";

        public class Stripe
        {
            public const string STRIPE_ORDER_DESCRIPTION = "Order Id: ";

            public const string STRIPE_CURRENCY = "USD";

            public const string STRIPE_CHARGE_STATUS_SUCCEEDED = "succeeded";
        }

        public class Email
        {
            public const string EMAIL_SUBJECT_ORDER_CREATED = "GourmeJunk - Order Created";

            public const string EMAIL_SUBJECT_ORDER_CANCELLED = "GourmeJunk - Order Cancelled";

            public const string EMAIL_CONTENT_ORDER_SUBMITTED = "Order has been submitted successfully. Order Id: {0}";

            public const string EMAIL_CONTENT_ORDER_CANCELLED= "Order has been cancelled. Order Id: {0}";

            public const string EMAIL_SUBJECT_ACCOUNT_CREATION_SUCCESSFULL = "GourmeJunk - Successfull Registration";

            public const string EMAIL_CONTENT_ACCOUNT_CREATION_SUCCESSFULL= "Welcome to GourmeJunk, {0} {1}! You have successfully created an account.";
        }
    }
}
