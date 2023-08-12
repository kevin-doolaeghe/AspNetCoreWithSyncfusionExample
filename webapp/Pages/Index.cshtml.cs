using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        public IActionResult OnGet() {
            bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated) return RedirectToPage("/Account/Login");
            else return RedirectToPage("/Dashboard");
        }
    }
}
