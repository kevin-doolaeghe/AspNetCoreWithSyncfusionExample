using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Base;
using webapp.Models;
using webapp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapp.Pages {

    [IgnoreAntiforgeryToken(Order = 1001)]
    public class RecordsModel : PageModel {

        private readonly DatabaseContext _context;

        public RecordsModel(DatabaseContext context) {
            _context = context;
        }

        public void OnGet() { }

        public async Task<JsonResult> OnPostDataSourceAsync([FromBody] DataManagerRequest dm) {
            IEnumerable<Record> data = await _context.Records.ToListAsync();
            int count = data.Count();
            
            DataOperations operation = new();
            data = operation.Execute(data, dm);
            return new JsonResult(new { result = data, count });
        }

        public async Task<IActionResult> OnPostCrudUpdateAsync([FromBody] CRUDModel<Record> request) {
            Record record = request.Value;
            switch (request.Action) {
                case "insert":
                    _context.Records.Add(record);
                    break;
                case "update":
                    _context.Records.Update(record);
                    break;
                case "remove":
                    _context.Records.Remove(record);
                    break;
            }
            await _context.SaveChangesAsync();

            IEnumerable<Record> data = await _context.Records.ToListAsync();
            return new JsonResult(new { result = data, count = data.Count() });
        }
    }
}
