using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IKbFonteMetaRepository
    {
        Task UpsertAsync(string fonteId, string areaId, int orgId, IDbTransaction transaction = null);
        Task DeletarAsync(string fonteId, int orgId, IDbTransaction transaction = null);
        Task<int> ContarPorAreaAsync(string areaId, int orgId);
        Task<int> ContarFontesAsync(int orgId);
        Task<IEnumerable<KbFonteResult>> ListarAsync(int orgId);
        Task<KbFonteResult> ObterPorFonteIdAsync(string fonteId, int orgId);
    }
}
