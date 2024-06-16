using Microsoft.AspNetCore.Mvc;

// Referencias para trabajar con los servicios
using PruebaTecnica.Models;
using PruebaTecnica.Resources;
using PruebaTecnica.Services.Contracts;

// Referencias para trabajar con la autenticación por Cookies
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario model)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Encriptación de pass a formato SHA256.
                //model.Pass = Utilidades.EncriptarPassword(model.Pass);

                // Encriptación.
                model.Pass = Utilidades.Hash(model.Pass);

                // Creación de Usuario.
                Usuario usuarioCreado = await _usuarioService.SaveUsuario(model);

                // Validación si usuario fue creado correctamente.
                if (usuarioCreado.Identificador > 0)
                {
                    // Redirección, acción: InicioSesion, Controlador: Inicio.
                    return RedirectToAction("InicioSesion", "Inicio");
                }

                // Si el usuario no se crea, mostramos un mensaje de error.
                ViewData["Mensaje"] = "No se pudo crear el usuario.";
            }
            catch (ApplicationException ex)
            {
                // Excepciones específicas
                ViewData["Mensaje"] = ex.Message;
            }
            catch (Exception ex)
            {
                // Excepciones generales.
                ViewData["Mensaje"] = "Ocurrió un error al intentar crear el usuario. Por favor, contacte al soporte técnico.";
            }

            // Retornamos la vista con el mensaje en ViewData.
            return View();
        }

        public IActionResult InicioSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InicioSesion(string usuario, string password)
        {
            // Obtener Usuario por nombre de usuario.
            Usuario usuarioEncontrado = await _usuarioService.GetUsuario(usuario, password);

            // Usuario no encontrado.
            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrecto.";
                return View();
            }

            // Configuración de autenticación.

            // 1. Lista de Claims para el Usuario autenticado.
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Usuario1)
            };

            // 2. Creación de la identidad de claims.
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Configuración de las propiedades de autenticación.
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                // 3.1 Permitir que la sesión sea renovada automáticamente.
                AllowRefresh = true
            };

            // 4. Autenticar al Usuario.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
                );

            // 5. Redirección, acción: Index, Controlador: Home.
            return RedirectToAction("Index", "Home");
        }
    }
}
