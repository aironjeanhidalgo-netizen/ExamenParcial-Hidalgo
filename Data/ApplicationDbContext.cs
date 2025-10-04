using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Curso ===
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasIndex(c => c.Codigo).IsUnique();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");
                    t.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
                });
            });

            // === Matricula ===
            modelBuilder.Entity<Matricula>(entity =>
            {
                entity.HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique(); // no duplicadas
            });
        }
    }
}
