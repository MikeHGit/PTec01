using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services.Contracts
{
    public interface IPersonaService
    {       
        Task<Persona> GetPersona(int idPersona);
        Task<Persona> SavePersona(Persona model);
        Task<Persona> UpdatePersona(Persona model);
        Task<Persona> DeletePersona(Persona model);
    }
}
