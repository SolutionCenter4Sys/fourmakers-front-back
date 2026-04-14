using DataTransferObject.Domain.Diretoria;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorDiretoriaService
    {
        Task<List<DiretoriaColaboradorDTO>> ListarDiretoriaDosColaboradoresAsync(int orgId, string cpfRequest);
    }
}