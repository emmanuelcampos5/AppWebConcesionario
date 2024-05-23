using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
                    return RedirectToAction("Restablecer", "Usuario", new { correoUsuario = temp.correoUsuario });
                }
                else
                {
                    //se guarda la informacion
                    var userClaims = new List<Claim>() { new Claim(ClaimTypes.Name, temp.correoUsuario) };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");

                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                    HttpContext.SignInAsync(userPrincipal);

                    return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> RegistrarUsuario([Bind]Usuario user, string listaLugares)
        {
            if (listaLugares == "Seleccione una Provincia")
            {
                return View(user);
            }
            if (user != null)
            {
                user.idRol = 2;
                user.lugarResidencia = listaLugares;
                user.password = this.GenerarClave();
                user.estadoSuscripcion = true;
                user.restablecer = true;
                user.estadoActivo = true;

                _context.Usuario.Add(user);

                await _context.SaveChangesAsync();

                Email email = new Email();

                email.EmailContra(user);

                
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Restablecer(string? correoUsuario)
        {
            var temp = await _context.Usuario.FirstOrDefaultAsync(u => u.correoUsuario == correoUsuario);

            SeguridadRestablecer restablecer = new SeguridadRestablecer();

            restablecer.Email = temp.correoUsuario;
            restablecer.Password = temp.password;

            EmailRestablecer = temp.correoUsuario;

            return View(restablecer);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restablecer([Bind] SeguridadRestablecer pRestablecer)
        {
            if (pRestablecer != null)
            {
                var temp = await _context.Usuario.FirstOrDefaultAsync(u => u.correoUsuario.Equals(EmailRestablecer));

                if (temp != null)
                {
                    if (temp.password.Equals(pRestablecer.Password))
                    {
                        if (pRestablecer.NewPassword.Equals(pRestablecer.Confirmar))
                        {
                            temp.password = pRestablecer.Confirmar;

                            temp.restablecer = false;

                            _context.Usuario.Update(temp);

                            await _context.SaveChangesAsync();

                            return RedirectToAction("Login", "Usuario");
                        }
                        else
                        {
                            TempData["Mensaje"] = "La confirmacion de la contra no es correcta";

                            return View(pRestablecer);
                        }
                    }
                    else
                    {
                        TempData["Mensaje"] = "El password es incorrecto";

                        return View(pRestablecer);
                    }
                }
                else
                {
                    TempData["Mensaje"] = "No existe el usuario a restablecer la contra";

                    return View(pRestablecer);
                }
            }
            else
            {
                TempData["Mensaje"] = "Datos incorrectos...";

                return View(pRestablecer);
            }
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



    }//cierre class
}//cierre namespace
