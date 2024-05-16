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
            return View(VehiculosEnPromocion());
        }

        //Devuelve la lista de vehiculos que se encuentran en promocion
        public List<(Vehiculo, Promocion)> VehiculosEnPromocion()
        {                         
            var vehiculosPromociones = new List<(Vehiculo, Promocion)>();

            foreach (var promocion in _context.Promocion.ToList())
            {
                var vehiculo = _context.Vehiculo.FirstOrDefault(v => v.idVehiculo == promocion.idVehiculo);
                vehiculosPromociones.Add((vehiculo, promocion));
            }

            return vehiculosPromociones;
        }
    }
}
