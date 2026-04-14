using DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo;
using System;
using System.Threading.Tasks;

namespace Core.Domain.Apontamento
{
    public interface IPeriodoFechadoRepository
    {
        FecharAlterarPeriodoDTO CriarDataPeriodoFechado(DateTime dataFim, int orgId, string cpf);

        FecharAlterarPeriodoDTO AlterarDataPeriodoFechado(DateTime dataFim, int orgId, string cpf, int? id);

        int ExistePeriodo(int orgId);

        Task<BuscaPeriodoFechadoResult> BuscaPeriodoFechado(int orgId);
    }
}