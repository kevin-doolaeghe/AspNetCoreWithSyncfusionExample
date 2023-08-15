using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using webapp.Models;

namespace webapp.Pages.Account {

    public class RegisterModel : PageModel {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IStringLocalizer<RegisterModel> _localizer;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<User> userManager, IUserStore<User> userStore, SignInManager<User> signInManager, IStringLocalizer<RegisterModel> localizer, ILogger<RegisterModel> logger) {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; } = default!;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel {

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "The field is required")]
            [EmailAddress(ErrorMessage = "Wrong email format")]
            [Display(Name = "Email")]
            public string Email { get; set; } = default!;

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "The field is required")]
            [StringLength(20, ErrorMessage = "Password error message", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = default!;

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "The field is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Confirm password error message")]
            [Display(Name = "Confirm password")]
            public string ConfirmPassword { get; set; } = default!;
        }

        public void OnGet(string? returnUrl = null) {
            ReturnUrl = returnUrl ?? "/";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null) {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid) {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                        return RedirectToPage("Login", new { returnUrl });
                    } else {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors) {
                    string description = _localizer[error.Code];
                    ModelState.AddModelError(string.Empty, description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        
        private User CreateUser() {
            try {
                return Activator.CreateInstance<User>();
            } catch {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml"
                );
            }
        }

        private IUserEmailStore<User> GetEmailStore() {
            if (!_userManager.SupportsUserEmail) {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>) _userStore;
        }
    }
}
