using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IAreaAtuacaoRepository
    {
        Task<IEnumerable<AreaAtuacaoBase>> ListarAreasAtuacaoAsync(int orgId);
        Task<AreaAtuacaoResult> InserirAreaAtuacaoAsync(AreaAtuacaoInput input);
        Task<AreaAtuacaoResult> GetAreaAtuacaoPorDescricaoAsync(string descricao, int orgId);
    }
}