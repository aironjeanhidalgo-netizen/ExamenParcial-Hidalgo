using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Data;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CatalogoController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CourseFilterViewModel filter)
        {
            var q = _db.Cursos.AsQueryable().Where(c => c.Activo);

            if (!string.IsNullOrWhiteSpace(filter.Nombre))
                q = q.Where(c => EF.Functions.Like(c.Nombre, $"%{filter.Nombre}%"));

            if (filter.CreditosMin.HasValue)
                q = q.Where(c => c.Creditos >= filter.CreditosMin.Value);

            if (filter.CreditosMax.HasValue)
                q = q.Where(c => c.Creditos <= filter.CreditosMax.Value);

            if (filter.CreditosMin.HasValue && filter.CreditosMax.HasValue && filter.CreditosMin > filter.CreditosMax)
                ModelState.AddModelError("CreditosMax", "Rango de créditos inválido.");

            if (filter.HorarioInicio.HasValue && filter.HorarioFin.HasValue)
            {
                if (filter.HorarioInicio.Value >= filter.HorarioFin.Value)
                {
                    ModelState.AddModelError("HorarioFin", "HorarioFin debe ser mayor que HorarioInicio.");
                }
                else
                {
                    // Filtrar cursos cuyo horario esté dentro del rango solicitado
                    q = q.Where(c => c.HorarioInicio >= filter.HorarioInicio.Value && c.HorarioFin <= filter.HorarioFin.Value);
                }
            }

            filter.Resultados = await q.OrderBy(c => c.Nombre).ToListAsync();
            return View(filter);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _db.Cursos.SingleOrDefaultAsync(c => c.Id == id && c.Activo);
            if (curso == null) return NotFound();
            return View(curso);
        }
    }
}
