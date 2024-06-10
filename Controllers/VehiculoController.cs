using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<IActionResult> Create(List<IFormFile> files, [Bind] Vehiculo vehiculo, int cantidad)
        {
            if (vehiculo == null)
            {
                return View(); //se valida que tenga datos sino lo mandamos al formulario
            } else
            {
                vehiculo.idVehiculo = 0;


                //dependiendo de la cantidad
                //el estado del vehiculo es true o false
                if(cantidad <= 0)
                {
                    cantidad = 0;
                    vehiculo.estadoActivo = false;
                }
                else
                {
                    vehiculo.estadoActivo= true;
                }


                //todo el insert de la imagen
                if(files.Count > 0)
                {
                    string filePath = @"wwwroot\css\img\";

                    string fileName = "";

                    foreach(var formFile in files)
                    {
                        if(formFile.Length > 0)
                        {
                            fileName = vehiculo.modeloVehiculo + "_" + formFile.FileName;

                            fileName = fileName.Replace(" ", "_");
                            fileName = fileName.Replace("#", "_");
                            fileName = fileName.Replace("-", "_");

                            filePath += fileName;

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);

                                vehiculo.imagenUrl = @"/css/img/" + fileName;
                            }//cierre using
                        }//cierre if
                       

                       
                    }//cierre for
                }

                //en caso de que la imagen falle
                //sera "ND"
                else
                {
                    vehiculo.imagenUrl = "ND";
                }


                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Nuevo vehiculo",
                    tablaModificada = "Inventario",
                    fechaModificacion = DateTime.Now,
                    idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                };


                _context.RegistroAuditoria.Add(auditoria);
                await _context.SaveChangesAsync();

               

                _context.Vehiculo.Add(vehiculo);

                await _context.SaveChangesAsync();


                // Crear un registro en inventario para el vehículo recién creado con cantidad inicial de 1
                Inventario inventario = new Inventario { idVehiculo = vehiculo.idVehiculo, cantidadVehiculos = cantidad };
                _context.Inventario.Add(inventario);
                await _context.SaveChangesAsync(); // Guardar cambios del inventario.

                // Agregar el inventario a la base de datos
                _context.Inventario.Add(inventario);

                return RedirectToAction("Index","Inventarios");


            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.Vehiculo.FirstOrDefaultAsync(x => x.idVehiculo == id);

            return View(temp);
        }



    }//cierre class
}//cierre namespace
