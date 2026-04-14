using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IAssistenteConfigRepository
    {
        Task<AssistenteConfigResult> ObterAsync(int orgId);
        Task<bool> SalvarAsync(AssistenteConfigUpdateInput input, int orgId, IDbTransaction transaction = null);
    }
}
