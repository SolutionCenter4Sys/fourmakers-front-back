using System;
using System.Collections.Generic;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeRelacionamento.VCX;

namespace MapaDeRelacionamento.Domain.Interfaces;

public interface IVCXService
{
    Task<ApiGenericResult<IEnumerable<VCXDorDTO>>> GetDoresByPosicaoIdAsync(Guid posicaoId);
    Task<ApiGenericResult<IEnumerable<VCXIniciativaDTO>>> GetIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<ApiGenericResult<VCXPosicaoDoresIniciativasResultDTO>> GetDoresAndIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<ApiGenericResult<VCXHistoricoDoresIniciativasResultDTO>> GetHistoricoDoresIniciativasByPosicaoIdAsync(Guid posicaoId);

    Task<ApiGenericResult<VCXDorDTO>> GetDorByIdAsync(Guid id);
    Task<ApiGenericResult<VCXDorDTO>> CreateDorAsync(VCXDorInputDTO input, string codigoInternoColaboradorAlterador);
    Task<ApiGenericResult<VCXDorDTO>> UpdateDorAsync(VCXDorInputDTO input, string codigoInternoColaboradorAlterador);
    Task<ApiGenericResult> DeleteDorAsync(Guid id, string codigoInternoColaboradorAlterador);

    Task<ApiGenericResult<VCXIniciativaDTO>> GetIniciativaByIdAsync(Guid id, int orgId);
    Task<ApiGenericResult<VCXIniciativaDTO>> CreateIniciativaAsync(VCXIniciativaInputDTO input, string codigoInternoColaboradorAlterador, int orgId);
    Task<ApiGenericResult<VCXIniciativaDTO>> UpdateIniciativaAsync(VCXIniciativaInputDTO input, string codigoInternoColaboradorAlterador, int orgId);
    Task<ApiGenericResult> DeleteIniciativaAsync(Guid id, string codigoInternoColaboradorAlterador, int orgId);

    Task<ApiGenericResult<IEnumerable<VCXNotaBastidoresDTO>>> GetNotasBastidoresByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<ApiGenericResult<VCXNotaBastidoresDTO>> GetNotaBastidoresByIdAsync(Guid id, int orgId);
    Task<ApiGenericResult<VCXNotaBastidoresDTO>> CreateNotaBastidoresAsync(VCXNotaBastidoresCriarInputDTO input, string codigoInternoColaboradorAlterador, int orgId);
    Task<ApiGenericResult<VCXNotaBastidoresDTO>> UpdateNotaBastidoresAsync(VCXNotaBastidoresAtualizarInputDTO input, string codigoInternoColaboradorAlterador, int orgId);
    Task<ApiGenericResult> DeleteNotaBastidoresAsync(Guid id, string codigoInternoColaboradorAlterador, int orgId);
    Task<ApiGenericResult<IEnumerable<VCXNotaBastidoresLogDTO>>> GetHistoricoNotasBastidoresByPosicaoIdAsync(Guid posicaoId);

    Task<ApiGenericResult<IEnumerable<VCXTemaDTO>>> GetTemasByOrgIdAsync(int orgId, string? descricao = null);
    Task<ApiGenericResult<VCXTemaDTO>> CreateTemaAsync(VCXTemaInputDTO input, int orgId);

    Task<ApiGenericResult<IEnumerable<VCXImpactoDTO>>> GetImpactosAsync();
    Task<ApiGenericResult<IEnumerable<VCXUrgenciaDTO>>> GetUrgenciasAsync();
    Task<ApiGenericResult<IEnumerable<VCXStatusDTO>>> GetStatusAsync();

    Task<ApiGenericResult<VCXAgendasPorColaboradorClienteResultDTO>> GetAgendasPorColaboradorClienteAsync(string codigoColaborador, string codigoCliente, int cursor, int limit);

    Task<ApiGenericResult<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>>> ListarOrcamentoHistoricoPorPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?>> BuscarUltimoOrcamentoPorPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>> CriarOrcamentoHistoricoPorPosicaoAsync(Guid posicaoId, OrganogramaPosicaoOrcamentoHistoricoInserirParamDTO param, string codigoInternoColaboradorAlterador, int orgId);
}
