using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWebConcesionario.Controllers
{
    public class PromocionController : Controller
    {
        private readonly AppDbContext _context;

        public PromocionController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var vehiculosEnPromocion = _context.Promociones.Include(p => p.Vehiculo)
            .Select(p => new
            {
                p.Vehiculo.idVehiculo,
                p.Vehiculo.marcaVehiculo,
                p.Vehiculo.modeloVehiculo,
                p.Vehiculo.tipoCombustible,
                p.Vehiculo.precioVehiculo,
                p.Vehiculo.estadoActivo,
                p.Vehiculo.imagenUrl,
                p.precioPromocion,
                p.lugarPromocion
            })
            .ToList();

            return View(vehiculosEnPromocion);
        }
    }
}
