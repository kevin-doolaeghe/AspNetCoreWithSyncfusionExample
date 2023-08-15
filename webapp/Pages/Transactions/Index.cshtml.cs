using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System.Text.Json;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Transactions {

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;

        public IndexModel(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public IList<Category> Categories { get; set; } = default!;

        public IEnumerable<Transaction> DataSource { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            Categories = await _databaseContext.Categories.AsNoTracking().ToListAsync();
            DataSource = await _databaseContext.Transactions
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            DataSource = await _databaseContext.Transactions
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
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            switch (request.Action) {
                case "insert":
                    _databaseContext.Transactions.Add(request.Value);
                    break;
                case "update":
                    _databaseContext.Transactions.Update(request.Value);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    _databaseContext.Transactions.Remove(_databaseContext.Transactions.Where(x => x.Id == id).First());
                    break;
            }
            await _databaseContext.SaveChangesAsync();

            if (request.Action == "remove") return new JsonResult(request);
            return new JsonResult(request.Value);
        }
    }
}
