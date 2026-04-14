using DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apontamento.Domain.Interfaces.Service
{
    public interface IPeriodoFechadoService
    {
        FecharAlterarPeriodoDTO FecharAlterarPeriodo(DateTime dataFim, int orgId, string cpf);

        Task<BuscaPeriodoFechadoResult> BuscaPeriodoFechado(int orgId);

        Task ValidaSeEstaNoPeriodoFechado(string dataValidacao, int orgId);

        Task ValidaSeEstaNoPeriodoFechado(List<string> datas, int orgId);
    }
}