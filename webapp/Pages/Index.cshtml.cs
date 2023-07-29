using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Charts;
using Syncfusion.EJ2.Linq;
using System.Globalization;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        private readonly DatabaseContext _databaseContext;

        public IndexModel(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public string? CurrentBalance { get; set; }

        public string? RealBalance { get; set; }

        public string? Cashflow { get; set; }

        public IList<Record> RecentRecords { get; set; } = default!;

        public IList<ColumnChartData> ColumnChartData1 { get; set; } = default!;

        public IList<PieChartData> PieChartData1 { get; set; } = default!;

        public IList<SplineChartData> SplineChartData1 { get; set; } = default!;

        public async Task OnGetAsync() {
            if (_databaseContext.Records != null) {
                RecentRecords = await _databaseContext.Records
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .Take(15)
                    .ToListAsync();

                var records = await _databaseContext.Records
                    .AsNoTracking()
                    .ToListAsync();

                var realBalance = records.Select(x => x.Amount).Sum();
                var currentBalance = records.Where(x => x.IsDone).Select(x => x.Amount).Sum();
                var cultureInfo = new CultureInfo("fr-FR");
                RealBalance = realBalance.ToString("C2", cultureInfo);
                CurrentBalance = currentBalance.ToString("C2", cultureInfo);
                Cashflow = (realBalance - currentBalance).ToString("C2", cultureInfo);

                ColumnChartData1 = records
                    .GroupBy(x => x.Category)
                    .Select(x => new ColumnChartData() {
                        Period = x.First().Category,
                        Value1 = x.Where(y => !y.IsDone).Count(),
                        Value2 = x.Where(y => y.IsDone).Count(),
                    })
                    .ToList();

                PieChartData1 = records
                    .Where(x => x.Amount < 0)
                    .GroupBy(x => x.Category)
                    .Select(x => new PieChartData() {
                        Description = x.First().Category,
                        Value = x.Sum(x => -x.Amount),
                        FormattedValue = x.Sum(x => -x.Amount).ToString("C2", cultureInfo),
                    })
                    .ToList();

                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                SplineChartData1 = records
                    .Where(x => x.Date >= firstDayOfMonth)
                    .OrderBy(x => x.Date)
                    .GroupBy(x => x.Date)
                    .Select(x => new SplineChartData() {
                        Period = x.First().Date.ToString("dd-MMM"),
                        Value2 = x.Where(y => y.Amount >= 0).Sum(y => y.Amount),
                        Value3 = x.Where(y => y.Amount < 0).Sum(y => -y.Amount),
                    })
                    .ToList();

                var initialBalance = records
                    .Where(x => x.Date < firstDayOfMonth)
                    .Select(x => x.Amount)
                    .Sum();
                for (int i = 0; i < SplineChartData1.Count; i++) {
                    if (i == 0) {
                        if (initialBalance < 0) SplineChartData1[i].Value3 += initialBalance;
                        else SplineChartData1[i].Value2 += initialBalance;
                    } else {
                        SplineChartData1[i].Value2 += SplineChartData1[i - 1].Value2;
                        SplineChartData1[i].Value3 += SplineChartData1[i - 1].Value3;
                    }
                    SplineChartData1[i].Value1 = SplineChartData1[i].Value2 - SplineChartData1[i].Value3;
                }
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
