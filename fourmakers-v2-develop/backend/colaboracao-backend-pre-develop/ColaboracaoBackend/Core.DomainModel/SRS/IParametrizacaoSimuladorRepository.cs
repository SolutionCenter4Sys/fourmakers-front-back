using System.Threading.Tasks;
using DataTransferObject.Domain.SRS;

namespace Core.Domain.SRS;

public interface IParametrizacaoSimuladorRepository
{
    Task<ParametrizacaoSimuladorResult?> ObterPorOrgAsync(int tbOrgId);
}
