using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services.Contracts
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string usuario, string pass);
        Task<Usuario> FindUsuario(int idUsuario);
        Task<Usuario> SaveUsuario(Usuario model);
        Task<Usuario> UpdateUsuario(Usuario model);
        Task<Usuario> DeleteUsuario(Usuario model);
        
    }
}
