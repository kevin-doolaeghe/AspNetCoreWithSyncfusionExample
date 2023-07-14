using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Records {

    public class DetailsModel : PageModel {

        private readonly DatabaseContext _context;

        public DetailsModel(DatabaseContext context) {
            _context = context;
        }

        public Record Record { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(long? id) {
            if (id == null || _context.Records == null) {
                return NotFound();
            }

            var record = await _context.Records.FirstOrDefaultAsync(m => m.RecordId == id);
            if (record == null) {
                return NotFound();
            } else {
                Record = record;
            }
            return Page();
        }
    }
}
