using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using webapp.Services;

namespace webapp.Pages.Balance {

    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        private readonly DatabaseContext _databaseContext;

        public IndexModel(ILogger<IndexModel> logger, DatabaseContext databaseContext) {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        public double CurrentBalance { get; set; }

        public double RealBalance { get; set; }

        public double Cashflow { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            var transactions = await _databaseContext.Transactions
                .AsNoTracking()
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
