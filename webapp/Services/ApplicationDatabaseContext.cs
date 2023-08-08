using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Services {

    public class ApplicationDatabaseContext : DbContext {

        public ApplicationDatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
