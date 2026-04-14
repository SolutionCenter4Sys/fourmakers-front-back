using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IAuditoriaAtendimentoRepository
    {
        Task InserirAsync(AuditoriaInsertInput input, int orgId, IDbTransaction transaction = null);
        Task<(IEnumerable<AuditoriaItemResult> itens, bool temMais)> ListarAsync(
            int orgId, int limite, int offset,
            DateTime? dataInicio = null, DateTime? dataFim = null,
            string categoria = null, string busca = null);
        Task<IEnumerable<AuditoriaItemResult>> ListarParaCsvAsync(int orgId,
            DateTime? dataInicio = null, DateTime? dataFim = null,
            string categoria = null, string busca = null,
            int maxLinhas = 2500);
    }
}
