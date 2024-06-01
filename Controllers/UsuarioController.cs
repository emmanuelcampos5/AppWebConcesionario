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
        private readonly AppDbContext _context;


        private static string EmailRestablecer = "";


        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Usuario.ToList());
        }

        [HttpGet]
        public IActionResult VistaAdministrativa()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        //------------------------------vistas---------------------------------------------

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }

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

                user.idRol = 2;
                //user.lugarResidencia = listaLugares;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restablecer(SeguridadRestablecer pRestablecer)
        {
            if (pRestablecer != null)
            {
                var usuario = _context.Usuario.First(c => c.correoUsuario == EmailRestablecer);

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

        [HttpGet]
        public IActionResult OlvidarContraseña()
        {
            return View();
        }

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
        [HttpGet]
        public IActionResult RealizarConsulta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RealizarConsulta([Bind] Usuario user, string mensaje)
        {
            if (this.EnviarEmailConsulta(user, mensaje))
            {
                TempData["MensajeCreado"] = "Consulta realizada correctamente, su respuesta llegará pronto al email.";
            }
            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var temp = await _context.Usuario.FirstOrDefaultAsync(x => x.idUsuario == id);

            return View(temp);
        }



        //---------------------------METODOS----------------------------------------------------

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
        public string GenerarClave()
        {
            Random rnd = new Random();

            string clave = string.Empty;

            clave = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(clave, 12).Select(
                s => s[rnd.Next(s.Length)]).ToArray());
        }
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
        private bool EnviarEmailRegistro(Usuario temp)
        {
            try
            {
                bool enviado = false;
                Email emailRegistro = new Email();
                emailRegistro.EnviarRegistro(temp);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool EnviarEmailRestablecer(Usuario temp)
        {
            try
            {
                bool enviado = false;
                Email emailR = new Email();
                emailR.EnviarRestablecer(temp);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        
        
        
        
        
        //------------funcion para enviar consulta x email

        private bool EnviarEmailConsulta(Usuario temp, string mensaje)
        {
            try
            {
                bool enviado = false;
                Email emailConsulta = new Email();
                emailConsulta.EnviarConsulta(temp, mensaje);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }





    }
}
