using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWebConcesionario.Controllers
{
    public class FacturaController : Controller
    {

        private readonly AppDbContext _context = null;

        public FacturaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var listado = await _context.Factura.ToListAsync();

            return View(listado);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Factura pFactura)
        {
            if (pFactura == null)
            {
                return View();
            }
            else
            {
                pFactura.idFactura = 0;

                _context.Factura.Add(pFactura);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");


            }

        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.Factura.FirstOrDefaultAsync(x => x.idFactura == id);

            return View(temp);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var temp = await _context.Factura.FirstOrDefaultAsync(x => x.idFactura == id);

            return View(temp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var temp = await _context.Factura.FirstOrDefaultAsync(x => x.idFactura == id);

            if (temp != null)
            {
                _context.Factura.Remove(temp);
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
