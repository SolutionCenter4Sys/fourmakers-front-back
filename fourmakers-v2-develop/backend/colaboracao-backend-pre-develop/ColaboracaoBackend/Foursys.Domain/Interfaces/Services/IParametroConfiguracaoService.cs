using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IParametroConfiguracaoService
    {
        Task<ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>> ListarParametroConfiguracaoDoUsuarioLogado(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>> ListarParametroConfiguracao(string cpfRequest, int orgIdLogada, int orgIdInformada);
        Task<ApiGenericResult<ParametroConfiguracaoResult>> ObterParametroConfiguracaoPorId(string id, string cpfRequest, int orgIdLogada, int orgIdInformada);
        Task<ApiGenericResult<ParametroConfiguracaoResult>> InserirParametroConfiguracao(ParametroConfiguracaoInput parametroConfiguracaoInput, string cpfRequest, int orgIdLogada, int orgIdInformada);
        Task<ApiGenericResult<ParametroConfiguracaoResult>> AtualizarParametroConfiguracao(ParametroConfiguracaoInput parametroConfiguracaoInput, Guid id, string cpfRequest, int orgIdLogada, int orgIdInformada);
        Task<ApiGenericResult> DeletarParametroConfiguracao(string id, string cpfRequest, int orgIdLogada, int orgIdInformada);
    }
}