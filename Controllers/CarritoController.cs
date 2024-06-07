
//Fecha Creacion: 17-05-2024
//Funcion: Contener los modulos y metodos necesarios para funcionamiento de la clase
//Fecha Ultima Modificacion:28-05-2024
//Responsable: Christopher Rodríguez //Razon: Agregar metodo de delete.



using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppWebConcesionario.Controllers
{
    public class CarritoController : Controller
    {

        private static List<Carrito> _carrito = new List<Carrito>();


        //Se encarga de mostrar el carrito de compras
        public IActionResult Index()
        {
            return View(_carrito);
        }


        public static List<Carrito> GetCarrito()
        {
            return _carrito;
        }

        //Se encarga del anadir los vehiculos al carrito
        public async Task<IActionResult> Create(int idVehiculo, string marcaVehiculo, string modeloVehiculo, decimal precioVehiculo, string foto)
        {
            var temp = _carrito.FirstOrDefault(x => x.idVehiculo == idVehiculo);

            if (temp != null)
            {
                temp.precioVehiculo = precioVehiculo;
                temp.marcaVehiculo = marcaVehiculo;
                temp.modeloVehiculo = modeloVehiculo;
            }
            else
            {
                // Si el vehículo no está en el carrito, podrías agregarlo aquí si lo deseas.
                _carrito.Add(new Carrito
                {
                    idVehiculo = idVehiculo,
                    marcaVehiculo = marcaVehiculo,
                    modeloVehiculo = modeloVehiculo,
                    precioVehiculo = precioVehiculo,
                    foto = foto
                });
            }



            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            


            return RedirectToAction("Index");
        }

        //Se encarga de la eliminacion de un vehiculo en el carrito
        public IActionResult Delete(int idVehiculo)
        {
            var item = _carrito.FirstOrDefault(x => x.idVehiculo == idVehiculo);
            if (item != null)
            {
                _carrito.Remove(item);
            }

            return RedirectToAction("Index");
        }
 


    }//cierre class
}//cierrenamespace
