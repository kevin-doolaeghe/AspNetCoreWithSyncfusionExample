using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        public IActionResult OnGet() {
            return RedirectToPage("Dashboard");
        }
    }
}
