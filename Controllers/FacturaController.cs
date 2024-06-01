﻿using AppWebConcesionario.Models;
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
                var userId = int.Parse(User.FindFirstValue("idUsuario"));
                var userName = User.FindFirstValue("NombreUsuario");

                // Obtener el carrito de compras del usuario (aquí debes implementar cómo obtienes el carrito del usuario)
                var carrito = CarritoController.GetCarrito();

                if (carrito == null || !carrito.Any())
                {
                    TempData["Mensaje"] = "Tu carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                // Calcular los totales
                double subtotal = carrito.Sum(item => (double)item.precioVehiculo);
                double montoDescuento = 0; // Calcular descuento si aplica
                double montoImpuesto = subtotal * 0.13; // Suponiendo un 13% de impuesto
                double totalFactura = subtotal + montoImpuesto - montoDescuento;

                // Crear la factura
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

                _context.Factura.Add(nuevaFactura);
                await _context.SaveChangesAsync();


                foreach (var item in carrito)
                {
                    var inventario = await _context.Inventario.FirstOrDefaultAsync(i => i.idVehiculo == item.idVehiculo);
                    if (inventario != null)
                    {
                        inventario.cantidadVehiculos -= 1;

                        if (inventario.cantidadVehiculos <= 0)
                        {
                            inventario.cantidadVehiculos = 0; // Evitar cantidades negativas
                            
                            // Cambiar el estado del vehículo a false
                            var vehiculo = await _context.Vehiculo.FindAsync(inventario.idVehiculo);
                            if (vehiculo != null)
                            {
                                vehiculo.estadoActivo = false;
                                _context.Update(vehiculo);
                            }
                        }

                        _context.Update(inventario);
                    }
                }

                await _context.SaveChangesAsync();


                EnviarCorreoFactura(nuevaFactura, carrito, userId, userName);

                carrito.Clear();


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
                html += $"<br><b>Subtotal</b>: {factura.subtotal}";
                html += $"<br><b>Descuento</b>: {factura.montoDescuento}";
                html += $"<br><b>Impuesto</b>: {factura.montoImpuesto}";
                html += $"<br><b>Total</b>: {factura.totalFactura}";
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
