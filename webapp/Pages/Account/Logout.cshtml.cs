﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Models;

namespace webapp.Pages.Account {

    public class LogoutModel : PageModel {

        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger) {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet() {
            return Redirect("/");
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null) {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return Redirect(returnUrl ?? "/");
        }
    }
}
