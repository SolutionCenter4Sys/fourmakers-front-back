using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoInput;
using DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoResult;
using System.Threading.Tasks;

namespace Apontamento.Domain.Interfaces
{
    public interface IApontamentoValidadorService
    {
        Task<ValidaApontamentoResult> ValidaApontamento(ValidaApontamentoInput input, CRUDEnum crudEnum, int orgId);
        Task<ValidaApontamentoResult> ValidaApontamentoEmLote(string projetoId, string atividadeId, long horas, string dataInicio, string dataFim, string cpfRequest, string cpfColaborador, int orgId);
        Task ValidaSeEhMeuLancamento(int orgId, string apontamentoId, string cpfRequest);
    }
}