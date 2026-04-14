using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Vaga;
using System.Threading.Tasks;

namespace Labs.Domain.Interfaces
{
    /// <summary>
    /// Serviço de domínio que expõe a extração de perfil a partir de um prompt (Match/Extractor).
    /// Quando <paramref name="logContext"/> é informado, grava request/response em tb_labs_log_extractor_extract_vaga.
    /// </summary>
    public interface IExtractorService
    {
        Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request, ExtrairPerfilLogContext? logContext = null);
    }
}
