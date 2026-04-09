using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReferralTracker.Data;
using ReferralTracker.Models;
using ReferralTracker.Services.Implementations;
using ReferralTracker.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=referraltracker.db"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddScoped<IReferralService, ReferralService>();

var app = builder.Build();

// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "TalentTeam", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed admin user
    if (await userManager.FindByEmailAsync("admin@referraltracker.com") == null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@referraltracker.com",
            Email = "admin@referraltracker.com",
            FullName = "Admin User",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, "Password123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Seed test employee user
    if (await userManager.FindByEmailAsync("employee@referraltracker.com") == null)
    {
        var employee = new ApplicationUser
        {
            UserName = "employee@referraltracker.com",
            Email = "employee@referraltracker.com",
            FullName = "Test Employee",
            Department = "Development",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(employee, "Password123!");
        await userManager.AddToRoleAsync(employee, "Employee");
    }

    // Seed test recruiter user
    if (await userManager.FindByEmailAsync("recruiter@referraltracker.com") == null)
    {
        var recruiter = new ApplicationUser
        {
            UserName = "recruiter@referraltracker.com",
            Email = "recruiter@referraltracker.com",
            FullName = "Test Recruiter",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(recruiter, "Password123!");
        await userManager.AddToRoleAsync(recruiter, "TalentTeam");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
