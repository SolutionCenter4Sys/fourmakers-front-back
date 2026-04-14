using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.Externo
{
    public interface IMapaDeAlocacaoExternoService
    {
        Task<ApiGenericResult<AlocacaoMensalResultDTO>> ObterAlocacoesHorasMensais(
            string tokenSistema,
            int mes,
            int ano,
            string codigoColaborador = null,
            string codigoProjeto = null);
    }
}
