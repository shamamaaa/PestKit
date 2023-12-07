
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PestKit.DAL;
using PestKit.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 8;
    option.Password.RequireNonAlphanumeric = false;

    option.User.RequireUniqueEmail = true;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
}
).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var app = builder.Build();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseEndpoints(endpoints =>
endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=home}/{action=index}/{id?}")
);

app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");


app.Run();

