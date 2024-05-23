using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace AppWebConcesionario.Models
{
    public class Email
    {

        public void EmailContra(Usuario usuario)
        {
            try
            {
                MailMessage email = new MailMessage();

                email.Subject = "Datos de registro en plataforma web !CarPlus";

                email.To.Add(new MailAddress(usuario.correoUsuario));

                email.Bcc.Add(new MailAddress("cr808pruebas@outlook.com"));

                email.From = new MailAddress("cr808pruebas@outlook.com");

                string html = "Bienvenidos a nuestra plataforma virtual de !CarPlus";

                html += "<br> A continuacion detallamos los datos registrados en nuestra pagina web";

                html += "<br><b>Email:</b>" + usuario.correoUsuario;

                html += "<br><b>Nombre:</b>" + usuario.nombreUsuario;

                html += "<br><b>Su contraseña temporal es: </b>" + usuario.password;

                html += "<br><b>No responda a este correo porque fue generado de manera automatica";

                email.IsBodyHtml = true;

                email.Priority = MailPriority.Normal;

                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8,
                    MediaTypeNames.Text.Html);

                email.AlternateViews.Add(view);

                SmtpClient smtp = new SmtpClient();

                smtp.Host = "smtp-mail.outlook.com";

                smtp.Port = 587;

                smtp.EnableSsl = true;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("cr808pruebas@outlook.com", "Ucr2024!");

                //se envia el email
                smtp.Send(email);

                //se liberan los recursos
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
