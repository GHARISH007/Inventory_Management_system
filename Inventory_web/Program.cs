using Inventoryweb.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<InventoryDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews(); // ? Add this

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ? Map controllers for MVC
app.MapControllers(); // For [ApiController] routes

// ? Route for traditional MVC pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=File}/{action=Inventoryweb}/{id?}");

app.Run();

