using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Apontamento.Domain.Interfaces.Service
{
    public interface IFolhaPontoService
    {
        Task<bool> ProcessarFolhaPontoAsync(string loteId, string codigoColaborador, int orgId);
        Task<SumarioFolhaPontoResult> ObterSumarioFolhaPontoAsync(byte[] arquivoPdf, string codigoColaborador, int orgId);
        Task<List<LoteFilaResult>> BuscarLotesPorOrgAsync(int orgId);
        Task<bool> ProcessarItemFolhaPontoAsync(FilaMessageDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId);
        Task<DetalhesLoteFolhaPontoResult> DetalharLoteAsync(string loteId);
        Task DeletarLoteAsync(string loteId);

        /// <summary>
        /// Reenfileira os itens de lote informados para reprocessamento.
        /// Para cada item encontrado, redefine seu status e envia nova mensagem à fila SQS.
        /// Itens não encontrados são reportados no resultado.
        /// </summary>
        /// <param name="itensLoteId">Lista de IDs de itens de lote a reprocessar</param>
        /// <param name="codigoColaborador">CPF do colaborador solicitante</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Resumo do reprocessamento iniciado</returns>
        Task<ReprocessarItensFolhaPontoResult> ReprocessarItensFolhaPontoAsync(List<string> itensLoteId, string codigoColaborador, int orgId);
    }
} 