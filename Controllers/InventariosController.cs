﻿
//Fecha Creacion: 14-05-2024
//Funcion: Contener los modulos y metodos necesarios para funcionamiento de la clase
//Fecha Ultima Modificacion:27-05-2024
//Responsable: Dylan Barrantes //Razon: Realizar ajuste en el metodo sincronizar inventario.



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppWebConcesionario.Models;
using System.Security.Claims;

namespace AppWebConcesionario.Controllers
{
    public class InventariosController : Controller

    {
        private readonly AppDbContext _context; //AppDbContext: representa el contexto de la base de datos.

        public InventariosController(AppDbContext context)
        {
            _context = context;
        }

        //Muestra la lista del inventario
        public async Task<IActionResult> Index()
        {
            // Llamar al método de sincronización antes de cargar la vista.
            //este metodo solo hacia falta llamarlo una vez y listo, pero para evitar error futuros mejor dejarlo
            await SincronizarInventario();
            var inventarios = await _context.Inventario.ToListAsync();
            var vehiculos = await _context.Vehiculo.ToListAsync();

            var inventariosVehiculos = from inventario in inventarios
                                       join vehiculo in vehiculos on inventario.idVehiculo equals vehiculo.idVehiculo
                                       select (inventario, vehiculo);

            return View(inventariosVehiculos.ToList());
        }

        //puesto que ya se habian agregado datos al inventario
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


        //Muestra formulario necesario para la edicion del inventario
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


            return View(inventario);
        }


        //Recolecta y envia la informacion necesaria para editar el inventario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Inventario inventario)
       {
            if (id != inventario.idVehiculo)
            {
               return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                     //actualiza el inventario
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();

                    //buscar el vehiculo que se relaciona al id del inventario
                    var vehiculo = await _context.Vehiculo.FindAsync(inventario.idVehiculo);

                    if (vehiculo != null)
                    {
                        if(inventario.cantidadVehiculos <= 0)
                        {
                            inventario.cantidadVehiculos = 0;
                            vehiculo.estadoActivo = false;
                        }
                        else
                        {
                            vehiculo.estadoActivo = true;
                        }

                        await _context.SaveChangesAsync();
                    }



                    var auditoria = new RegistroAuditoria
                    {
                        idAuditoria = 0,
                        descripcion = "Cantidad vehiculos modificada",
                        tablaModificada = "Inventario",
                        fechaModificacion = DateTime.Now,
                        idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                    };
                    _context.RegistroAuditoria.Add(auditoria);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExiste(inventario.idVehiculo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

               
            }

            return RedirectToAction("Index");
        }

        //Verifica la existencia en inventario
        private bool InventarioExiste(int id)
        {
            return _context.Inventario.Any(e => e.idVehiculo == id);
        }



    }//cierre class
}//cierre namespace
