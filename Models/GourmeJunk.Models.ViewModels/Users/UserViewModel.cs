using AutoMapper;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GourmeJunk.Models.ViewModels.Users
{
    public class UserViewModel : IMapFrom<GourmeJunkUser>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.User.FIRST_NAME_DISPLAY)]
        public string FirstName { get; set; }

        [Display(Name = ModelConstants.User.LAST_NAME_DISPLAY)]
        public string LastName { get; set; }

        public string Email { get; set; }
        
        public string Authorization { get; set; }

        public Nullable<DateTimeOffset> LockoutEnd { get; set; }
    }
}
