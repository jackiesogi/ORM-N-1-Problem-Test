using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PersonalWebsite.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure ApplicationDbContext to use SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // Adjust password settings as needed
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Razor Pages (for login, registration, etc.)
builder.Services.AddRazorPages();

// Register the IHttpClientFactory
builder.Services.AddHttpClient();

// Build the app
var app = builder.Build();

app.Urls.Add("http://localhost:5015");

// Ensure database is created and apply any pending migrations.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    // Apply migrations or initialize database here if needed
}

// Configure the HTTP request pipeline.
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

//app use default DefaultConnection
app.Use(async (context, next) =>
{
    context.Items["DefaultConnection"] = app.Configuration.GetConnectionString("DefaultConnection");
    await next();
});

app.UseEndpoints(endpoints =>
{
    // 保留原本的路由設定
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
        
    // 支援 Razor Pages
    endpoints.MapRazorPages();
});

app.Run();

