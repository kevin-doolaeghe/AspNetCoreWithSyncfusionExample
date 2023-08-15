using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Balance {

    public class IndexModel : PageModel {

        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _databaseContext;

        public IndexModel(UserManager<User> userManager, DatabaseContext databaseContext) {
            _userManager = userManager;
            _databaseContext = databaseContext;
        }

        public double CurrentBalance { get; set; }

        public double RealBalance { get; set; }

        public double Cashflow { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var transactions = await _databaseContext.Transactions
                .AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .ToListAsync();
            CurrentBalance = transactions.Where(x => x.IsDone).Select(x => x.Amount).Sum();
            RealBalance = transactions.Select(x => x.Amount).Sum();
            Cashflow = double.Abs(RealBalance - CurrentBalance);

            return new JsonResult(new {
                currentBalance = CurrentBalance,
                formattedCurrentBalance = CurrentBalance.ToString("C2", CultureInfo.CurrentCulture),
                realBalance = RealBalance,
                formattedRealBalance = RealBalance.ToString("C2", CultureInfo.CurrentCulture),
                cashflow = Cashflow,
                formattedCashflow = Cashflow.ToString("C2", CultureInfo.CurrentCulture),
            });
        }
    }
}
