using GourmeJunk.Common;
using GourmeJunk.Data.Models;
using GourmeJunk.Services.Common;
using GourmeJunk.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<GourmeJunkUser> _signInManager;
        private readonly UserManager<GourmeJunkUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<GourmeJunkRole> _roleManager;

        public RegisterModel(
            UserManager<GourmeJunkUser> userManager,
            SignInManager<GourmeJunkUser> signInManager,            
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<GourmeJunkRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;           
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MaxLength(25)]
            [Display(Name = WebConstants.IdentityModels.FIRSTNAME_DISPLAY)]
            [RegularExpression(WebConstants.IdentityModels.USERS_NAME_PATTERN, ErrorMessage = WebConstants.IdentityModels.USERS_NAME_PATTERN_ERROR)]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(25)]
            [Display(Name = WebConstants.IdentityModels.LASTNAME_DISPLAY)]
            [RegularExpression(WebConstants.IdentityModels.USERS_NAME_PATTERN, ErrorMessage = WebConstants.IdentityModels.USERS_NAME_PATTERN_ERROR)]
            public string LastName { get; set; }

            [Required]
            [MaxLength(100)]
            [RegularExpression(WebConstants.IdentityModels.ADDRESS_PATTERN)]
            public string Address { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var role = Request.Form[WebConstants.REGISTER_INPUT_ROLE_FIELD].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new GourmeJunkUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,  
                    Address = Input.Address                  
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (role == GlobalConstants.KITCHEN_ROLE_NAME || role == GlobalConstants.RECEPTION_ROLE_NAME)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, GlobalConstants.CUSTOMER_ROLE_NAME);
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        await _emailSender.SendEmailAsync(user.Email,
                            ServicesDataConstants.Email.EMAIL_SUBJECT_ACCOUNT_CREATION_SUCCESSFULL,
                            string.Format(ServicesDataConstants.Email.EMAIL_CONTENT_ACCOUNT_CREATION_SUCCESSFULL, user.FirstName, user.LastName));

                        return LocalRedirect(returnUrl);
                    }

                    _logger.LogInformation("User created a new account with password.");
                   
                    return RedirectToAction("Index", "User", new { Area = "Admin"});
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
