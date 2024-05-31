using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HttpGet]
        public IActionResult ListaAdmin()
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



        [HttpGet]
        public IActionResult Create()
        {
            //necesitamos traer los datos del vehiculo a la vista
            //asi si el admin va a seleccionar el vehiculo, sepa cuales existen
            var vehiculos = _context.Vehiculo.ToList();
            ViewData["Vehiculos"] = new SelectList(vehiculos, "idVehiculo", "modeloVehiculo"); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Vehiculo vehiculo, Promocion promocion)
        {
            if (vehiculo == null || promocion == null)
            {
                return View(); // se valida que tenga datos sino lo mandamos de vuelta al formulario
            }

            // Verificar si ya existe una promoción para el vehículo
            var existePromocion = await _context.Promocion.FindAsync(vehiculo.idVehiculo);
            if (existePromocion != null)
            {
                // Actualizar la promoción existente
                existePromocion.precioPromocion = promocion.precioPromocion;
                existePromocion.lugarPromocion = promocion.lugarPromocion;

                _context.Update(existePromocion);
            }
            else
            {
                // Si no existe, crear una nueva promoción
                promocion.idVehiculo = vehiculo.idVehiculo;
                _context.Promocion.Add(promocion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



    }
}
