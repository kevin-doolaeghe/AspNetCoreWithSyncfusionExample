using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Balance {

    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(DatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor) {
            _databaseContext = databaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public double CurrentBalance { get; set; }

        public double RealBalance { get; set; }

        public double Cashflow { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

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

        public async Task<User?> GetUserAsync() {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _databaseContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
