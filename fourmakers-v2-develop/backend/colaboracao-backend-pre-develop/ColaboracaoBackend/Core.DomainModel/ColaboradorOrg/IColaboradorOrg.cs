using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface IBuscaColaboradorOrgRepository
    {
        Task<ColaboradorEColaboradorOrgDTO> GetColaboradorEOrgsAsync(string documentoColaborador, string codigoInternoColaborador, int? orgId);
    }
}