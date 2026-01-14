using MAVPC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAVPC.Services
{
    public interface ITrafficService
    {
        Task<List<Camara>> GetCamarasAsync();
        Task<List<Incidencia>> GetIncidenciasAsync();
        Task<bool> AddCamaraAsync(Camara nuevaCamara);
    }
}