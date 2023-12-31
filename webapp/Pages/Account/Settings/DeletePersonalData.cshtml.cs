﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using webapp.Models;

namespace webapp.Pages.Account.Settings {

    public class DeletePersonalDataModel : PageModel {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<DeletePersonalDataModel> _localizer;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<DeletePersonalDataModel> localizer, ILogger<DeletePersonalDataModel> logger) {
            _userManager = userManager;
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
        public class InputModel {

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "The field is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = default!;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            
            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword) {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password)) {
                    ModelState.AddModelError(string.Empty, _localizer["Incorrect password"]);
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded) {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
            
            return Redirect("/");
        }
    }
}
