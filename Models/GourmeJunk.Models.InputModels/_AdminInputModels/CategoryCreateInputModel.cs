﻿using GourmeJunk.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class CategoryCreateInputModel
    {
        [Required]
        [StringLength(ModelConstants.Category.NameMaxLength, MinimumLength = ModelConstants.Category.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        [RegularExpression(ModelConstants.OnlyCharactersPattern, ErrorMessage = ModelConstants.OnlyCharactersError)]
        [DisplayName(ModelConstants.Category.NameDisplay)]
        public string Name { get; set; }

        public string StatusMessage { get; set; }
    }
}
