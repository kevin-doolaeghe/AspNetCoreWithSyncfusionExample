﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Models;
using webapp.Services;

namespace webapp.Pages {

    public class IndexModel : PageModel {

        private readonly DatabaseContext _context;

        public IndexModel(DatabaseContext context) {
            _context = context;
        }

        public IList<Record> Records { get; set; } = default!;

        public async Task OnGetAsync() {
            if (_context.Records != null) {
                Records = await _context.Records.ToListAsync();
            }
        }
    }
}
