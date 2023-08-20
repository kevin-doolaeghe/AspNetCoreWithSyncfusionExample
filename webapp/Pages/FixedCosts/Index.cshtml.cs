using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Syncfusion.EJ2.Base;
using System.Security.Claims;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.FixedCosts {

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<IndexModel> _localizer;

        public IndexModel(DatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IStringLocalizer<IndexModel> localizer) {
            _databaseContext = databaseContext;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public IList<Category> Categories { get; set; } = default!;

        public IEnumerable<MonthlyTransaction> DataSource { get; set; } = default!;

        [BindProperty]
        public DateTime SelectedMonth { get; set; } = DateTime.Today;

        [TempData]
        public string StatusMessage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            Categories = await _databaseContext.Categories
                .AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .ToListAsync();
            DataSource = await _databaseContext.MonthlyTransactions
                .Where(x => x.UserId == user.Id)
                .OrderBy(x => x.DayOfMonth)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            var monthlyTransactions = await _databaseContext.MonthlyTransactions
                .AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .OrderBy(x => x.DayOfMonth)
                .Select(x => new Transaction() {
                    CategoryId = x.CategoryId,
                    UserId = user.Id,
                    IsDone = false,
                    Note = x.Note,
                    Amount = x.Amount,
                    Date = new DateTime(SelectedMonth.Year, SelectedMonth.Month, x.DayOfMonth),
                })
                .ToListAsync();

            if (monthlyTransactions.Count == 0) return RedirectToPage();

            _databaseContext.Transactions.AddRange(monthlyTransactions);
            await _databaseContext.SaveChangesAsync();

            StatusMessage = _localizer["Transactions added"];
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            DataSource = await _databaseContext.MonthlyTransactions
                .Where(x => x.UserId == user.Id)
                .OrderBy(x => x.DayOfMonth)
                .ToListAsync();
            int count = DataSource.Cast<MonthlyTransaction>().Count();

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

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<MonthlyTransaction> request) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            switch (request.Action) {
                case "insert":
                    request.Value.UserId = user.Id;
                    _databaseContext.MonthlyTransactions.Add(request.Value);
                    break;
                case "update":
                    _databaseContext.MonthlyTransactions.Update(request.Value);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    var monthlyTransaction = await _databaseContext.MonthlyTransactions
                        .Where(x => x.Id == id)
                        .Where(x => x.UserId == user.Id)
                        .FirstAsync();
                    _databaseContext.MonthlyTransactions.Remove(monthlyTransaction);
                    break;
            }
            await _databaseContext.SaveChangesAsync();

            if (request.Action == "remove") return new JsonResult(request);
            return new JsonResult(request.Value);
        }

        public async Task<User?> GetUserAsync() {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _databaseContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
