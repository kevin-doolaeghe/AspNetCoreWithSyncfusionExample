using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Services {

    public class DatabaseContext : DbContext {

        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Record> Records { get; set; }
    }
}
