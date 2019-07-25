namespace GourmeJunk.Services.Common
{
    public class ServicesDataConstants
    {
        public const string WWWROOT = "wwwroot";

        public const string MENUITEMS_IMGS_PATH = WWWROOT + @"\images\MenuItems";      

        public const string MENUITEM_DEFAULT_IMG_PATH = @"\images\MenuItems\img_default.jpg";

        public const string NULL_REFERENCE_ID = "{0} with id {1} not found.";

        public const string NO_SUBCATEGORY_SELECTED_DEFAULT_VALUE = "-1";

        public const string JPG_EXTENSION = ".jpg";

        public const string PNG_EXTENSION = ".png";

        public const string INVALID_IMG_TYPE = "Image type {0} is not supported. Valid types are: \".jpg\" and \".png\"";

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
    }
}
