using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System.Globalization;
using System.Text.Json;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Transactions {

    [IgnoreAntiforgeryToken]
    public class CurrentMonthModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        private readonly DatabaseContext _databaseContext;

        public CurrentMonthModel(ILogger<IndexModel> logger, DatabaseContext databaseContext) {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        public IList<Category> Categories { get; set; } = default!;

        public IEnumerable<Transaction> DataSource { get; set; } = default!;

        public double CurrentBalance { get; set; }

        public double RealBalance { get; set; }

        public double Cashflow { get; set; }

        public async Task OnGetAsync() {
            Categories = await _databaseContext.Categories.AsNoTracking().ToListAsync();
            var records = await _databaseContext.Transactions
                .AsNoTracking()
                .ToListAsync();

            RealBalance = records.Select(x => x.Amount).Sum();
            CurrentBalance = records.Where(x => x.IsDone).Select(x => x.Amount).Sum();
            Cashflow = double.Abs(RealBalance - CurrentBalance);

            var currentDate = DateTime.Today;
            DataSource = await _databaseContext.Transactions
                .Where(x => x.Date.Year == currentDate.Year && x.Date.Month == currentDate.Month)
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<JsonResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            var currentDate = DateTime.Today;
            DataSource = await _databaseContext.Transactions
                .Where(x => x.Date.Year == currentDate.Year && x.Date.Month == currentDate.Month)
                .OrderBy(x => x.Date)
                .ToListAsync();
            int count = DataSource.Cast<Transaction>().Count();

            DataOperations operations = new();
            // Search
            if (dm.Search != null && dm.Search.Count > 0) {
                DataSource = operations.PerformSearching(DataSource, dm.Search);
            }
            // Sorting
            if (dm.Sorted != null && dm.Sorted.Count > 0) {
                DataSource = operations.PerformSorting(DataSource, dm.Sorted);
            }
            // Filtering
            if (dm.Where != null && dm.Where.Count > 0) {
                DataSource = operations.PerformFiltering(DataSource, dm.Where, dm.Where.First().Operator);
            }
            // Paging
            if (dm.Skip != 0) {
                DataSource = operations.PerformSkip(DataSource, dm.Skip);
            }
            if (dm.Take != 0) {
                DataSource = operations.PerformTake(DataSource, dm.Take);
            }
            return new JsonResult(dm.RequiresCounts ? new { result = DataSource, count } : new { result = DataSource });
        }

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<Transaction> request) {
            switch (request.Action) {
                case "insert":
                    _databaseContext.Transactions.Add(request.Value);
                    break;
                case "update":
                    _databaseContext.Transactions.Update(request.Value);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    _databaseContext.Transactions.Remove(_databaseContext.Transactions.Where(x => x.TransactionId == id).First());
                    break;
            }
            await _databaseContext.SaveChangesAsync();

            if (request.Action == "remove") return new JsonResult(request);
            return new JsonResult(request.Value);
        }
    }
}
