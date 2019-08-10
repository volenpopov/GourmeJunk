namespace GourmeJunk.Web.Common
{
    public class WebConstants
    {
        public const string REGISTER_INPUT_ROLE_FIELD = "inputUserRole";

        public const string CART_FORM_MENUITEM_ID_FIELDNAME = "MenuItem.Id";

        public const string SESSION_NAME_SHOPPING_CART_INDIVIDUAL_ITEMS_COUNT = "ssCartCount";

        public const int SESSION_SHOPPING_CART_INITIAL_INDIVIDUAL_ITEMS_COUNT = 0;

        public class IdentityModels
        {
            public const string USERS_NAME_PATTERN = @"^[a-zA-Z]+$";

            public const string USERS_NAME_PATTERN_ERROR = "Invalid input. Name can contain only letters.";

            public const string ADDRESS_PATTERN = @"^[a-zA-Z0-9-\s-\,-.]+[a-zA-Z-0-9]$";

            public const string FIRSTNAME_DISPLAY = "First Name";

            public const string LASTNAME_DISPLAY = "Last Name";
        }        

        public class Error
        {
            public const string ENTITY_ALREADY_EXISTS = "Error: {0} already exists!";            
        }
    }
}
