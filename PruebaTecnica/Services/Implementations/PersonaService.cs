using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PruebaTecnica.Controllers;
using PruebaTecnica.Models;
using PruebaTecnica.Services.Contracts;
using PruebaTecnica.Services.Implementations;

namespace PruebaTecnica.Services.Implementations
{
    public class PersonaService : IPersonaService
    {
        private readonly Models.DBPruebaTecnicaContext _db;

        public PersonaService(DBPruebaTecnicaContext db)
        {
            _db = db;
        }

        // Obtener Persona.
        public async Task<Persona> GetPersona(int idPersona)
        {
            try
            {
                // Utiliza FindAsync para obtener la persona de forma asincrona.
                Persona persona = await _db.Personas.FindAsync(idPersona);

                // Si no se encuentra la persona, lanzar una excepción.
                if (persona == null)
                {
                    throw new KeyNotFoundException("Persona no encontrada.");
                }

                return persona;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales.
                throw new ApplicationException("Ocurrió un error al intentar encontrar la persona. Por favor, contacte al soporte técnico.", ex);
            }
        }

        // Guardar Persona.
        public async Task<Persona> SavePersona(Persona model)
        {
            try
            {
                _db.Personas.Add(model);
                await _db.SaveChangesAsync();
                return model;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && sqlException.Number == 2627)
                {
                    throw new ApplicationException("Ya existe el número de documento.", ex);
                }
                else
                {
                    throw new ApplicationException("No se pudo guardar el registro. Error al actualizar la base de datos.", ex);
                }
            }
        }

        // Actualización de Persona.
        public async Task<Persona> UpdatePersona(Persona model)
        {
            try
            {
                // Verificar que persona existe.
                var personaOriginal = await _db.Personas.FindAsync(model.Identificador);
                if (personaOriginal == null)
                {
                    throw new KeyNotFoundException("Persona no encontrada.");
                }

                // Verifica si el numero de identificacion de la persona ya existe en otro registro.
                var numDocExistente = await _db.Personas
                    .FirstOrDefaultAsync(p => p.NumeroIdentificacion == model.NumeroIdentificacion && p.Identificador != model.Identificador);

                if (numDocExistente != null)
                {
                    throw new ApplicationException("El número de documento ya está registrado.");
                }

                // Actualiza solo los campos necesarios.
                personaOriginal.Nombres = model.Nombres;
                personaOriginal.Apellidos = model.Apellidos;
                personaOriginal.NumeroIdentificacion = model.NumeroIdentificacion;
                personaOriginal.Email = model.Email;
                personaOriginal.TipoIdentificacion = model.TipoIdentificacion;
                personaOriginal.FechaModificacion = DateTime.Now;

                await _db.SaveChangesAsync();
                return personaOriginal;

            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("No se pudo guardar el registro. Error al actualizar la base de datos.", ex);
            }
        }

        // Eliminar Persona.
        public async Task<Persona> DeletePersona(Persona model)
        {
            try
            {
                _db.Personas.Remove(model);
                await _db.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al intentar eliminar la persona. Por favor, contacte al soporte técnico.", ex);
            }
        }
    }
}
