using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using webapp.Models;

namespace webapp.Pages.Account.Settings {

    public class EmailModel : PageModel {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<EmailModel> _localizer;
        private readonly ILogger<EmailModel> _logger;

        public EmailModel(UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<EmailModel> localizer, ILogger<EmailModel> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

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
        [TempData]
        public string StatusMessage { get; set; } = default!;

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
            [Display(Name = "New email")]
            public string NewEmail { get; set; } = default!;
        }

        private async Task LoadAsync(User user) {
            var email = await _userManager.GetEmailAsync(user);
            if (email != null) {
                Email = email;
                Input = new InputModel { NewEmail = email };
            }
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync() {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.UserName = Input.NewEmail;
            user.Email = Input.NewEmail;

            var changeEmailResult = await _userManager.UpdateAsync(user);
            if (!changeEmailResult.Succeeded) {
                foreach (var error in changeEmailResult.Errors) {
                    string description = _localizer[error.Code];
                    ModelState.AddModelError(string.Empty, description);
                }
                return Page();
            }
            
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their email successfully.");
            StatusMessage = _localizer["Email updated"];

            await LoadAsync(user);
            return Page();
        }
    }
}
