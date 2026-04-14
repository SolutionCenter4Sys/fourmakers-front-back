using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;

namespace Core.Domain.SRS;

public interface IAdmissaoOrigemRepository
{
    /// <summary>Cria ou substitui o vínculo de origem de uma admissão. Registra log automaticamente.</summary>
    Task<Guid> InserirAsync(Guid admissaoId, AdmissaoOrigemInput input, string alteradorCpf);

    /// <summary>Retorna o vínculo de origem de uma admissão, ou <c>null</c> se não houver.</summary>
    Task<AdmissaoOrigemResult> ObterPorAdmissaoAsync(Guid admissaoId);
}
