using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppWebConcesionario.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario user)
        {
            var temp = this.ValidarUsuario(user);

            if (temp != null)
            {
                bool restablecer = false;

                //verifica si hay que restablecer la contraseña
                restablecer = this.VerificarRestablecer(temp);

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

        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarUsuario([Bind]Usuario user, string listaLugares)
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
            }

            return RedirectToAction("Index", "Home");
        }


        //---------------------------METODOS----------------------------------------------------
        private string GenerarClave()
        {
            Random random = new Random();
            string clave = string.Empty;
            clave = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(clave, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Usuario ValidarUsuario(Usuario temp)
        {
            Usuario autorizado = null;
            var user = _context.Usuario.FirstOrDefault(u => u.correoUsuario == temp.correoUsuario);

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

            var user = _context.Usuario.First(u => u.correoUsuario == temp.correoUsuario);

            if (user != null)
            {
                if (user.restablecer)
                {
                    verificado = true;
                }
            }
            return verificado;
        }

    }
}
