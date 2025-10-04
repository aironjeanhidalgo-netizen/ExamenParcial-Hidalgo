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
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge(); // no autenticado

            // 1) Curso válido y activo
            var curso = await _db.Cursos.SingleOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null)
            {
                TempData["Error"] = "Curso no disponible.";
                return RedirectToAction("Details", "Catalogo", new { id = cursoId });
            }

            // 2) Ya matriculado (no Canceladas)
            var already = await _db.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != MatriculaEstado.Cancelada);
            if (already)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction("Details", "Catalogo", new { id = cursoId });
            }

            // 3) Cupo
            var inscritos = await _db.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != MatriculaEstado.Cancelada);
            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Error"] = "Cupo máximo alcanzado.";
                return RedirectToAction("Details", "Catalogo", new { id = cursoId });
            }

            // 4) Solapamiento horario con otras matrículas del usuario
            var userCursos = await _db.Matriculas
                .Where(m => m.UsuarioId == userId && m.Estado != MatriculaEstado.Cancelada)
                .Include(m => m.Curso)
                .Select(m => m.Curso!)
                .ToListAsync();

            foreach (var c in userCursos)
            {
                // overlap: start1 < end2 && start2 < end1
                if (curso.HorarioInicio < c.HorarioFin && c.HorarioInicio < curso.HorarioFin)
                {
                    TempData["Error"] = $"Conflicto de horario con el curso '{c.Nombre}'.";
                    return RedirectToAction("Details", "Catalogo", new { id = cursoId });
                }
            }

            // 5) Crear matrícula en estado Pendiente y guardar
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = userId,
                FechaRegistro = DateTime.UtcNow,
                Estado = MatriculaEstado.Pendiente
            };
            _db.Matriculas.Add(matricula);

            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = "Inscripción creada correctamente (Pendiente).";
            }
            catch (DbUpdateException)
            {
                // manejo simple de condición de carrera: volver a comprobar cupo
                var inscritos2 = await _db.Matriculas.CountAsync(m => m.CursoId == cursoId && m.Estado != MatriculaEstado.Cancelada);
                if (inscritos2 >= curso.CupoMaximo)
                    TempData["Error"] = "No se pudo inscribir: cupo alcanzado.";
                else
                    TempData["Error"] = "No se pudo inscribir. Intenta nuevamente.";
            }

            return RedirectToAction("Details", "Catalogo", new { id = cursoId });
        }
    }
}
