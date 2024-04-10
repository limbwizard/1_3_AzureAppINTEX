using AzureAppINTEX.Data;
using AzureAppINTEX.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);



// Configure Azure App Configuration so connection string can be hidden
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var settings = config.Build();
    var appConfigConnectionString = settings["ConnectionStrings:AppConfig"];
    if (!string.IsNullOrEmpty(appConfigConnectionString))
    {
        config.AddAzureAppConfiguration(appConfigConnectionString);
    }
});




// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// This version works
//builder.Services.AddDefaultIdentity<Customer>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddDefaultIdentity<Customer>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();





builder.Services.AddControllersWithViews();

//GoogleSetup
//var services = builder.Services;
//var configuration = builder.Configuration;
//services.AddAuthentication().AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
//});

//Microsft setup
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
    microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
})
    .AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

services.AddSession();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddControllersWithViews();




// Assuming EFStoreRepository and EFOrderRepository implement IStoreRepository and IOrderRepository, respectively
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Scoped service for the Cart
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// For server-side Blazor (optional based on your application's needs)
builder.Services.AddServerSideBlazor();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



// csp security
app.Use(async (context, next) =>
{
    string csp = "default-src 'self'; " +
                 "img-src 'self' https: data:; " +
                 // Allow scripts from self, Stripe, jsDelivr, and also allow inline scripts
                 "script-src 'self' 'unsafe-inline' https://checkout.stripe.com https://cdn.jsdelivr.net; " +
                 // Allow styles from self and Google Fonts, and allow inline styles
                 "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                 "font-src 'self' https://fonts.gstatic.com; " + // Keep fonts restricted to self and Google Fonts
                                                                 // Allow frames from self and Stripe
                 "frame-src 'self' https://checkout.stripe.com; " +
                 "frame-ancestors 'self'; " +
                 // Allow connections to self, Stripe API, and loosen restrictions to include all websocket and http connections for development tools
                 "connect-src 'self' https://api.stripe.com wss: http:; " +
                 "object-src 'none'; " + // Disallow plugins (Flash, Silverlight, etc.)
                                         // Keep form actions within the same origin
                 "form-action 'self'; " +
                 "media-src 'self'; " + // Restrict media to self to minimize risks
                 "worker-src 'self'; " + // Allow worker scripts from the same origin only
                 "report-uri /csp-report;"; // Report CSP violations (ensure you have an endpoint configured to handle these reports)

    context.Response.Headers.Add("Content-Security-Policy", csp); // Apply the CSP
    await next();
});





// Enable session before routing
app.UseSession();


// COOKIES YUM
app.UseCookiePolicy(new CookiePolicyOptions
{
    CheckConsentNeeded = context => true,
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseStatusCodePages("text/plain", "Status code page, status code: {0}");

app.Use(async (context, next) =>
{
    var consentFeature = context.Features.Get<ITrackingConsentFeature>();
    if (!consentFeature?.CanTrack ?? false)
    {
        var cookieString = context.Request.Headers["Cookie"];
        if (string.IsNullOrEmpty(cookieString))
        {
            context.Response.Cookies.Append("cookieconsent_status", "deny");
        }
    }

    await next.Invoke();
});



app.UseRouting();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();
app.Run();
