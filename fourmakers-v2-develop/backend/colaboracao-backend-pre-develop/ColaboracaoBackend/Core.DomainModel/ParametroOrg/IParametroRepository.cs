using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.Parametro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.ParametroOrg
{
    public interface IParametroRepository
    {
        Task<IEnumerable<ParametroResult>> ListarParametros(TipoParametroEnum tipoParametroEnum);
        Task<ParametroResult> ObterParametroPorId(string id, TipoParametroEnum tipoParametroEnum);
        Task<ParametroResult> ObterParametroPorCodigo(string codigoParametro, TipoParametroEnum tipoParametroEnum);
        Task<ParametroResult> InserirParametro(ParametroRepositoryInput parametroRepositoryInput, TipoParametroEnum tipoParametroEnum);
        Task<ParametroResult> AtualizarParametro(ParametroRepositoryInput parametroRepositoryInput, TipoParametroEnum tipoParametroEnum);
        Task<bool> DeletarParametro(string id);
    }
}