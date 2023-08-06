using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;

        public IndexModel(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public double CurrentBalance { get; set; }

        public double RealBalance { get; set; }

        public double Cashflow { get; set; }

        public IList<ColumnChartData> TransactionsByStatusData { get; set; } = default!;

        public IList<PieChartData> ExpensesByCategoryData { get; set; } = default!;

        public IList<SplineChartData> ExpenseVsIncomeData { get; set; } = default!;

        public IList<Transaction> RecentTransactions { get; set; } = default!;

        public async Task OnGetAsync() {
            var transactions = await _databaseContext.Transactions
                .AsNoTracking()
                .Include(x => x.Category)
                .ToListAsync();

            RecentTransactions = transactions
                .OrderByDescending(x => x.Date)
                .Take(20)
                .ToList();

            CurrentBalance = transactions.Where(x => x.IsDone).Select(x => x.Amount).Sum();
            RealBalance = transactions.Select(x => x.Amount).Sum();
            Cashflow = double.Abs(RealBalance - CurrentBalance);

            // Line chart data
            TransactionsByStatusData = transactions
                .GroupBy(x => x.CategoryId)
                .Select(x => new ColumnChartData() {
                    Category = x.First().Category?.Description,
                    Pending = x.Where(y => !y.IsDone).Count(),
                    Closed = x.Where(y => y.IsDone).Count(),
                })
                .ToList();

            // Pie chart data
            ExpensesByCategoryData = transactions
                .Where(x => x.Amount < 0)
                .GroupBy(x => x.CategoryId)
                .Select(x => new PieChartData() {
                    Category = x.First().Category?.Description,
                    Value = x.Sum(x => -x.Amount),
                    FormattedValue = x.Sum(x => -x.Amount).ToString("C2", CultureInfo.CurrentCulture),
                })
                .ToList();

            // Spline chart data
            var initialBalance = transactions
                .OrderByDescending(x => x.Date)
                .Skip(20)
                .Select(x => x.Amount)
                .Sum();

            ExpenseVsIncomeData = RecentTransactions
                .OrderBy(x => x.Date)
                .GroupBy(x => x.Date)
                .Select(x => new SplineChartData() {
                    Date = x.First().Date.ToString("dd-MMM"),
                    Income = x.Where(y => y.Amount >= 0).Sum(y => y.Amount),
                    Expense = x.Where(y => y.Amount < 0).Sum(y => -y.Amount),
                })
                .ToList();

            for (int i = 0; i < ExpenseVsIncomeData.Count; i++) {
                if (i == 0) {
                    if (initialBalance < 0) ExpenseVsIncomeData[i].Expense += initialBalance;
                    else ExpenseVsIncomeData[i].Income += initialBalance;
                } else {
                    ExpenseVsIncomeData[i].Income += ExpenseVsIncomeData[i - 1].Income;
                    ExpenseVsIncomeData[i].Expense += ExpenseVsIncomeData[i - 1].Expense;
                }
                ExpenseVsIncomeData[i].Balance = ExpenseVsIncomeData[i].Income - ExpenseVsIncomeData[i].Expense;
            }
        }

        public class ColumnChartData {

            public string? Category { get; set; }

            public double Pending { get; set; }

            public double Closed { get; set; }
        }

        public class PieChartData {

            public string? Category { get; set; }

            public double Value { get; set; }

            public string? FormattedValue { get; set; }
        }

        public class SplineChartData {

            public string? Date { get; set; }

            public double Balance { get; set; }

            public double Income { get; set; }

            public double Expense { get; set; }
        }
    }
}
