using DataTransferObject.Domain.MapaDeRelacionamento.VCX;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.VCX;

public interface IVCXRepository
{
    Task<IEnumerable<VCXDorDTO>> GetDoresByPosicaoIdAsync(Guid posicaoId);
    Task<IEnumerable<VCXIniciativaDTO>> GetIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<VCXPosicaoDoresIniciativasResultDTO> GetDoresAndIniciativasByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<VCXHistoricoDoresIniciativasResultDTO> GetHistoricoDoresIniciativasByPosicaoIdAsync(Guid posicaoId);

    Task<VCXDorDTO?> GetDorByIdAsync(Guid id);
    Task<Guid> InsertDorAsync(VCXDorInputDTO input);
    Task UpdateDorAsync(Guid id, VCXDorInputDTO input);
    Task DeleteDorAsync(Guid id);

    Task<VCXIniciativaDTO?> GetIniciativaByIdAsync(Guid id, int orgId);
    Task<Guid> InsertIniciativaAsync(VCXIniciativaInputDTO input);
    Task UpdateIniciativaAsync(Guid id, VCXIniciativaInputDTO input);
    Task DeleteIniciativaAsync(Guid id);

    Task InserirDoresLogAsync(VCXLogDTO log);
    Task InserirIniciativasLogAsync(VCXLogDTO log);

    Task<IEnumerable<VCXNotaBastidoresDTO>> GetNotasBastidoresByPosicaoIdAsync(Guid posicaoId, int orgId);
    Task<VCXNotaBastidoresDTO?> GetNotaBastidoresByIdAsync(Guid id, int orgId);
    Task<Guid> InsertNotaBastidoresAsync(VCXNotaBastidoresCriarInputDTO input);
    Task UpdateNotaBastidoresAsync(Guid id, VCXNotaBastidoresAtualizarInputDTO input);
    Task DeleteNotaBastidoresAsync(Guid id);
    Task InserirNotasBastidoresLogAsync(VCXLogDTO log);
    Task<IEnumerable<VCXNotaBastidoresLogDTO>> GetHistoricoNotasBastidoresByPosicaoIdAsync(Guid posicaoId);

    Task<IEnumerable<VCXTemaDTO>> GetTemasByOrgIdAsync(int orgId, string? descricao = null);
    Task<Guid> InsertTemaAsync(string descricao, int orgId);

    Task<IEnumerable<VCXImpactoDTO>> GetImpactosAsync();
    Task<IEnumerable<VCXUrgenciaDTO>> GetUrgenciasAsync();
    Task<IEnumerable<VCXStatusDTO>> GetStatusAsync();

    Task<VCXAgendasPorColaboradorClienteResultDTO> GetAgendasPorColaboradorClienteAsync(string codigoColaborador, string codigoCliente, int cursor, int limit);

    Task<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>> PosicaoOrcamentoHistoricoListarPorPosicao(string posicaoId);
    Task<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?> PosicaoOrcamentoHistoricoObterUltimoPorPosicao(string posicaoId);
    Task InserirHistoricoOrcamentoPosicao(string id, string posicaoId, int orgId, decimal orcamento, DateTime? dataInicio, DateTime? dataFim, string codigoInternoColaboradorAlterador);
}
