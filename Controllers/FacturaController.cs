using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                //traemos los datos del usuario que inicio sesion
                //solo se puede comprar si se ha iniciado sesion
                var userId = int.Parse(User.FindFirstValue("idUsuario"));
                var userName = User.FindFirstValue("NombreUsuario");

                // Obtener el carrito de compras del usuario (aquí debes implementar cómo obtienes el carrito del usuario)
                var carrito = CarritoController.GetCarrito();

                //validamos que el carrito no venga vacio
                if (carrito == null || !carrito.Any())
                {
                    TempData["Mensaje"] = "Tu carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                
                double montoDescuento = 0; // Calcular descuento si aplica
               

                
               

                //hacemos un recorrido de los carros en el carrito
                foreach (var item in carrito)
                {

                    //esto da error, por mas que el idfactura sea una llave foranea el necesita de una primaria y exclusiva de el
                    //ejemplo un IdDetalle
                    //sumando eso se podria hacer sin ningun problema, pero de mi parte no sabria como solucionarlo
                    //claro, si solo se compra un carro no hay problema, el problema es cuando se compra mas

                    //var detalleFactura = new Det_Factura
                    //{
                    //    idFactura = nuevaFactura.idFactura,
                    //    idVehiculo = item.idVehiculo,
                    //    precioUnitario = (double)item.precioVehiculo,
                    //    cantidad = 1,
                    //    subtotal = (double)item.precioVehiculo,
                    //    montoDescuento = 0,
                    //    montoImpuesto = (double)item.precioVehiculo * 0.13
                    //};

                    //recorremos el inventario para encontrar en el mismo, los vehiculos en el carrito
                    var inventario = await _context.Inventario.FirstOrDefaultAsync(i => i.idVehiculo == item.idVehiculo);
                    if (inventario != null)
                    {
                        //cuando se encuentra ese carro se le resta la cantidad en -1
                        inventario.cantidadVehiculos -= 1;

                        //buscamos los datos precisos del vehiculo
                        var vehiculo = await _context.Vehiculo.FindAsync(inventario.idVehiculo);

                        if (vehiculo != null) 
                        {

                            // Convertir ambos valores a decimal para compararlos directamente
                            decimal precioCarrito = Convert.ToDecimal(item.precioVehiculo);
                            decimal precioVehiculo = Convert.ToDecimal(vehiculo.precioVehiculo);

                            //hacemos un comparativa entre el precio del carrito y del original del vehiculo
                            //esto para poder ver si existe un descuento
                            if (precioCarrito < precioVehiculo)
                            {
                                montoDescuento += (double)precioVehiculo - (double)precioCarrito;
                            }

                            //si es igual o menor a 0
                            if (inventario.cantidadVehiculos <= 0)
                            {
                                // Evitar cantidades negativas, no deberia de entrar en cantidades negativas, pero mejor prevenir que lamentar
                                inventario.cantidadVehiculos = 0;

                                // Cambiar el estado del vehículo a false si la cantidad es igual a 0
                                vehiculo.estadoActivo = false;
                                _context.Update(vehiculo);

                            }//cierre if cantidad

                        }//cierre if null

                        _context.Update(inventario);

                    }
                }

                // Calcular los totales
                double subtotal = carrito.Sum(item => (double)item.precioVehiculo);
                double montoImpuesto = subtotal * 0.13; // Suponiendo un 13% de impuesto
                double totalFactura = subtotal + montoImpuesto;//aqui se supone que deberia de restarle el descuento, pero, ya el descuento esta hecho desde el precio del vehiculo
                //asi que soloamente se muestra el descuento en el correo


                // Creamos la factura
                var nuevaFactura = new Factura
                {
                    idUsuario = userId,
                    subtotal = subtotal,
                    montoDescuento = montoDescuento,
                    montoImpuesto = montoImpuesto,
                    totalFactura = totalFactura,
                    fechaFactura = DateTime.Now,
                    tipoPago = "Tarjeta"
                };

                //guardamos los datos
                _context.Factura.Add(nuevaFactura);
                await _context.SaveChangesAsync();


                EnviarCorreoFactura(nuevaFactura, carrito, userId, userName);

                carrito.Clear();


                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Se ha realizado una compra",
                    tablaModificada = "Factura y Vehiculos",
                    fechaModificacion = DateTime.Now,
                    idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                };

                TempData["Mensaje"] = "Tu Factura ha sido enviada al tu correo electronico";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Mensaje"] = "Por favor, inicie sesión antes de proceder con la compra.";
                return RedirectToAction("Login", "Usuario");
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

                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Se ha borrado una factura",
                    tablaModificada = "Facturas",
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




        //------------------------------------------correo-------------------------------------
        private void EnviarCorreoFactura(Factura factura, List<Carrito> carrito, int userId, string userName)
        {

            try
            {
                MailMessage email = new MailMessage();


                email.Subject = "Datos de registro en la plataforma web ICarPlus";

                email.To.Add(new MailAddress("ICarPlusAppWeb@outlook.com"));


                email.To.Add(new MailAddress(User.Identity.Name));

                email.From = new MailAddress("ICarPlusAppWeb@outlook.com");

                string html = "Hola, Gracias por realizar una compra con iCar Plus";

                html += $"<br><b>Estimado {userName}, aquí está su factura:";
                html += $"<br><b>Subtotal</b> $: {factura.subtotal}";
                html += $"<br><b>Descuento</b> $: {factura.montoDescuento}";
                html += $"<br><b>Impuesto</b> $: {factura.montoImpuesto}";
                html += $"<br><b>Total</b> $: {factura.totalFactura}";
                html += "<br><b>Detalles de la compra:</b>";

                foreach (var item in carrito)
                {
                    html += $"<br><b>Marca:</b>{item.marcaVehiculo} <b>Modelo: </b>{item.modeloVehiculo} <b>Precio:</b> ${item.precioVehiculo}";
                }

                email.IsBodyHtml = true;

                email.Priority = MailPriority.Normal;

                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);

                SmtpClient smtp = new SmtpClient();

                smtp.Host = "smtp-mail.outlook.com";

                smtp.Port = 587;

                smtp.EnableSsl = true;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("ICarPlusAppWeb@outlook.com", "Ucr+2023");

                smtp.Send(email);

                email.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







    }//cierre class
}//cierre namespace
