using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Core.Domain.SRS;

public interface IAdmissaoPipelineStatusRepository
{
    Task<IEnumerable<AdmissaoPipelineStatusResult>> ListarPorPipelineAsync(int orgId, Guid pipelineId, bool somenteAtivos = true);

    /// <summary>Remove vínculos atuais e insere <paramref name="itens"/> (transação atômica).</summary>
    Task SubstituirPorPipelineAsync(int orgId, Guid pipelineId, IReadOnlyList<AdmissaoPipelineStatusItemInput> itens);
}
