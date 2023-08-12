using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Services {

    public class DatabaseContext : IdentityDbContext<User> {

        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<MonthlyTransaction> MonthlyTransactions { get; set; }
    }
}
