using Microsoft.EntityFrameworkCore;
using System;
using webapp.Models;
using webapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DatabaseContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString"))
);
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

    /*
    if (!context.Categories.Any()) {
        var categories = new Category[] {
            new Category { Icon = "🍔", Name = "Food", Type = CategoryType.Expense},
            new Category { Icon = "💰", Name = "Salary", Type = CategoryType.Income},
            new Category { Icon = "🎁", Name = "Pleasure", Type = CategoryType.Expense},
            new Category { Icon = "⚽", Name = "Activity", Type = CategoryType.Expense},
            new Category { Icon = "💸", Name = "Savings", Type = CategoryType.Income},
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }
    */
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();