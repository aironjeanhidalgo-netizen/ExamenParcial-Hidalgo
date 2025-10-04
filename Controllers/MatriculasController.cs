using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Data;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public MatriculasController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int cursoId)
        {
            var userId = _um.GetUserId(User);

            // 1) Ya matriculado (no doble matrícula) — incluye sólo estados no cancelados
            var exists = await _db.Matriculas.AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != MatriculaEstado.Cancelada);
            if (exists) return BadRequest("Ya estás matriculado en este curso.");

            // 2) Obtener curso activo
            var curso = await _db.Cursos.SingleOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null) return NotFound("Curso no encontrado o inactivo.");

            // 3) Contar matrículas activas (Pendiente o Confirmada)
            var inscritos = await _db.Matriculas.CountAsync(m => m.CursoId == cursoId && m.Estado != MatriculaEstado.Cancelada);
            if (inscritos >= curso.CupoMaximo) return BadRequest("Cupo máximo alcanzado.");

            // 4) Crear matricula dentro de try/catch (evita duplicados por índice único)
            var matricula = new Matricula { CursoId = cursoId, UsuarioId = userId, FechaRegistro = DateTime.UtcNow, Estado = MatriculaEstado.Pendiente };
            _db.Matriculas.Add(matricula);
            try
            {
                await _db.SaveChangesAsync();
                return Ok("Matriculado correctamente.");
            }
            catch (DbUpdateException)
            {
                // Puede deberse a condición de carrera o índice único: leer de nuevo
                return BadRequest("No se pudo matricular. Intenta nuevamente.");
            }
        }
    }
}
