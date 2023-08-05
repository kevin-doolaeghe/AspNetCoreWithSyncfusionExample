using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Services {

    public class DatabaseContext : DbContext {

        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
