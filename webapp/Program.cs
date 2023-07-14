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

    if (!context.Records.Any()) {
        var records = new Record[] {
            new Record { Note = "Salary 06-2023", Category = "💰 Salary", Amount = 1000, Date = DateTime.Parse("2023-06-28"), IsDone = true },
            new Record { Note = "Food", Category = "🍔 Food", Amount = -100, Date = DateTime.Parse("2023-07-02"), IsDone = true },
            new Record { Note = "Romain's birthday", Category = "🎁 Pleasure", Amount = -80, Date = DateTime.Parse("2023-07-06"), IsDone = false },
            new Record { Note = "Movie", Category = "⚽ Activity", Amount = -10, Date = DateTime.Parse("2023-07-09"), IsDone = true },
            new Record { Note = "Monthly transaction", Category = "💸 Savings", Amount = -200, Date = DateTime.Parse("2023-07-10"), IsDone = false },
        };

        context.Records.AddRange(records);
        context.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();