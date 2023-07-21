using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Base;
using System.Text.Json;
using webapp.Models;
using webapp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapp.Pages.Records {

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        private readonly DatabaseContext _context;

        public IndexModel(ILogger<IndexModel> logger, DatabaseContext context) {
            _logger = logger;
            _context = context;
        }

        public IList<Record> Records { get; set; } = default!;

        public async Task OnGet() {
            Records = await _context.Records.ToListAsync();
        }

        public async Task<JsonResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            Records = await _context.Records.ToListAsync();
            return new JsonResult(new { result = Records, Records.Count });
        }

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<Record> request) {
            _logger.LogInformation($"Request: {JsonSerializer.Serialize(request)}");

            Record record = request.Value;
            switch (request.Action) {
                case "insert":
                    _context.Records.Add(record);
                    break;
                case "update":
                    _context.Records.Update(record);
                    break;
                case "remove":
                    long id = long.Parse($"{request.Key}");
                    Record? rec = _context.Records.Where(x => x.RecordId == id).FirstOrDefault();
                    if (rec != null) _context.Records.Remove(rec);
                    break;
            }
            await _context.SaveChangesAsync();

            Records = await _context.Records.ToListAsync();
            return new JsonResult(new { result = Records, count = Records.Count });
        }
    }
}
