using BankLoyaltySystem.Data;
using BankLoyaltySystem.Models;
using BankLoyaltySystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext

builder.Services.AddDbContext<BankLoyaltySystem1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankConnection")));


// Register services
builder.Services.AddScoped<ITransactionService, TransactionService>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=EnterCustomerId}/{id?}");

app.Run();