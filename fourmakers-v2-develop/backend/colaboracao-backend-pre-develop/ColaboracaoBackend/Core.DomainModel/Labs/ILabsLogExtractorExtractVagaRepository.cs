using System;
using System.Threading.Tasks;

namespace Core.Domain.Labs
{
    /// <summary>
    /// Repositório para log de chamadas ao ExtrairPerfilDeUmPrompt (Extractor / tb_labs_log_extractor_extract_vaga).
    /// </summary>
    public interface ILabsLogExtractorExtractVagaRepository
    {
        /// <summary>
        /// Insere o log e retorna o id (GUID) do registro (tb_labs_log_extractor_extract_vaga.id).
        /// </summary>
        Task<Guid> InserirAsync(int tbOrgId, string? idVaga, string? codigoInternoColaborador, string? objetoRequest, string? objetoResponse);

        /// <summary>
        /// Verifica se existe registro com o id informado.
        /// </summary>
        Task<bool> ExisteAsync(Guid id);
    }
}
