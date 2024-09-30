using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShoppingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IHttpContextAccessor to access HttpContext within controllers
builder.Services.AddHttpContextAccessor();

// Add distributed memory cache for session handling
builder.Services.AddDistributedMemoryCache();

// Add session configuration (with 30 minutes idle timeout)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Ensure the session cookie is essential
});

// Add authentication using cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to login page if not authenticated
        options.LogoutPath = "/Account/Logout"; // Redirect to logout page when logging out
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to an error page for unauthorized access
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
        options.SlidingExpiration = true; // Renew cookie if the user is active
    });

// Configure the database connection (replace with your database settings)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=DESKTOP-UB4UJGD;Database=TaskDB;Trusted_Connection=True;TrustServerCertificate=True;");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enable HSTS in production environments
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use routing for request handling
app.UseRouting();

// Authentication middleware
app.UseAuthentication();

// Session middleware should come after authentication
app.UseSession();

// Authorization middleware
app.UseAuthorization();


// Configure default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the app
app.Run();
