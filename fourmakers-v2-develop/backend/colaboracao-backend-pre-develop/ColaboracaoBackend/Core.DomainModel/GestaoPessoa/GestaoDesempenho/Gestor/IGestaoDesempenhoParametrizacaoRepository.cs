using System.Threading.Tasks;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;

namespace Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public interface IGestaoDesempenhoParametrizacaoRepository
    {
        Task<ParametrizacaoOrgDTO> ObterParametrizacaoPorOrgAsync(int orgId);
        Task<bool> UpsertParametrizacaoAsync(ParametrizacaoOrgDTO parametrizacao);
    }
}
