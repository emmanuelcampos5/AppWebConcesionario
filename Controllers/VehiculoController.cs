using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWebConcesionario.Controllers
{
    public class VehiculoController : Controller
    {

        private readonly AppDbContext _context = null;

        public VehiculoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var listado = await _context.Vehiculo.ToListAsync();

            return View(listado);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> files, [Bind] Vehiculo vehiculo)
        {
            if (vehiculo == null)
            {
                return View(); //se valida que tenga datos sino lo mandamos al formulario
            } else
            {
                vehiculo.idVehiculo = 0;

                _context.Vehiculo.Add(vehiculo);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");


            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.Vehiculo.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var temp = await _context.Vehiculo.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var temp = await _context.Vehiculo.FirstOrDefaultAsync(x => x.idVehiculo == id);

            if (temp != null)
            {
                _context.Vehiculo.Remove(temp);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var temp = await _context.Vehiculo.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Vehiculo vehiculo)
        {
            if(id == vehiculo.idVehiculo)
            {
                var temp = await _context.Vehiculo.FirstOrDefaultAsync(r => r.idVehiculo == id);

                _context.Vehiculo.Remove(temp);
                _context.Vehiculo.Add(vehiculo);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

    }//cierre class
}//cierre namespace
