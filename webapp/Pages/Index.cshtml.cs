using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        public IActionResult OnGet() {
            if (User.Identity?.IsAuthenticated ?? false) return Redirect("/Dashboard");
            else return Redirect("/Account/Login");
        }
    }
}
