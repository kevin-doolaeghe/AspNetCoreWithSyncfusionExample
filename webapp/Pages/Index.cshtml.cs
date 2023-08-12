using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        public IndexModel() {
        }

        public IActionResult OnGet() {
            return RedirectToPage("Dashboard");
        }
    }
}
