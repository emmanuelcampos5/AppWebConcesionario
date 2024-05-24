﻿using AppWebConcesionario.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                    return RedirectToAction("Restablecer", "Usuario", new { correoUsuario = temp.correoUsuario });
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
        public async Task<IActionResult> RegistrarUsuario([Bind] Usuario user, string listaLugares)
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

                return View();
            }

            if (user != null)
            {
                user.idRol = 2;
                user.lugarResidencia = listaLugares;
                user.password = this.GenerarClave();
                user.estadoSuscripcion = true;
                user.restablecer = true;
                user.estadoActivo = true;

                if (!UsuarioExistente(user))
                {
                    //_context.Usuario.Add(user);
                    try
                    {
                        //_context.SaveChanges();

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
        public IActionResult Restablecer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Restablecer(string s)
        {
            return View();
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
                EmailRegistro emailRegistro = new EmailRegistro();
                emailRegistro.Enviar(temp);
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }//cierre class
}//cierre namespace
