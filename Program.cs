using AzureAppINTEX.Data;
using AzureAppINTEX.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;
using AzureAppINTEX.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure App Configuration
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Customer>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 9; // Minimum length of the password
    options.Password.RequireDigit = true; // Requires at least one digit
    options.Password.RequireUppercase = true; // Requires at least one uppercase letter
    options.Password.RequireLowercase = true; // Requires at least one lowercase letter
    options.Password.RequireNonAlphanumeric = true; // Requires at least one non alphanumeric character
    options.Password.RequiredUniqueChars = 4; // Requires a number of unique characters
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Add Razor Pages services
builder.Services.AddRazorPages();

builder.Services.AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// IMPORTANT: Session and HttpContextAccessor services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    // Configure session settings if necessary, like timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Important for GDPR
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Scoped service registrations for repositories and Cart
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

// Register the EmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();

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
    app.UseHsts();
}


// Content Security Policy Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
                                 "default-src *; " + // Allow everything from all origins
                                 "script-src * 'unsafe-inline' 'unsafe-eval'; " + // Allow all scripts, including inline and eval
                                 "style-src * 'unsafe-inline'; " + // Allow all styles, including inline styles
                                 "img-src * data:; " + // Allow all images from any origin plus data URIs
                                 "connect-src *; " + // Allow connections to all URLs
                                 "font-src *; " + // Allow all fonts from any origin
                                 "object-src *; " + // Allow all object sources
                                 "frame-src *;"); // Allow all iframes
    await next.Invoke();
});






app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseCookiePolicy(new CookiePolicyOptions
{
    // This line ensures that the consent check is not needed for essential cookies.
    CheckConsentNeeded = context => false, // No consent check required
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
