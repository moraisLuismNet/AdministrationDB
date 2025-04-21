using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AdministrationDataBase.Context;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<BDContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.Configure<CookieAuthenticationOptions>(options =>
{
    options.LoginPath = new PathString("/Login");
});

CookieAuthenticationOptions cookie = new CookieAuthenticationOptions();
cookie.LoginPath = new PathString("/Login");
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

var supportedCultures = new[] { new CultureInfo("en-US") };


builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorRuntimeCompilation();
var app = builder.Build();


var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MassagesCustomer}/{action=Index}");

app.Run();
