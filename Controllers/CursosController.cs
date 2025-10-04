using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JEAN_HIDALGO_EXAMEN_PARCIAL.Data;
using System.Threading.Tasks;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CursosController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Cursos
        public async Task<IActionResult> Index()
        {
            var cursosActivos = await _db.Cursos
                .Where(c => c.Activo)
                .ToListAsync();
            return View(cursosActivos);
        }
    }
}

