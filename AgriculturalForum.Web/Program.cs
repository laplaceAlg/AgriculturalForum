using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AgriculturalForum.Web;
using AgriculturalForum.Web.Helpper;
using AgriculturalForum.Web.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages()
	.AddRazorRuntimeCompilation();
builder.Services.AddDbContext<KltnDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("dbKLTN")));
// Add services to the container.
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(option =>
				{
					option.Cookie.Name = "AuthenticationCookie";
					option.LoginPath = "/Account/Login";
					option.AccessDeniedPath = "/Account/AccessDenined";
					option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
				});



//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

/*app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
	);
	endpoints.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
});

ApplicationContext.Configure
(
	hostEnvironment: app.Services.GetService<IWebHostEnvironment>()
);
app.Run();
