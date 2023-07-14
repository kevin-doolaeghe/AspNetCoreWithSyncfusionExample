using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages.Records
{
    public class DeleteModel : PageModel
    {
        private readonly webapp.Services.DatabaseContext _context;

        public DeleteModel(webapp.Services.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Record Record { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Records == null)
            {
                return NotFound();
            }

            var record = await _context.Records.FirstOrDefaultAsync(m => m.RecordId == id);

            if (record == null)
            {
                return NotFound();
            }
            else 
            {
                Record = record;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null || _context.Records == null)
            {
                return NotFound();
            }
            var record = await _context.Records.FindAsync(id);

            if (record != null)
            {
                Record = record;
                _context.Records.Remove(Record);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
