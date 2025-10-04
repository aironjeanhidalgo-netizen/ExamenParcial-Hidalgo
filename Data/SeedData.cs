using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // === Crear rol Coordinador ===
            if (!await roleManager.RoleExistsAsync("Coordinador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));
            }

            // === Crear usuario Coordinador (si no existe) ===
            var email = "coordinador@demo.com";
            var password = "Coordinador123!";

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, password);
            }

            // === Asignar el rol Coordinador ===
            if (!await userManager.IsInRoleAsync(user, "Coordinador"))
            {
                await userManager.AddToRoleAsync(user, "Coordinador");
            }

            // === Insertar cursos iniciales ===
            if (!await context.Cursos.AnyAsync())
            {
                context.Cursos.AddRange(
                    new Curso
                    {
                        Codigo = "INF101",
                        Nombre = "Introducción a la Programación",
                        Creditos = 4,
                        CupoMaximo = 30,
                        HorarioInicio = new TimeSpan(8, 0, 0),
                        HorarioFin = new TimeSpan(10, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "MAT201",
                        Nombre = "Cálculo II",
                        Creditos = 5,
                        CupoMaximo = 25,
                        HorarioInicio = new TimeSpan(10, 0, 0),
                        HorarioFin = new TimeSpan(12, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "ADM301",
                        Nombre = "Gestión Empresarial",
                        Creditos = 3,
                        CupoMaximo = 20,
                        HorarioInicio = new TimeSpan(14, 0, 0),
                        HorarioFin = new TimeSpan(16, 0, 0),
                        Activo = true
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
