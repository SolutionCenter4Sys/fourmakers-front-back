using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.Services.Pdi
{
    /// <summary>
    /// Serviço de métricas PDI: big numbers, listagem filtrada (restrição DIRETORIA, datas, gestor, colaborador, status), exportação CSV.
    /// </summary>
    public interface IPdiMetricasService
    {
        /// <summary>Métricas no escopo do usuário (org + tb_restricao_acesso_colaborador tipo DIRETORIA quando houver).</summary>
        Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasPorOrgAsync(string cpfUsuario, int orgId, PdiMetricasFiltroRequestDTO filtro = null);

        Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiGestorItemDTO>>> ListarGestoresOrgAsync(
            string cpfUsuario, int orgId, int? pagina, int? tamanhoPagina);

        Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiUnidadeItemDTO>>> ListarUnidadesMetricasAsync(
            string cpfUsuario, int orgId, int? pagina, int? tamanhoPagina);

        Task<ApiGenericResult<PdiMetricasListagemPaginadaDTO<PdiColaboradorMetricaItemDTO>>> ListarColaboradoresMetricasAsync(
            string cpfUsuario, int orgId, string? cpfGestor, string? codDiretoria, int? pagina, int? tamanhoPagina);

        Task<ApiGenericResult<PdiMetricasCsvExportResultDTO>> ExportarMetricasCsvAsync(
            string cpfUsuario, int orgId, PdiMetricasExportRequestDTO request);

        Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasColaboradorAsync(string cpf, int orgId);

        Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasGestorAsync(string cpfGestor, int orgId);

        Task<ApiGenericResult<PdiMetricasResultDTO>> ObterMetricasGestorPorColaboradorAsync(string colaboradorId, string cpfGestor, int orgId);
    }
}
