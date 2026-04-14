using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.GestaoPessoa.Pdi
{
    public interface IPdiRepository
    {
        Task<IEnumerable<PdiResumoDTO>> ListarMeusPdisAsync(string codigoInternoColaborador, int orgId);
        Task<IEnumerable<PdiResumoDTO>> ListarPdisPorColaboradorIdAsync(string colaboradorId, int orgId);
        Task<PdiResumoDTO> ObterPdiCompletoPorIdAsync(Guid pdiId, string codigoInternoColaborador, int orgId);
        /// <param name="codigoInternoCriacao">Quem criou o PDI (colaborador ou gestor). Se null, usa codigoInternoColaborador.</param>
        Task<Guid> InserirPdiAsync(string codigoInternoColaborador, int orgId, PdiCriarRequestDTO request, string codigoInternoCriacao = null, IDbTransaction transaction = null);
        Task<bool> AtualizarPdiAsync(Guid pdiId, string codigoInternoColaborador, PdiAtualizarRequestDTO request, IDbTransaction transaction = null);
        Task<Guid> InserirActionPlanAsync(Guid pdiId, string codigoInternoColaborador, PdiActionPlanInputDTO request, IDbTransaction transaction = null);
        Task<bool> AtualizarActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, PdiActionPlanInputDTO request, IDbTransaction transaction = null);
        Task<bool> ExcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, IDbTransaction transaction = null);
        /// <summary>Retorna (ColaboradorId, CriadoPor, Status) do PDI para regras de fluxo (Em Análise vs aprovação gestor).</summary>
        Task<(string ColaboradorId, string CriadoPor, string Status)> ObterCriadorPdiAsync(Guid pdiId);
        /// <summary>Atualiza apenas o status do PDI (ex.: IN_ANALYSIS após add plano pelo próprio; IN_PROGRESS após gestor aprovar).</summary>
        Task<bool> AtualizarStatusPdiAsync(Guid pdiId, string status, string codigoInternoAlteracao, IDbTransaction transaction = null);
        Task<bool> ConcluirActionPlanAsync(Guid pdiId, Guid actionPlanId, string codigoInternoColaborador, IDbTransaction transaction = null);
        /// <param name="link">Coluna <c>link</c> em <c>tb_pdi_ativos</c>; opcional se houver arquivo.</param>
        /// <param name="docBytes">Opcional se houver <paramref name="link"/>.</param>
        Task<Guid> InserirEvidenciaAsync(Guid pdiId, string docName, string docPath, string docMime, long? docSize, string tipo, byte[] docBytes, string link, IDbTransaction transaction = null);
        Task<IEnumerable<PdiEvidenciaDTO>> ListarEvidenciasAsync(Guid pdiId, string codigoInternoColaborador);
        /// <summary>Retorna o binário e metadados da evidência apenas se pertencer ao PDI e ao colaborador. Item <c>Link</c> vem da coluna <c>link</c>.</summary>
        Task<(byte[] Doc, string DocName, string DocMime, string Link)> ObterEvidenciaBytesAsync(Guid pdiId, Guid evidenciaId, string codigoInternoColaborador);
        Task<IEnumerable<PdiResumoTimeDTO>> ListarPdisDoTimeAsync(IEnumerable<string> codigosInternosSubordinados, int orgId);
        Task<(IEnumerable<PdiResumoTimeDTO> Items, int TotalCount)> ListarPdisDoTimeAsync(IEnumerable<string> codigosInternosSubordinados, int orgId, int pagina, int tamanhoPagina);

        /// <summary>
        /// Lista PDIs da organização para visão RH. <paramref name="codigosInternosColaborador"/> null = todos os PDIs com <c>tb_org_id</c> = orgId; lista vazia não deve ser passada (use escopo vazio no serviço).
        /// </summary>
        Task<(IEnumerable<PdiResumoRhDTO> Items, int TotalCount)> ListarPdisRhPorOrgAsync(int orgId, IReadOnlyList<string> codigosInternosColaborador, int pagina, int tamanhoPagina);

        /// <summary>Contagens por status no mesmo escopo da listagem RH (org + opcional IN colaboradores).</summary>
        Task<PdiMetricasBigNumbersDTO> ObterContagensPdisRhAsync(int orgId, IReadOnlyList<string> codigosInternosColaborador);
        Task<PdiCompletoTimeDTO> ObterPdiCompletoDoTimeAsync(Guid pdiId, int orgId);
        /// <summary>Retorna o dead_line do PDI (null se não existir ou for nulo).</summary>
        Task<DateTime?> ObterDeadlinePdiAsync(Guid pdiId);
        Task<bool> PertenceAoColaboradorAsync(Guid pdiId, string codigoInternoColaborador);
        Task<int> ContarActionPlansAsync(Guid pdiId);
        Task<int> ContarActionPlansConcluidosAsync(Guid pdiId);

        /// <summary>Retorna contagens por status (big numbers) para os colaboradores informados.</summary>
        Task<PdiMetricasBigNumbersDTO> ObterContagensPorStatusAsync(IEnumerable<string> codigosColaborador, int orgId);

        /// <summary>Lista PDIs com progresso e previsão, filtrados por status (ex.: ativos = IN_ANALYSIS, IN_PROGRESS; históricos = COMPLETED, CANCELLED).</summary>
        Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisComPrevisaoPorStatusesAsync(IEnumerable<string> codigosColaborador, int orgId, IEnumerable<string> statuses);

        /// <summary>Retorna contagens por status para todos os PDIs da organização (tb_org_id).</summary>
        Task<PdiMetricasBigNumbersDTO> ObterContagensPorStatusPorOrgAsync(int orgId);

        /// <summary>Lista PDIs da organização com progresso e previsão, filtrados por status.</summary>
        Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisComPrevisaoPorStatusesPorOrgAsync(int orgId, IEnumerable<string> statuses);

        /// <summary>Lista gestores da organização para filtro (vw_gestores_colaboradores_org). Retorna codigo_interno e nome.</summary>
        Task<IEnumerable<PdiGestorItemDTO>> ListarGestoresOrgAsync(int orgId);

        /// <summary>Gestores cujo time tem colaboradores nas diretorias informadas (null = todas da org).</summary>
        Task<IEnumerable<PdiGestorItemDTO>> ListarGestoresPorDiretoriasAsync(int orgId, IReadOnlyList<string> codDiretorias);

        /// <summary>Contagens por status com escopo (diretoria, colaboradores, datas). Não filtra por status.</summary>
        Task<PdiMetricasBigNumbersDTO> ObterContagensMetricasFiltradasAsync(PdiMetricasQueryDTO query);

        /// <summary>Lista PDIs com escopo e filtros; statuses vazio = todos.</summary>
        Task<IEnumerable<PdiMetricaItemDTO>> ListarPdisMetricasFiltradasAsync(PdiMetricasQueryDTO query);

        /// <summary>Total de PDIs no mesmo escopo/filtros da listagem (para paginação).</summary>
        Task<int> ContarPdisMetricasFiltradasAsync(PdiMetricasQueryDTO query);

        /// <summary>Gestores paginados (mesmo critério de <see cref="ListarGestoresPorDiretoriasAsync"/>).</summary>
        Task<(IEnumerable<PdiGestorItemDTO> Itens, int TotalItens)> ListarGestoresPorDiretoriasPaginadoAsync(
            int orgId, IReadOnlyList<string> codDiretorias, int skip, int take);

        /// <summary>Linhas para exportação; só retorna PDIs que pertencem ao escopo.</summary>
        Task<IEnumerable<PdiMetricaExportLinhaDTO>> ListarPdisMetricasExportPorIdsAsync(IEnumerable<Guid> pdiIds, PdiMetricasQueryDTO escopo);
    }
}
