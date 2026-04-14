using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.Services.Pdi
{
    public interface IMeusPdisService
    {
        Task<ApiGenericResult<IEnumerable<PdiResumoDTO>>> ListarMeusPdisAsync(string cpf, int orgId);
        Task<ApiGenericResult<PdiCriarResponseDTO>> CriarPdiAsync(string cpf, int orgId, PdiCriarRequestDTO request);
        Task<ApiGenericResult<PdiAtualizarResponseDTO>> AtualizarPdiAsync(Guid id, string cpf, int orgId, PdiAtualizarRequestDTO request);
        Task<ApiGenericResult<PdiActionPlanDTO>> AdicionarActionPlanAsync(Guid pdiId, string cpf, int orgId, PdiActionPlanInputDTO request);
        Task<ApiGenericResult<PdiConcluirActionPlanResponseDTO>> ConcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string cpf, int orgId);
        /// <summary>Arquivo e link (coluna <c>link</c>) são opcionais, mas é obrigatório enviar ao menos um.</summary>
        Task<ApiGenericResult<PdiEvidenciaUploadResultDTO>> UploadEvidenciaAsync(Guid pdiId, string cpf, int orgId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link);
        Task<ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>> ListarEvidenciasAsync(Guid pdiId, string cpf, int orgId);
        /// <summary>Obtém o arquivo da evidência para download (ler). Evidência deve pertencer ao PDI do colaborador.</summary>
        Task<ApiGenericResult<PdiEvidenciaDownloadDTO>> ObterEvidenciaAsync(Guid pdiId, Guid evidenciaId, string cpf, int orgId);
    }
}
