using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        public IActionResult OnGet() {
            if (User.Identity?.IsAuthenticated ?? false) return RedirectToPage("/Dashboard");
            else return RedirectToPage("/Account/Login");
        }
    }
}
