using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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



        [HttpGet]
        public IActionResult Create()
        {
            //necesitamos traer los datos del vehiculo a la vista
            //asi si el admin va a seleccionar el vehiculo, sepa cuales existen
            ViewData["Vehiculos"] = new SelectList(_context.Vehiculo, "idVehiculo", "modeloVehiculo"); 


            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind]Promocion promocion)
        {
            if (promocion == null)
            {
                return View();
            }

            // Encontrar el vehículo correspondiente en la base de datos usando el idVehiculo seleccionado
            var vehiculo = await _context.Vehiculo.FindAsync(promocion.idVehiculo);
            if (vehiculo == null)
            {
                ModelState.AddModelError(string.Empty, "El vehículo no se encuentra.");
                ViewData["Vehiculos"] = new SelectList(_context.Vehiculo, "idVehiculo", "modeloVehiculo");
                return View(promocion);
            }

            // Verificar que el precio de la promoción sea menor al precio del vehículo
            if (promocion.precioPromocion >= vehiculo.precioVehiculo)
            {
                ModelState.AddModelError(string.Empty, "El precio de la promoción debe ser menor al precio del vehículo.");
                ViewData["Vehiculos"] = new SelectList(_context.Vehiculo, "idVehiculo", "modeloVehiculo");
                return View(promocion);
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
                _context.Promocion.Add(promocion);
            }


            await _context.SaveChangesAsync();

            // Obtener la lista de usuarios
            var usuarios = await _context.Usuario.Where(u => u.lugarResidencia == promocion.lugarPromocion).ToListAsync();

            Email email = new Email();


            var auditoria = new RegistroAuditoria
            {
                idAuditoria = 0,
                descripcion = "Creacion de promocion",
                tablaModificada = "Promocion",
                fechaModificacion = DateTime.Now,
                idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
            };
            _context.RegistroAuditoria.Add(auditoria);
            await _context.SaveChangesAsync();

            // Llamar al método para enviar el correo electrónico
            email.EnviarPromocion(usuarios, vehiculo, promocion);

            return RedirectToAction("Index");
        }


        //Borrar promociones-------------------------------

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var temp = await _context.Promocion.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var temp = await _context.Promocion.FirstOrDefaultAsync(x => x.idVehiculo == id);

            if (temp != null)
            {
                _context.Promocion.Remove(temp);
                await _context.SaveChangesAsync();

                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Eliminacion de Promocion",
                    tablaModificada = "Promocion",
                    fechaModificacion = DateTime.Now,
                    idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                };

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }


        //Editar promociones-------------------------------------
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var temp = await _context.Promocion.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Promocion promocion)
        {
            if (id == promocion.idVehiculo)
            {
                var temp = await _context.Promocion.FirstOrDefaultAsync(r => r.idVehiculo == id);


                var vehiculo = await _context.Vehiculo.FindAsync(promocion.idVehiculo);
                if (promocion.precioPromocion >= vehiculo.precioVehiculo)
                {
                    ModelState.AddModelError(string.Empty, "El precio de la promoción debe ser menor al precio del vehículo.");
                    return View(promocion);
                }

                _context.Promocion.Remove(temp);
                _context.Promocion.Add(promocion);


                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Edicion de promocion",
                    tablaModificada = "Promocion",
                    fechaModificacion = DateTime.Now,
                    idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                };

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
