using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using TrendifyV1.Core;
using TrendifyV1.Core.Implementations;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TrendifyV1DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LaptopConnction")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TrendifyV1DbContext>();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TrendifyV1DbContext>();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddRoles<IdentityRole<Guid>>() 
//    .AddEntityFrameworkStores<TrendifyV1DbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<TrendifyV1DbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

#region Services
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IBasketService, BasketService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
#endregion

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//app.MapGet("/", () => "App is running");

using (var scope = app.Services.CreateScope())
{
    await TrendifyV1.Data.AdminSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.Run();
