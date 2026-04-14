using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers.Parametro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IParametroService
    {
        Task<ApiGenericResult<IEnumerable<ParametroResult>>> ListarParametros(string cpfRequest, int orgId);
        Task<ApiGenericResult<ParametroResult>> ObterParametroPorId(string id, string CpfRequest, int orgId);
        Task<ApiGenericResult<ParametroResult>> InserirParametro(ParametroInput parametroInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<ParametroResult>> AtualizarParametro(ParametroInput parametroInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarParametro(string id, string cpfRequest, int orgId);
    }
}