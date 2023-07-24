using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System.Globalization;
using System.Text.Json;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Records {

    [IgnoreAntiforgeryToken]
    public class CurrentMonthModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        private readonly DatabaseContext _databaseContext;

        public CurrentMonthModel(ILogger<IndexModel> logger, DatabaseContext databaseContext) {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        public IEnumerable<Record> DataSource { get; set; } = default!;

        public string? CurrentBalance { get; set; }

        public string? RealBalance { get; set; }

        public string? Cashflow { get; set; }

        public async Task OnGetAsync() {
            var records = await _databaseContext.Records
                .AsNoTracking()
                .ToListAsync();

            var realBalance = records.Select(x => x.Amount).Sum();
            var currentBalance = records.Where(x => x.IsDone).Select(x => x.Amount).Sum();
            var cultureInfo = new CultureInfo("fr-FR");
            RealBalance = realBalance.ToString("C2", cultureInfo);
            CurrentBalance = currentBalance.ToString("C2", cultureInfo);
            Cashflow = (realBalance - currentBalance).ToString("C2", cultureInfo);

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var initialBalance = _databaseContext.Records
                .AsNoTracking()
                .Where(x => x.Date < firstDayOfMonth)
                .Select(x => x.Amount)
                .Sum();

            var dataSource = await _databaseContext.Records
                .Where(x => x.Date.Year == today.Year && x.Date.Month == today.Month)
                .OrderBy(x => x.Date)
                .ToListAsync();

            for (int i = 0; i < dataSource.Count; i++) {
                if (i == 0) dataSource[i].SlidingBalance = initialBalance + dataSource[i].Amount;
                else dataSource[i].SlidingBalance = dataSource[i - 1].SlidingBalance + dataSource[i].Amount;
            }
            DataSource = dataSource;
        }

        public async Task<JsonResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var initialBalance = _databaseContext.Records
                .AsNoTracking()
                .Where(x => x.Date < firstDayOfMonth)
                .Select(x => x.Amount)
                .Sum();

            var dataSource = await _databaseContext.Records
                .Where(x => x.Date.Year == today.Year && x.Date.Month == today.Month)
                .OrderBy(x => x.Date)
                .ToListAsync();
            for (int i = 0; i < dataSource.Count; i++) {
                if (i == 0) dataSource[i].SlidingBalance = initialBalance + dataSource[i].Amount;
                else dataSource[i].SlidingBalance = dataSource[i - 1].SlidingBalance + dataSource[i].Amount;
            }
            DataSource = dataSource;
            int count = DataSource.Cast<Record>().Count();

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

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<Record> request) {
            _logger.LogInformation($"Request: {JsonSerializer.Serialize(request)}");
            switch (request.Action) {
                case "insert":
                    _databaseContext.Records.Add(request.Value);
                    break;
                case "update":
                    _databaseContext.Records.Update(request.Value);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    _databaseContext.Records.Remove(_databaseContext.Records.Where(x => x.RecordId == id).First());
                    break;
            }
            await _databaseContext.SaveChangesAsync();

            if (request.Action == "remove") return new JsonResult(request);
            return new JsonResult(request.Value);
        }
    }
}
