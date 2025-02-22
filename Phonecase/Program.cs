using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔹 Add Database Context
builder.Services.AddDbContext<PhoneCaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IManagementRepository, SqlManagementRepository>();
builder.Services.AddScoped<IProductRepository, SqlProductRepositoy>();
builder.Services.AddScoped<IVendorRepository, SqlVendorRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Leisure}/{action=Dashboard}/{id?}")
    .WithStaticAssets();


app.Run();
