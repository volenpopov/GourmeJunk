namespace GourmeJunk.Models.Common
{
    public class ModelConstants
    {
        public const string OnlyCharactersPattern = "^[a-zA-Z]+$";
        public const string OnlyCharactersError = "Input must contain only characters";

        public const string NameLengthError = "Name length must be between {2} and {1} symbols";
        public class Category
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 25;

            public const string NameDisplay = "Category Name";
        }

    }
}
