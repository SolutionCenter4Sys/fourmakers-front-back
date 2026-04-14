using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.ParametroOrg
{
    public interface IParametroConfiguracaoRepository
    {
        string GetParametroConfiguracao(string parametro, string codigoInternoColaborador, int orgId);
        Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoDoUsuarioLogado(string cpfRequest, int orgId, TipoParametroEnum tipoParametro);

        Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoPorOrg(int orgId, TipoParametroEnum tipoParametro);
        Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoPorCodigoParametro(string codigoParametro, int orgId, TipoParametroEnum tipoParametro);
        Task<ParametroConfiguracaoResult> ObterParametroConfiguracaoPorId(string id, TipoParametroEnum tipoParametro, int orgId);
        Task<ParametroConfiguracaoResult> InserirParametroConfiguracao(ParametroConfiguracaoRepositoryInput input, TipoParametroEnum tipoParametro);
        Task<ParametroConfiguracaoResult> AtualizarParametroConfiguracao(ParametroConfiguracaoRepositoryInput input, TipoParametroEnum tipoParametro);
        Task<bool> DeletarParametroConfiguracao(string id, int orgId);
    }
}