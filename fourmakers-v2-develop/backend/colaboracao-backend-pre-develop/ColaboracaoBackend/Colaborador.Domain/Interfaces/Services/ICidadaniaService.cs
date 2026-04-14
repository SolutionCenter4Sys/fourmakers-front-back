using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.Cidadania;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface ICidadaniaService
    {
        Task<ApiGenericResult<List<CidadaniaDTO>>> ListarCidadanias();

        Task<ApiGenericResult<List<CidadaniaStatusDTO>>> ListarStatusCidadania();

        Task<ApiGenericResult<CidadaniaColaboradorDTO>> BuscarCidadaniaColaboradorPorId(int id);

        Task<ApiGenericResult<CidadaniaColaboradorDTO>> InserirCidadaniaColaborador(int cidadaniaId, int cidadaniaStatusId, string codigoInternoColaborador);

        Task<ApiGenericResult<CidadaniaColaboradorDTO>> AtualizarCidadaniaColaborador(int statusId, int id, string codigoInternoColaborador);

        Task<ApiGenericResult<bool>> RemoverCidadaniaColaborador(int id, string codigoInternoColaborador);
    }
}