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

        public IList<Record> RecentRecords { get; set; } = default!;

        public async Task OnGetAsync() {
            var records = await _databaseContext.Records
                .AsNoTracking()
                .ToListAsync();

            RecentRecords = records
                .OrderByDescending(x => x.Date)
                .Take(20)
                .ToList();

            RealBalance = records.Select(x => x.Amount).Sum();
            CurrentBalance = records.Where(x => x.IsDone).Select(x => x.Amount).Sum();
            Cashflow = double.Abs(RealBalance - CurrentBalance);

            // Line chart data
            TransactionsByStatusData = records
                .GroupBy(x => x.Category)
                .Select(x => new ColumnChartData() {
                    Period = x.First().Category,
                    Value1 = x.Where(y => !y.IsDone).Count(),
                    Value2 = x.Where(y => y.IsDone).Count(),
                })
                .ToList();

            // Pie chart data
            ExpensesByCategoryData = records
                .Where(x => x.Amount < 0)
                .GroupBy(x => x.Category)
                .Select(x => new PieChartData() {
                    Description = x.First().Category,
                    Value = x.Sum(x => -x.Amount),
                    FormattedValue = x.Sum(x => -x.Amount).ToString("C2", CultureInfo.CurrentCulture),
                })
                .ToList();

            // Spline chart data
            var initialBalance = records
                .OrderByDescending(x => x.Date)
                .Skip(20)
                .Select(x => x.Amount)
                .Sum();

            ExpenseVsIncomeData = RecentRecords
                .OrderBy(x => x.Date)
                .GroupBy(x => x.Date)
                .Select(x => new SplineChartData() {
                    Period = x.First().Date.ToString("dd-MMM"),
                    Value2 = x.Where(y => y.Amount >= 0).Sum(y => y.Amount),
                    Value3 = x.Where(y => y.Amount < 0).Sum(y => -y.Amount),
                })
                .ToList();

            for (int i = 0; i < ExpenseVsIncomeData.Count; i++) {
                if (i == 0) {
                    if (initialBalance < 0) ExpenseVsIncomeData[i].Value3 += initialBalance;
                    else ExpenseVsIncomeData[i].Value2 += initialBalance;
                } else {
                    ExpenseVsIncomeData[i].Value2 += ExpenseVsIncomeData[i - 1].Value2;
                    ExpenseVsIncomeData[i].Value3 += ExpenseVsIncomeData[i - 1].Value3;
                }
                ExpenseVsIncomeData[i].Value1 = ExpenseVsIncomeData[i].Value2 - ExpenseVsIncomeData[i].Value3;
            }
        }

        public class ColumnChartData {

            public string? Period { get; set; }

            public double Value1 { get; set; }

            public double Value2 { get; set; }
        }

        public class PieChartData {

            public string? Description { get; set; }

            public double Value { get; set; }

            public string? FormattedValue { get; set; }
        }

        public class SplineChartData {

            public string? Period { get; set; }

            public double Value1 { get; set; }

            public double Value2 { get; set; }

            public double Value3 { get; set; }
        }
    }
}
