using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Services;

namespace webapp.Pages {

    public class LanguageModel : PageModel {

        private readonly ILogger<LanguageModel> _logger;

        public LanguageModel(ILogger<LanguageModel> logger) {
            _logger = logger;
        }

        public void OnGet(string? newCulture) {
            _logger.LogInformation($"New culture: {newCulture}");

            if (newCulture != null) {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(newCulture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            Response.Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
    }
}
