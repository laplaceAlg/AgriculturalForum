using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;
using AgriculturalForum.Web.Extensions;
using Microsoft.Extensions.Options;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
#region Localizer
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(options =>
    options.DataAnnotationLocalizerProvider = (type, factory) =>
    {
        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
        return factory.Create(nameof(SharedResource), assemblyName.Name);
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("vi-VN")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: "vi-VN", uiCulture: "vi-VN");
    options.SupportedCultures = supportCultures;
    options.SupportedUICultures = supportCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});
#endregion

builder.Services.AddDbContext<KltnDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbKLTN")));

builder.Services.AddScoped<ICategoryPostRepository, CategoryPostRepository>();
builder.Services.AddScoped<ICategoryProductRepository, CategoryProductRepository>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IReplyRepository, ReplyRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services to the container.
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "AuthenticationCookie";
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/AccessDenied";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                });

// Cấu hình xác thực cho trang quản trị
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("AdminCookie", option =>
    {
        option.Cookie.Name = "AdminAuthenticationCookie";
        option.LoginPath = "/Admin/Account/Login";
        option.AccessDeniedPath = "/Admin/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    });


//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddNotyf(config =>
    { 
        config.DurationInSeconds = 5; 
        config.IsDismissable = true; 
        config.Position = NotyfPosition.BottomRight; 
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseNotyf();

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
