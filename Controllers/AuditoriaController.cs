using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWebConcesionario.Controllers
{
    public class AuditoriaController : Controller
    {

        private readonly AppDbContext _context = null;

        public AuditoriaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var listado = await _context.RegistroAuditoria.ToListAsync();

            return View(listado);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.RegistroAuditoria.FirstOrDefaultAsync(x => x.idAuditoria == id);

            return View(temp);
        }





    }//ciere class
}//cierre namespace
