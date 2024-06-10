
//Fecha Creacion: 14-05-2024
//Funcion: Contener los modulos y metodos necesarios para funcionamiento de la clase
//Fecha Ultima Modificacion:29-05-2024
//Responsable: Christopher Rodríguez. //Razon: Realizar ajuste para el registro de usuario.

using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;



namespace AppWebConcesionario.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context; //AppDbContext: representa el contexto de la base de datos.


        private static string EmailRestablecer = ""; //EmailRestablecer: representa el correo al cual se quiere restablecer la contrasena 


        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }



        //Muestra lista de Usuarios
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Usuario.ToList());
        }

        //Muestra lista de opciones administrativas
        [HttpGet]
        public IActionResult VistaAdministrativa()
        {
            return View();
        }

        //Muestra formulario para autenticarse
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        //------------------------------vistas---------------------------------------------


        //Recolecta y envia los datos para realizar la autenticacion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario user)
        {
            var temp = this.ValidarUsuario(user);

            if (temp != null)
            {
                bool restablecer = false;

                //verifica si hay que restablecer la contraseña
                restablecer = VerificarRestablecer(temp);

                //si hay que restablecerla, se lleva a la View restablecer
                if (restablecer)
                {
                    return RedirectToAction("Restablecer", "Usuario", new { tempCorreo = temp.correoUsuario });
                }
                else
                {
                    //se guarda la informacion
                    var userClaims = new List<Claim>
                        {
                             new Claim(ClaimTypes.Name, temp.correoUsuario),
                             new Claim("idUsuario", temp.idUsuario.ToString()),
                             new Claim("NombreUsuario", temp.nombreUsuario),
                             new Claim("Rol", temp.idRol.ToString())
                        };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");

                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                    HttpContext.SignInAsync(userPrincipal);

                    if (temp.idRol == 1)
                    {
                        return RedirectToAction("VistaAdministrativa", "Usuario");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            TempData["Mensaje"] = "Usuario o Contraseña incorrecta";
            return View(user);
        }

        //Realizar el cierre de sesion
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        //Muestra detalltes de usuario
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.Usuario.FirstOrDefaultAsync(x => x.idUsuario == id);

            return View(temp);
        }

        //Muestra formulario para que un usuario se registre
        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }

        //Recolecta y envia los datos para registrar un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarUsuario([Bind] Usuario user)
        {
            //if (listaLugares == "Seleccione una Provincia")
            //{
            //    return View(user);
            //}          

            if (user != null)
            {
                //var lugarResidencia = user.lugarResidencia;
                //user.lugarResidencia = listaLugares;
                user.idRol = 2;
                user.password = this.GenerarClave();
                user.estadoSuscripcion = true;
                user.restablecer = true;
                user.estadoActivo = true;

                if (!UsuarioExistente(user))
                {
                    _context.Usuario.Add(user);
                    if (user.lugarResidencia == null)
                    {
                        TempData["MensajeError"] = "No se logro crear la cuenta..";
                    }

                    try
                    {
                        _context.SaveChanges();

                        if (this.EnviarEmailRegistro(user))
                        {
                            TempData["MensajeCreado"] = "Usuario creado correctamente, Su contraseña fue enviada por email";
                        }
                        else
                        {
                            TempData["MensajeCreado"] = "Usuario creado pero no se envio el email, comuniquese con el administrador;";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["MensajeError"] = "No se logro crear la cuenta.." + ex.Message;
                    }
                }
                else
                {
                    TempData["MensajeError"] = "El usuario ya se encuentra registrado en el sistema";
                }
            }
            return View(user);
        }


        //Muestra el formulario para poder restablecer la contrasena
        [HttpGet]
        public IActionResult Restablecer(string? tempCorreo)
        {
            var usuario = _context.Usuario.First(usuario => usuario.correoUsuario.Equals(tempCorreo));

            SeguridadRestablecer restablecer = new SeguridadRestablecer();

            restablecer.Email = usuario.correoUsuario;
            restablecer.Password = usuario.password;

            EmailRestablecer = usuario.correoUsuario;

            return View(restablecer);
        }

        //Recolecta y envia los datos necesarios para el restablecimiento de la contrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restablecer(SeguridadRestablecer pRestablecer)
        {
            if (pRestablecer != null)
            {
                var usuario = _context.Usuario.First(c => c.correoUsuario == EmailRestablecer); //Se crea la variable usuario para guardar datos

                if (usuario.password.Equals(pRestablecer.Password))
                {
                    if (pRestablecer.NewPassword.Equals(pRestablecer.Confirmar))
                    {
                        usuario.password = pRestablecer.Confirmar;
                        usuario.restablecer = false;

                        _context.Usuario.Update(usuario);
                        _context.SaveChanges();

                        return RedirectToAction("Login", "Usuario");
                    }
                    else
                    {
                        TempData["MensajeError"] = "Las contraseñas no coinciden";
                        return View(pRestablecer);
                    }
                }
                else
                {
                    TempData["MensajeError"] = "La contraseña es incorrecta";
                    return View(pRestablecer);
                }
            }
            else
            {
                TempData["MensajeError"] = "Datos incorrectos";
                return View(pRestablecer);
            }

        }

        //Muestra la vista para los usuarios cuando olvidan la contrasena
        [HttpGet]
        public IActionResult OlvidarContraseña()
        {
            return View();
        }


        //Recolecta y envia los datos necesarios para poder enviar informacion cuando olvidan la contrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OlvidarContraseña(string? tempCorreo)
        {
            var usuario = _context.Usuario.FirstOrDefault(c => c.correoUsuario == tempCorreo);

            if (usuario != null)
            {
                usuario.password = this.GenerarClave();
                usuario.restablecer = true;

                _context.Usuario.Update(usuario);

                try
                {
                    _context.SaveChanges();

                    if (this.EnviarEmailRestablecer(usuario))
                    {
                        TempData["MensajeCreado"] = "Se envió una contraseña temporal por email";
                    }
                    else
                    {
                        TempData["MensajeCreado"] = "No se pudo enviar una contraseña temporal, comuniquese con el administrador;";
                    }
                }
                catch (Exception ex)
                {
                    TempData["MensajeError"] = "No se logro restablecer la contraseña.." + ex.Message;
                }
                return View();
            }
            else
            {
                TempData["MensajeError"] = "El email no se encuentra asociado a una cuenta";
                return View();
            }
                     
        }

        //---------------------------MODULO CONSULTAS-------------------------------------------
        
        //Muestra el formulario para que el usuario pueda realizar una consulta
        [HttpGet]
        public IActionResult RealizarConsulta()
        {
            return View();
        }


        //Recolecta y envia los datos necesarios para poder enviar la consulta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RealizarConsulta(string mensaje)
        {
            
            if (User.Identity.IsAuthenticated)
            {

                // Traer los datos del usuario autenticado desde la base de datos
                int userId = int.Parse(User.FindFirstValue("idUsuario"));
                Usuario user = _context.Usuario.FirstOrDefault(u => u.idUsuario == userId);

                if(user != null)
                {
                    if (this.EnviarEmailConsulta(user, mensaje))
                    {
                       
                        TempData["MensajeCreado"] = "Consulta realizada correctamente, su respuesta llegará pronto al email.";

                    }
                    return View(user);
                }
                else
                {
                    TempData["MensajeError"] = "Hubo un error al enviar su consulta. Por favor, inténtelo de nuevo.";
                    return View(user);
                }
            }
            else
            {
                TempData["Mensaje"] = "Antes de Realizar la consulta, por favor inicia sesion";
                return RedirectToAction("Login", "Usuario");
            }
            
        }

        //Encargado de crear y enviar el email de consulta al correo de destino definido
        private bool EnviarEmailConsulta(Usuario temp, string mensaje)
        {
            try
            {
                bool enviado = false;
                Email emailConsulta = new Email(); //Objeto Email con metodo emailConsulta
                emailConsulta.EnviarConsulta(temp, mensaje);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //---------------------------METODOS----------------------------------------------------

        //Se encarga de verificar la existencia de un usuario
        public bool UsuarioExistente(Usuario temp)
        {
            bool existente = false;
            foreach (var usuario in _context.Usuario.ToList())
            {
                if (usuario.correoUsuario == temp.correoUsuario || usuario.cedulaUsuario == temp.cedulaUsuario)
                {
                    if (usuario.idRol == 2)
                    {
                        existente = true;
                    }
                    else
                    {
                        if (usuario.correoUsuario == temp.correoUsuario)
                        {
                            existente = true;
                        }
                    }
                }
            }
            return existente;
        }


        //---------------------------METODOS----------------------------------------------------

        //Se encarga de generar una clave aleatoria para nuevos usuarios
        public string GenerarClave()
        {
            Random rnd = new Random(); 

            string clave = string.Empty;

            clave = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(clave, 12).Select(
                s => s[rnd.Next(s.Length)]).ToArray());
        }

        //Se encarga de validar la autorizacion de un usuario
        private Usuario ValidarUsuario(Usuario temp)
        {
            Usuario autorizado = null;

            var user = _context.Usuario.FirstOrDefault(u => u.correoUsuario.Equals(temp.correoUsuario));
            
            if (user != null)
            {
                if (user.password.Equals(temp.password))
                {
                    autorizado = user;
                }
            }
            return autorizado;
        }

        //Se encarga de verificar que el restablecimineto sea el correcto
        private bool VerificarRestablecer(Usuario temp)
        {
            bool verificado = false;

            var user = _context.Usuario.FirstOrDefault(u => u.correoUsuario.Equals(temp.correoUsuario));

            if (user != null)
            {
                if (user.restablecer == true)
                {
                    verificado = true;
                }
                
            }

            return verificado;
        }

        //Encargado de crear y enviar el email de registro al correo de destino definido
        private bool EnviarEmailRegistro(Usuario temp)
        {
            try
            {
                bool enviado = false;
                Email emailRegistro = new Email(); //Objeto Email con metodo emailRegistro
                emailRegistro.EnviarRegistro(temp);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Encargado de crear y enviar el email de restablecer contra al correo de destino definido
        private bool EnviarEmailRestablecer(Usuario temp)
        {
            try
            {
                bool enviado = false;
                Email emailR = new Email(); //Objeto Email con metodo emailRestablecer
                emailR.EnviarRestablecer(temp);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        //Muestra el fomrulario para la edicion de un usuario
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var temp = await _context.Usuario.FirstOrDefaultAsync(x => x.idUsuario == id);

            return View(temp);
        }

        //Recolecta y envia los datos necesarios para la edicion de usuarios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Usuario usuario)
        {
            if (id == usuario.idUsuario)
            {
                var temp = await _context.Usuario.FirstOrDefaultAsync(r => r.idUsuario == id);

                _context.Usuario.Remove(temp);
                _context.Usuario.Add(usuario);


                var auditoria = new RegistroAuditoria
                {
                    idAuditoria = 0,
                    descripcion = "Edicion de usuario",
                    tablaModificada = "Usuario",
                    fechaModificacion = DateTime.Now,
                    idUsuarioModificacion = int.Parse(User.FindFirstValue("idUsuario"))
                };

                _context.RegistroAuditoria.Add(auditoria);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Usuario", new { id = usuario.idUsuario });
            }
            else
            {
                return NotFound();
            }
        }





    }
}
