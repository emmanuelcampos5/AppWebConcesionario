﻿using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace AppWebConcesionario.Models
{
    public class Email
    {

        public void EnviarConsulta(Usuario user, string mensaje)
        {

            try
            {
                MailMessage email = new MailMessage();

                //Asunto del email
                email.Subject = "Datos de consulta realizada en la plataforma web ICarPlus";

                //Destinatarios
                //Direccion del correo del administrador
                email.To.Add(new MailAddress("ICarPlusAppWebConsultas@outlook.com"));

                //Direccion del correo del usuario
                //email.To.Add(new MailAddress(user.correoUsuario));

                //emisor del correo 
                email.From = new MailAddress("ICarPlusAppWebConsultas@outlook.com");   //se debe crear un nuevo correo para este depto (realiza y responde las consultas)

                //se construye la vista HTML del cuerpo del correo electronico
                string html = "Zona de consultas para ICarPlus";
                html += "<br> A continuacion detallamos los datos de la consulta realizada por el usuario" + user.nombreUsuario;
                html += "<br><b>Consulta: </b> " + mensaje;
                html += "<br><b>Responder este correo a esta direccion: </b>" + user.correoUsuario;
               
                html += "<br>Plataforma web ICarPlus.</b>";

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
                smtp.Credentials = new NetworkCredential("ICarPlusAppWebConsultas@outlook.com", "Ucr+2023");

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

    }
}
