using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Roles
            if (!await roleManager.RoleExistsAsync("Coordinador"))
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));

            // Usuario Coordinador
            var coordEmail = "coordinador@uni.edu";
            var coord = await userManager.FindByEmailAsync(coordEmail);
            if (coord == null)
            {
                coord = new IdentityUser { UserName = coordEmail, Email = coordEmail, EmailConfirmed = true };
                await userManager.CreateAsync(coord, "Admin123$");
                await userManager.AddToRoleAsync(coord, "Coordinador");
            }

            // Cursos iniciales
            if (!await context.Cursos.AnyAsync())
            {
                context.Cursos.AddRange(
                    new Curso { Codigo = "INF101", Nombre = "Programación I", Creditos = 3, CupoMaximo = 30, HorarioInicio = new TimeSpan(8,0,0), HorarioFin = new TimeSpan(10,0,0), Activo=true },
                    new Curso { Codigo = "MAT201", Nombre = "Matemática II", Creditos = 4, CupoMaximo = 25, HorarioInicio = new TimeSpan(10,0,0), HorarioFin = new TimeSpan(12,0,0), Activo=true },
                    new Curso { Codigo = "ADM301", Nombre = "Administración", Creditos = 2, CupoMaximo = 20, HorarioInicio = new TimeSpan(14,0,0), HorarioFin = new TimeSpan(16,0,0), Activo=true }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
