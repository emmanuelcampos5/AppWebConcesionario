using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Drawing;
using System.Security.Claims;

namespace AppWebConcesionario.Models
{
    public class Email
    {

        public void EnviarConsulta(Usuario user, string mensaje)
        {
            try
            {
                MailMessage email = new MailMessage();

                // Asunto del email
                email.Subject = "Consulta realizada en la plataforma web ICarPlus";

                // Destinatario: Dirección del correo del administrador
                email.To.Add(new MailAddress("ICarPlusAppWebConsultas@outlook.com"));

                // Emisor del correo: Dirección del correo del usuario
                email.From = new MailAddress("ICarPlusAppWebConsultas@outlook.com");

                // Construir la vista HTML del cuerpo del correo electrónico
                string html = "Zona de consultas para ICarPlus";
                html += "<br>A continuación, detallamos los datos de la consulta realizada por el usuario " + user.nombreUsuario;
                html += "<br><b>Consulta: </b>" + mensaje;
                html += "<br><b>Responder este correo a esta dirección: </b>" + user.correoUsuario;
                html += "<br>Plataforma web ICarPlus.</b>";

                // Indicar que el contenido es en HTML
                email.IsBodyHtml = true;

                // Indicar la prioridad
                email.Priority = MailPriority.Normal;

                // Instanciar la vista del HTML para el cuerpo del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                // Agregar la vista HTML al cuerpo del correo
                email.AlternateViews.Add(view);

                // Configurar el protocolo de comunicación SMTP
                SmtpClient smtp = new SmtpClient();

                // Servidor de correo a implementar
                smtp.Host = "smtp-mail.outlook.com";

                // Configurar el puerto de comunicación
                smtp.Port = 587;

                // Indicar si el buzón utiliza seguridad tipo SSL
                smtp.EnableSsl = true;

                // Indicar las credenciales de autenticación
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("ICarPlusAppWebConsultas@outlook.com", "Ucr+2023");

                // Método para enviar el correo
                smtp.Send(email);

                // Liberar las instancias de los objetos
                email.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnviarRegistro(Usuario user)
        {
            try
            {
                MailMessage email = new MailMessage();

                //Asunto del email
                email.Subject = "Datos de registro en la plataforma web ICarPlus";

                //Destinatarios
                //Direccion del correo del administrador
                email.To.Add(new MailAddress("ICarPlusAppWeb@outlook.com"));

                //Direccion del correo del usuario
                email.To.Add(new MailAddress(user.correoUsuario));

                //emisor del correo
                email.From = new MailAddress("ICarPlusAppWeb@outlook.com");

                //se construye la vista HTML del cuerpo del correo electronico
                string html = "Bienvenido a ICarPlus, gracias por formar parte de nuestra plataforma";
                html += "<br> A continuacion detallamos los datos de inicio de sesion para nuestra plataforma web";
                html += "<br><b>Email:</b>" + user.correoUsuario;
                html += "<br><b>Su contraseña temporal:</b>" + user.password;
                html += "<br><b>No responda este correo porque fue generado de forma automatica. ";
                html += "Por la plataforma web ICarPlus.</b>";

                //se indica que el contenido es en html
                email.IsBodyHtml = true;

                //se indica la prioridad
                email.Priority = MailPriority.Normal;

                //se instancia la vista del html para el cuerpo del body del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                //se agrega la vista html al cuerpo del correo
                email.AlternateViews.Add(view);

                //se configura el protocolo de comunicacion smtp
                SmtpClient smtp = new SmtpClient();

                //servidor de correo a implementar
                smtp.Host = "smtp-mail.outlook.com";

                //se configura el puerto de comunicacion
                smtp.Port = 587;

                //se indica si el buzon utiliza seguridad tipo SSL
                smtp.EnableSsl = true;

                //se indica las credenciales de autenticacion
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("ICarPlusAppWeb@outlook.com", "Ucr+2023");

                //metodo para enviar el correo
                smtp.Send(email);

                //se liberan las instancias de los objetos
                email.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnviarRestablecer(Usuario user)
        {
            try
            {
                MailMessage email = new MailMessage();

                //Asunto del email
                email.Subject = "Restablecer contraseña";

                //Destinatarios
                //Direccion del correo del administrador
                email.To.Add(new MailAddress("ICarPlusAppWeb@outlook.com"));

                //Direccion del correo del usuario
                email.To.Add(new MailAddress(user.correoUsuario));

                //emisor del correo
                email.From = new MailAddress("ICarPlusAppWeb@outlook.com");

                //se construye la vista HTML del cuerpo del correo electronico
                string html = "Contraseña temporal para la plataforma web ICarPlus";
                html += "<br> A continuacion detallamos los datos de inicio de sesion para nuestra plataforma web";
                html += "<br><b>Email:</b>" + user.correoUsuario;
                html += "<br><b>Su contraseña temporal:</b>" + user.password;
                html += "<br><b>No responda este correo porque fue generado de forma automatica. ";
                html += "Por la plataforma web ICarPlus.</b>";

                //se indica que el contenido es en html
                email.IsBodyHtml = true;

                //se indica la prioridad
                email.Priority = MailPriority.Normal;

                //se instancia la vista del html para el cuerpo del body del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                //se agrega la vista html al cuerpo del correo
                email.AlternateViews.Add(view);

                //se configura el protocolo de comunicacion smtp
                SmtpClient smtp = new SmtpClient();

                //servidor de correo a implementar
                smtp.Host = "smtp-mail.outlook.com";

                //se configura el puerto de comunicacion
                smtp.Port = 587;

                //se indica si el buzon utiliza seguridad tipo SSL
                smtp.EnableSsl = true;

                //se indica las credenciales de autenticacion
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("ICarPlusAppWeb@outlook.com", "Ucr+2023");

                //metodo para enviar el correo
                smtp.Send(email);

                //se liberan las instancias de los objetos
                email.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnviarPromocion(IEnumerable<Usuario> user, Vehiculo vehiculo, Promocion promocion)
        {
            try
            {

                foreach (var usuario in user)
                {
                    if (usuario.lugarResidencia == promocion.lugarPromocion) // Verifica si la dirección del usuario coincide con la promoción
                    {

                        MailMessage email = new MailMessage();

                        email.Subject = "Plataforma web ICarPlus";

                        email.To.Add(new MailAddress("ICarPlusAppWeb@outlook.com"));

                        email.To.Add(new MailAddress(usuario.correoUsuario));

                        email.From = new MailAddress("ICarPlusAppWeb@outlook.com");

                        string html = "Hola, desde la administracion de iCarPlus le desamos un feliz dia";
                        html += "<br>Acabamos de activar una promocion en nuestra pagina";
                        html += "<br>Para esta ocasion tenemos nuestro " + vehiculo.marcaVehiculo + vehiculo.modeloVehiculo + " en oferta para nuestros usuarios de " + promocion.lugarPromocion;
                        html += "<br><b>Actualmente se encuentra en:</b> $" + promocion.precioPromocion;
                        html += "<br><b>Precio anterior:</b>$ <s>" + vehiculo.precioVehiculo + "</s>";
                        html += "<br><b>Recuerda que esta oferta es por tiempo limitado, corre antes de que se acabe.</b>";
                        html += "<br><b>No responda este correo porque fue generado de forma automatica. ";
                        html += "Por la plataforma web ICarPlus.</b>";

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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


       






    }//cierre class
}//cierre namespace
