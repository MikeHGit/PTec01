using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Models;
using PruebaTecnica.Resources;
using PruebaTecnica.Services.Contracts;
using PruebaTecnica.Services.Implementations;

namespace PruebaTecnica.Controllers
{
    // Restringir el acceso a métodos o controladores específicos solo a usuarios autenticados.
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly DBPruebaTecnicaContext _db;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(DBPruebaTecnicaContext db, IUsuarioService usuarioService)
        {
            _db = db;
            _usuarioService = usuarioService;
        }
        public IActionResult Index()
        {
            // Mostrar tabla de Usuarios
            List<Usuario> lista = _db.Usuarios.ToList();
            return View(lista);
        }

        // Datos que se muestran al momento de actualizar.
        [HttpGet]
        public async Task<IActionResult> Actualizar(int Identificador)
        {
            Usuario model = new Usuario();

            if (Identificador != 0)
            {
                try
                {
                    // Se buscan los datos del empleado. 
                    model = await _usuarioService.FindUsuario(Identificador);
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

        // Actualizar Usuario.
        [HttpPost]
        public async Task<IActionResult> Actualizar(Usuario model)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                // Llamada al servicio para actualizar el usuario.
                var usuarioResult = await _usuarioService.UpdateUsuario(model);

                // Verifica si la operación fue exitosa.
                if (usuarioResult != null && usuarioResult.Identificador > 0)
                {
                    // Redirección exitosa a la página de inicio.
                    return RedirectToAction("Index", "Usuario");
                }
                else
                {
                    // Si la operación no se realizó correctamente, muestra un mensaje de error.
                    ViewData["Mensaje"] = "No se pudo actualizar el registro.";
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
                // Obtener los datos del usuario
                Usuario model = await _usuarioService.FindUsuario(Identificador);

                if (model == null)
                {
                    // Si no se encontró la persona con el identificador dado, manejar el error
                    ViewData["Mensaje"] = "Usuario no encontrada.";
                }

                // Mostrar la vista con los datos de la persona para confirmar la eliminación
                return View(model);
            }
            catch (ApplicationException ex)
            {
                // Manejar excepciones específicas de la aplicación.
                ViewData["Mensaje"] = ex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                // Manejar excepciones generales.
                ViewData["Mensaje"] = "Ocurrió un error inesperado. Por favor, contacte al soporte técnico.";
                return View("Error");
            }
        }

        // Eliminar Usuario.
        [HttpPost]
        public async Task<IActionResult> Eliminar(Usuario model)
        {
            try
            {
                await _usuarioService.DeleteUsuario(model);

                // Redirección después de eliminar correctamente
                return RedirectToAction("Index", "Usuario");
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
    }
}
