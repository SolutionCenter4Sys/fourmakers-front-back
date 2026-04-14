using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Projeto.GestorExterno;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IGestorExternoService
    {
        Task<ApiGenericResult<IEnumerable<GestorExternoResult>>> ListarGestoresExternos(string cpfRequest, string codigoCliente, int orgId, string busca);
        Task<ApiGenericResult<GestorExternoResult>> ObterGestorExternoPorCodigo(string codGestorExterno, string CpfRequest, int orgId);
        Task<ApiGenericResult<GestorExternoResult>> InserirGestorExterno(GestorExternoInput gestorExternoInput, string cpfRequest, string tokenUsuario, int orgId);
        Task<ApiGenericResult<GestorExternoResult>> AtualizarGestorExterno(GestorExternoInput gestorExternoInput, string codGestorExterno, string cpfRequest, string tokenUsuario, int orgId);
        Task<ApiGenericResult> DeletarGestorExterno(string id, string cpfRequest, int orgId);
    }
}