﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Models;

namespace webapp.Pages.Account.Settings {

    public class PersonalDataModel : PageModel {

        private readonly UserManager<User> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(UserManager<User> userManager, ILogger<PersonalDataModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return Page();
        }
    }
}
