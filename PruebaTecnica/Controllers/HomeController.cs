using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using PruebaTecnica.Models;
using PruebaTecnica.Resources;
using PruebaTecnica.Services.Contracts;
using PruebaTecnica.Services.Implementations;
using System.Diagnostics;
using System.Security.Claims;

namespace PruebaTecnica.Controllers
{
    // Restringir el acceso a métodos o controladores específicos solo a usuarios autenticados.
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DBPruebaTecnicaContext _db;
        private readonly IPersonaService _personaService;

        public HomeController(DBPruebaTecnicaContext db, IPersonaService personaService)
        {
            _db = db;
            _personaService = personaService;            
        }

        public IActionResult Index()
        {
            // Mostrar Usuario en Home.
            string nombreUsuario = "";
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                nombreUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value)
                    .SingleOrDefault();
            }
            ViewData["userName"] = nombreUsuario;


            // Mostrar tabla de Personas.
            List<Persona> lista = _db.Personas.ToList();
            return View(lista);
        }

        // Datos que se muestran al momento de actualizar.
        [HttpGet]
        public async Task<IActionResult> Guardar(int Identificador)
        {
            Persona model = new Persona();

            if (Identificador != 0)
            {
                try
                {
                    // Se buscan los datos del empleado. 
                    model = await _personaService.GetPersona(Identificador);
                }
                catch (ApplicationException ex)
                {
                    ViewData["Mensaje"] = ex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["Mensaje"] = "Ocurrió un error inesperado. Por favor, contacte al soporte técnico.";
                    return View("Error");
                }
            }

            return View(model);
        }

        // Registro o Actualización.
        [HttpPost]
        public async Task<IActionResult> Guardar( Persona model )
        {
            try
            {
                Persona personaResult;

                if (model.Identificador == 0)
                {
                    // Creación.
                    personaResult = await _personaService.SavePersona(model);
                }
                else
                {
                    // Actualización.
                    personaResult = await _personaService.UpdatePersona(model);
                }

                // Verifica si la operación fue exitosa.
                if (personaResult != null && personaResult.Identificador > 0)
                {
                    // Redirección, acción: Index, Controlador: Home.
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si persona no se guarda, mostramos un mensaje de error.
                    ViewData["Mensaje"] = "No se pudo guardar el registro.";
                }

            }
            catch (ApplicationException ex)
            {
                // Excepciones específicas
                ViewData["Mensaje"] = ex.Message;
            }
            catch (Exception ex)
            {
                // Excepciones generales.
                ViewData["Mensaje"] = "Ocurrió un error al intentar crear el registro. Por favor, contacte al soporte técnico.";
            }

            // Retornamos la vista con el mensaje en ViewData.
            return View(model);
        }

        // Vista de Eliminar.
        [HttpGet]
        public async Task<IActionResult> Eliminar(int Identificador)
        {
            if (Identificador == 0)
            {
                // Si el identificador es 0, probablemente sea un error, redirigir a página de error.
                ViewData["Mensaje"] = "Identificador de persona inválido.";
            }

            try
            {
                // Obtener los datos de la persona por su identificador
                Persona model = await _personaService.GetPersona(Identificador);

                if (model == null)
                {
                    // Si no se encontró la persona con el identificador dado, manejar el error
                    ViewData["Mensaje"] = "Persona no encontrada.";
                }

                // Mostrar la vista con los datos de la persona para confirmar la eliminación
                return View(model);
            }
            catch (ApplicationException ex)
            {
                // Manejar excepciones específicas de la aplicación
                ViewData["Mensaje"] = ex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                // Manejar excepciones generales
                ViewData["Mensaje"] = "Ocurrió un error inesperado. Por favor, contacte al soporte técnico.";
                return View("Error");
            }
        }

        // Eliminar Persona.
        [HttpPost]
        public async Task<IActionResult> Eliminar(Persona model)
        {
            try
            {
                await _personaService.DeletePersona(model);

                // Redirección después de eliminar correctamente.
                return RedirectToAction("Index", "Home");
            }
            catch (ApplicationException ex)
            {
                // Excepciones específicas.
                ViewData["Mensaje"] = ex.Message;
            }
            catch (Exception ex)
            {
                // Excepciones generales.
                ViewData["Mensaje"] = "Ocurrió un error al intentar eliminar la persona. Por favor, contacte al soporte técnico.";
            }

            // Retorno de vista con el mensaje en ViewData.
            return View(model);
        }

        // Cerrar Sesión
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioSesion", "Inicio");
        }
    }
}