using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppWebConcesionario.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Listado()
        {
            return View(_context.Usuario.ToList());
        }
    }
}
