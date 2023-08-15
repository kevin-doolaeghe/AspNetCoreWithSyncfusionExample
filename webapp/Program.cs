using webapp.Models;
using webapp.Services;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddDbContext<DatabaseContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString"))
);
builder.Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<DatabaseContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2V1hhQlJMfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Ud0FjUX5fcXdcRWhf");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
} else {
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.EnsureCreated();
    // context.Database.Migrate();

    if (app.Environment.IsDevelopment()) {
        if (!context.Categories.Any()) {
            var categories = new List<Category>() {
                new Category { Icon = "💰", Name = "Salary" },
                new Category { Icon = "🍔", Name = "Food" },
                new Category { Icon = "🎁", Name = "Pleasure" },
                new Category { Icon = "⚽", Name = "Activity" },
                new Category { Icon = "💸", Name = "Savings" },
            };
            context.Categories.AddRange(categories);
        }
        if (!context.Transactions.Any()) {
            var transactions = new List<Transaction>() {
                new Transaction { Note = "Salary 06-2023", CategoryId = 1, Amount = 1000, Date = DateTime.Parse("2023-06-28"), IsDone = true },
                new Transaction { Note = "Food", CategoryId = 2, Amount = -100, Date = DateTime.Parse("2023-07-02"), IsDone = true },
                new Transaction { Note = "Romain's birthday", CategoryId = 3, Amount = -80, Date = DateTime.Parse("2023-07-06"), IsDone = false },
                new Transaction { Note = "Movie", CategoryId = 4, Amount = -10, Date = DateTime.Parse("2023-07-09"), IsDone = true },
                new Transaction { Note = "Monthly transaction", CategoryId = 5, Amount = -200, Date = DateTime.Parse("2023-07-10"), IsDone = false },
            };
            context.Transactions.AddRange(transactions);
        }
        context.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

var supportedCultures = new[] {
    new CultureInfo("en-US"),
    new CultureInfo("fr-FR")
};
app.UseRequestLocalization(new RequestLocalizationOptions {
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.Run();
