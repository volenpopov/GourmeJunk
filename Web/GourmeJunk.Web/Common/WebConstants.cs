namespace GourmeJunk.Web.Common
{
    public class WebConstants
    {
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
