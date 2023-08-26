using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System.Security.Claims;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Categories {

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(DatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor) {
            _databaseContext = databaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Category> DataSource { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync() {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            DataSource = await _databaseContext.Categories
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetNumberOfTransactionsAssociatedAsync(long id) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            var transactionsWithCategory = await _databaseContext.Transactions
                        .AsNoTracking()
                        .Where(x => x.UserId == user.Id)
                        .Where(x => x.CategoryId == id)
                        .ToListAsync();

            return new JsonResult(transactionsWithCategory.Count);
        }

        public async Task<IActionResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            DataSource = await _databaseContext.Categories
                .Where(x => x.UserId == user.Id)
                .ToListAsync();
            int count = DataSource.Cast<Category>().Count();

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

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<Category> request) {
            if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/");

            var user = await GetUserAsync();
            if (user == null) return Redirect("/");

            switch (request.Action) {
                case "insert":
                    request.Value.UserId = user.Id;
                    _databaseContext.Categories.Add(request.Value);
                    break;
                case "update":
                    _databaseContext.Categories.Update(request.Value);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    var transactionsWithCategory = await _databaseContext.Transactions
                        .AsNoTracking()
                        .Where(x => x.UserId == user.Id)
                        .Where(x => x.CategoryId == id)
                        .ToListAsync();
                    if (transactionsWithCategory.Count == 0) {
                        var category = await _databaseContext.Categories
                            .Where(x => x.UserId == user.Id)
                            .Where(x => x.Id == id)
                            .FirstAsync();
                        _databaseContext.Categories.Remove(category);
                    }
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
