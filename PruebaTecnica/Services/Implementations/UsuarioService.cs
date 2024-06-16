using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PruebaTecnica.Controllers;
using PruebaTecnica.Models;
using PruebaTecnica.Resources;
using PruebaTecnica.Services.Contracts;
using PruebaTecnica.Services.Implementations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PruebaTecnica.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly Models.DBPruebaTecnicaContext _db;

        public UsuarioService(DBPruebaTecnicaContext db)
        {
            _db = db;
        }

        // Obtener Usuario por usuario y contraseña.
        public async Task<Usuario> GetUsuario(string usuario, string pass)
        {
            // Busca al usuario en la base de datos por usuario.
            Usuario foundUser = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario1 == usuario);

            // Verificar si se encontró un usuario y devolverlo.
            if (foundUser == null)
            {
                return null;
            }
            // Verificar la contraseña.
            bool isPasswordValid = Utilidades.Verify(foundUser.Pass, pass);

            // Si la contraseña no es válida, devolver null.
            if (!isPasswordValid)
            {
                return null;
            }

            // Devolver el usuario encontrado
            return foundUser;
        }

        // Obtener Usuario por id usuario.
        public async Task<Usuario> FindUsuario(int idUsuario)
        {
            try
            {
                // Utiliza FindAsync para obtener la persona de forma asincrona.
                Usuario usuario = await _db.Usuarios.FindAsync(idUsuario);

                // Si no se encuentra la persona, lanzar una excepción.
                if (usuario == null)
                {
                    throw new KeyNotFoundException("Persona no encontrada.");
                }

                return usuario;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales.
                throw new ApplicationException("Ocurrió un error al intentar encontrar la persona. Por favor, contacte al soporte técnico.", ex);
            }
        }

        // Guardar Usuario.
        public async Task<Usuario> SaveUsuario(Usuario model)
        {
            try
            {
                _db.Usuarios.Add(model);
                await _db.SaveChangesAsync();
                return model;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && sqlException.Number == 2627)
                {
                    throw new ApplicationException("El usuario ya se encuentra registrado.", ex);
                }
                else
                {
                    throw new ApplicationException("No se pudo guardar el usuario. Error al actualizar la base de datos.", ex);
                }
            }
        }

        // Actualización de Usuario
        public async Task<Usuario> UpdateUsuario(Usuario model)
        {
            try
            {
                // Verificar que el Usuario existe.
                var usuarioOriginal = await _db.Usuarios.FindAsync(model.Identificador);
                if (usuarioOriginal == null)
                {
                    throw new KeyNotFoundException("Usuario no encontrado.");
                }

                // Verifica si el nombre de usuario ya existe en otro registro.
                var usuarioExistente = await _db.Usuarios
                    .FirstOrDefaultAsync(u => u.Usuario1 == model.Usuario1 && u.Identificador != model.Identificador);

                if (usuarioExistente != null)
                {
                    throw new ApplicationException("El nombre de usuario ya está en uso.");

                }

                // Verificar la contraseña.
                bool isPasswordValid = Utilidades.Verify(usuarioOriginal.Pass, model.Pass);

                // Si la contraseña no es válida.
                if (isPasswordValid)
                {
                    throw new ApplicationException("La nueva contraseña no puede ser igual a la contraseña actual.");
                }
                else
                {
                    usuarioOriginal.Pass = Utilidades.Hash(model.Pass);
                }

                // Actualiza solo los campos necesarios.
                usuarioOriginal.Usuario1 = model.Usuario1;
                usuarioOriginal.FechaModificacion = DateTime.Now;

                await _db.SaveChangesAsync();
                return usuarioOriginal;

            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("No se pudo guardar el registro. Error al actualizar la base de datos.", ex);
            }
        }

        // Eliminar Usuario
        public async Task<Usuario> DeleteUsuario(Usuario model)
        {
            try
            {
                _db.Usuarios.Remove(model);
                await _db.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al intentar eliminar el usuario. Por favor, contacte al soporte técnico.", ex);
            }
        }
    }
}