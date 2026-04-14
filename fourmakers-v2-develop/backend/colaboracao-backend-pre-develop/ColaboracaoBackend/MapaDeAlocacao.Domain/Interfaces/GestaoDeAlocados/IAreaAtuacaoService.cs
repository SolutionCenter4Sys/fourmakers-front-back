using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IAreaAtuacaoService
    {
        Task<ApiGenericResult<IEnumerable<AreaAtuacaoBase>>> ListarAreasAtuacao(string cpfRequest, int orgId);
        Task<List<AreaAtuacaoResult>> InserirAreasAtuacaoCasoNaoExista(List<GestorExternoAreaAtuacaoInput> listaAreasAtuacaoBase, int orgId);
    }
}