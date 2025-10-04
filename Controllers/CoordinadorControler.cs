using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Data;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Models;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CoordinadorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // LISTAR cursos (panel principal)
        public async Task<IActionResult> Index()
        {
            var cursos = await _db.Cursos.OrderBy(c => c.Nombre).ToListAsync();
            return View(cursos);
        }

        // CREAR - GET
        public IActionResult Create() => View(new Curso());

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso model)
        {
            if (!ModelState.IsValid) return View(model);

            // validaciones adicionales (ej: HorarioInicio < HorarioFin)
            if (model.HorarioInicio >= model.HorarioFin)
            {
                ModelState.AddModelError("", "HorarioInicio debe ser menor que HorarioFin.");
                return View(model);
            }
            _db.Cursos.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDITAR - GET
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            if (model.HorarioInicio >= model.HorarioFin)
            {
                ModelState.AddModelError("", "HorarioInicio debe ser menor que HorarioFin.");
                return View(model);
            }

            _db.Cursos.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DESACTIVAR (soft delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            curso.Activo = false;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DETALLES (incluye matrículas)
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            var matriculas = await _db.Matriculas
                .Where(m => m.CursoId == id)
                .OrderByDescending(m => m.FechaRegistro)
                .ToListAsync();

            // Obtener emails (opcional) desde Identity Users
            var userIds = matriculas.Select(m => m.UsuarioId).Distinct().ToList();
            var users = await _db.Users.Where(u => userIds.Contains(u.Id))
                                      .ToDictionaryAsync(u => u.Id, u => u.Email);

            ViewBag.Users = users; // ViewBag[usuarioId] -> email
            return View((curso, matriculas));
        }

        // CONFIRMAR matrícula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int matriculaId)
        {
            var m = await _db.Matriculas.FindAsync(matriculaId);
            if (m == null) return NotFound();
            m.Estado = MatriculaEstado.Confirmada;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = m.CursoId });
        }

        // CANCELAR matrícula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int matriculaId)
        {
            var m = await _db.Matriculas.FindAsync(matriculaId);
            if (m == null) return NotFound();
            m.Estado = MatriculaEstado.Cancelada;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = m.CursoId });
        }
    }
}
