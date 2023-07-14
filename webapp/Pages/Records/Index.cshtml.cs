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
    public class IndexModel : PageModel
    {
        private readonly webapp.Services.DatabaseContext _context;

        public IndexModel(webapp.Services.DatabaseContext context)
        {
            _context = context;
        }

        public IList<Record> Record { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Records != null)
            {
                Record = await _context.Records.ToListAsync();
            }
        }
    }
}
