using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Data;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1️⃣ CONFIGURAR BASE DE DATOS
// ============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =====================================
// 2️⃣ CONFIGURAR IDENTITY Y ROLES
// =====================================
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // para pruebas
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// =====================================
// 3️⃣ MVC + RAZOR
// =====================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// =====================================
// 4️⃣ MIGRACIONES Y SEMILLA INICIAL
// =====================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    context.Database.Migrate();
    await SeedData.Initialize(context, userManager, roleManager);
}

// =====================================
// 5️⃣ CONFIGURAR PIPELINE
// =====================================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ❗ Necesario para Bootstrap/CSS/JS
app.UseRouting();
app.UseAuthentication(); // 🔥 IMPORTANTE
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
