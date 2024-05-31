using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppWebConcesionario.Models;

namespace AppWebConcesionario.Controllers
{
    public class InventariosController : Controller
    {
        private readonly AppDbContext _context;

        public InventariosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Llamar al método de sincronización antes de cargar la vista.
            //este metodo solo hacia falta llamarlo una vez y listo, pero para evitar error futuros mejor dejarlo
            await SincronizarInventario(); 
            return View(await _context.Inventario.ToListAsync());
        }

        //puesto que ya se habian agregado datos a invenatrio
        //tuve que modificar la vista para que se sincronicen ambas tablas
        public async Task<IActionResult> SincronizarInventario()
        {
            var vehiculos = await _context.Vehiculo.ToListAsync(); // Obtener todos los vehículos.
            var inventarios = await _context.Inventario.ToListAsync(); // Obtener todos los inventarios.

            foreach (var vehiculo in vehiculos)
            {
                if (!inventarios.Any(i => i.idVehiculo == vehiculo.idVehiculo)) // Verifica si el vehículo ya tiene inventario.
                {
                    // Crear un nuevo inventario para el vehículo.
                    var nuevoInventario = new Inventario
                    {
                        idVehiculo = vehiculo.idVehiculo,
                        cantidadVehiculos = 1 // O cualquier lógica para determinar la cantidad inicial.
                    };
                    _context.Inventario.Add(nuevoInventario); // Agregar el nuevo inventario a la base de datos.
                }
            }
            await _context.SaveChangesAsync(); // Guardar todos los cambios en la base de datos.

            return RedirectToAction("Index"); // Redireccionar al índice del inventario o donde consideres apropiado.
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .FirstOrDefaultAsync(m => m.idVehiculo == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario.FindAsync(id);

            if (inventario == null)
            {
                return NotFound();
            }
//----------------------------------------------------------------------------------------------------------------
            //modifcar vehiculo
//----------------------------------------------------------------------------------------------------------------


            return View(inventario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idVehiculo,cantidadVehiculos")] Inventario inventario)
        {
            if (id != inventario.idVehiculo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExists(inventario.idVehiculo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inventario);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .FirstOrDefaultAsync(m => m.idVehiculo == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventario.Remove(inventario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
            return _context.Inventario.Any(e => e.idVehiculo == id);
        }
    }
}
