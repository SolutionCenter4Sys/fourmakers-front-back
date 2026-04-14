using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.Services.Pdi
{
    public interface IPdisDoTimeService
    {
        Task<ApiGenericResult<IEnumerable<PdiResumoTimeDTO>>> ListarPdisDoTimeAsync(string cpf, int orgId);
        Task<ApiGenericResult<PdiListagemTimeResult>> ListarPdisDoTimePaginadoAsync(string cpf, int orgId, int pagina, int tamanhoPagina);
        Task<ApiGenericResult<IEnumerable<PdiResumoDTO>>> ListarPdisPorColaboradorIdAsync(string colaboradorId, string cpfGestor, int orgId);
        Task<ApiGenericResult<PdiCompletoTimeDTO>> ObterPdiCompletoDoTimeAsync(Guid pdiId, string cpf, int orgId);

        /// <summary>Gestor: cria PDI para um colaborador do time (próprio ou subordinado).</summary>
        Task<ApiGenericResult<PdiCriarResponseDTO>> CriarPdiParaColaboradorAsync(string colaboradorId, string cpfGestor, int orgId, PdiCriarRequestDTO request);
        /// <summary>Gestor: atualiza PDI de um colaborador do time.</summary>
        Task<ApiGenericResult<PdiAtualizarResponseDTO>> AtualizarPdiParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, PdiAtualizarRequestDTO request);
        /// <summary>Gestor: adiciona action plan ao PDI do colaborador.</summary>
        Task<ApiGenericResult<PdiActionPlanDTO>> AdicionarActionPlanParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, PdiActionPlanInputDTO request);
        /// <summary>Gestor: atualiza action plan do PDI do colaborador.</summary>
        Task<ApiGenericResult<PdiActionPlanDTO>> AtualizarActionPlanParaColaboradorAsync(string codigoInternoColaborador, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId, PdiActionPlanInputDTO request);
        /// <summary>Gestor: remove action plan do PDI do colaborador.</summary>
        Task<ApiGenericResult> RemoverActionPlanParaColaboradorAsync(string colaboradorId, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId);
        /// <summary>Gestor: marca action plan como concluído.</summary>
        Task<ApiGenericResult<PdiConcluirActionPlanResponseDTO>> ConcluirActionPlanParaColaboradorAsync(string codigoInternoColaborador, Guid pdiId, Guid actionPlanId, string cpfGestor, int orgId);
        /// <summary>Gestor: envia evidência para o PDI do colaborador.</summary>
        /// <summary>Arquivo e link (coluna <c>link</c>) são opcionais, mas é obrigatório enviar ao menos um.</summary>
        Task<ApiGenericResult<PdiEvidenciaUploadResultDTO>> UploadEvidenciaParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link);
        /// <summary>Gestor: lista evidências do PDI do colaborador.</summary>
        Task<ApiGenericResult<IEnumerable<PdiEvidenciaDTO>>> ListarEvidenciasParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId);
        /// <summary>Gestor: obtém o arquivo da evidência para download (ler). Colaborador deve pertencer ao time.</summary>
        Task<ApiGenericResult<PdiEvidenciaDownloadDTO>> ObterEvidenciaParaColaboradorAsync(string colaboradorId, Guid pdiId, Guid evidenciaId, string cpfGestor, int orgId);
        /// <summary>Gestor: aprova o PDI (criado pelo gestor). Colaborador criou os planos de ação; ao aprovar, PDI vai para Andamento (IN_PROGRESS).</summary>
        Task<ApiGenericResult<PdiAprovarResponseDTO>> AprovarPdiParaColaboradorAsync(string colaboradorId, Guid pdiId, string cpfGestor, int orgId);
    }
}
