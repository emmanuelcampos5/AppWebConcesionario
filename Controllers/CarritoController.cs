using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppWebConcesionario.Controllers
{
    public class CarritoController : Controller
    {



        private static List<Carrito> _carrito = new List<Carrito>();

        public IActionResult Index()
        {
            return View(_carrito);
        }

        private void UpdateCartCount()
        {
            ViewData["CartCount"] = _carrito.Count;
        }


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
            UpdateCartCount();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Index", "Vehiculo");
        }

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
