﻿namespace GourmeJunk.Services.Common
{
    public class ServicesDataConstants
    {
        public const string WWWROOT = "wwwroot";

        public const string MENUITEMS_IMGS_PATH = WWWROOT + @"\images\MenuItems";      

        public const string MENUITEM_DEFAULT_IMG_PATH = WWWROOT + @"\images\MenuItems\img_default.jpg";

        public const string NULL_REFERENCE_ID = "{0} with id {1} not found.";

        public const string GET_DELETED_ENTITY = "{0} {1} has been deleted and you cannot access it.";

        public const string INVALID_IMG_TYPE = "Image type {0} is not supported. Valid types are: \".jpg\" and \".png\"";
    }
}
