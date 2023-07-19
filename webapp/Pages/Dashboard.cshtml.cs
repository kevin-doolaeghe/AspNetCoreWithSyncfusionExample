using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Charts;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages {

    public class DashboardModel : PageModel {

        private readonly DatabaseContext _context;

        public DashboardModel(DatabaseContext context) {
            _context = context;
        }

        public double ActualBalance { get; set; }

        public double Balance { get; set; }

        public IList<Object> ColumnChartData { get; set; } = default!;

        public IList<Object> PieChartData { get; set; } = default!;

        public IList<Object> SplineChartData { get; set; } = default!;

        public async Task OnGetAsync() {
            if (_context.Records != null) {
                var records = await _context.Records.ToListAsync();

                Balance = records.Select(x => x.Amount).Sum();
                ActualBalance = records.Where(x => x.IsDone).Select(x => x.Amount).Sum();

                ColumnChartData = new List<Object>() {
                    new { Period = "2017", OnlinePercentage = 60, RetailPercentage = 40 },
                    new { Period = "2018", OnlinePercentage = 56, RetailPercentage = 44 },
                    new { Period = "2019", OnlinePercentage = 71, RetailPercentage = 29 },
                    new { Period = "2020", OnlinePercentage = 85, RetailPercentage = 15 },
                    new { Period = "2021", OnlinePercentage = 73, RetailPercentage = 27 }
                };

                PieChartData = new List<Object>() {
                    new { Product = "TV : 30 (12%)",     Percentage = 12, Text = "TV, 30<br>12%" },
                    new { Product = "PC : 20 (8%)",      Percentage = 8,  Text = "PC, 20<br>8%" },
                    new { Product = "Laptop : 40 (16%)", Percentage = 16, Text = "Laptop, 40<br>16%" },
                    new { Product = "Mobile : 90 (36%)", Percentage = 36, Text = "Mobile, 90<br>36%" },
                    new { Product = "Camera : 27 (11%)", Percentage = 11, Text = "Camera, 27<br>11%" }
                };
                
                SplineChartData = new List<Object>() {
                    new { Period = "Jan", OnlinePercentage = 3600, RetailPercentage = 6400 },
                    new { Period = "Feb", OnlinePercentage = 6200, RetailPercentage = 5300 },
                    new { Period = "Mar", OnlinePercentage = 8100, RetailPercentage = 4900 },
                    new { Period = "Apr", OnlinePercentage = 5900, RetailPercentage = 5300 },
                    new { Period = "May", OnlinePercentage = 8900, RetailPercentage = 4200 },
                    new { Period = "Jun", OnlinePercentage = 7200, RetailPercentage = 6500 },
                    new { Period = "Jul", OnlinePercentage = 4300, RetailPercentage = 7900 },
                    new { Period = "Aug", OnlinePercentage = 4600, RetailPercentage = 3800 },
                    new { Period = "Sep", OnlinePercentage = 5500, RetailPercentage = 6800 },
                    new { Period = "Oct", OnlinePercentage = 6350, RetailPercentage = 3400 },
                    new { Period = "Nov", OnlinePercentage = 5700, RetailPercentage = 6400 },
                    new { Period = "Dec", OnlinePercentage = 8000, RetailPercentage = 6800 }
                };
            }
        }
    }
}
