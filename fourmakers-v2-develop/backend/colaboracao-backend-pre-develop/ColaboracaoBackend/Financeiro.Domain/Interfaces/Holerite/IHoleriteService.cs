using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Holerite;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoteFilaResult = DataTransferObject.Domain.Financeiro.Holerite.LoteFilaResult;

namespace Financeiro.Domain.Interfaces.Holerite
{
    public interface IHoleriteService
    {
        /// <summary>
        /// Processa um lote de holerite
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="codigoColaborador">Código do colaborador</param>
        /// <param name="orgId">ID da organização</param>
        /// <param name="tipoProcessamento">Tipo de processamento do holerite (Mensal ou Adiantamento)</param>
        /// <returns>True se processado com sucesso</returns>
        Task<bool> ProcessarHoleriteAsync(string loteId, string codigoColaborador, int orgId, TipoProcessamentoHoleriteEnum tipoProcessamento);

        /// <summary>
        /// Processa (ou reprocessa) um item específico de holerite de forma idempotente.
        /// Se o holerite já existir para o itemLoteId, atualiza o conteúdo; caso contrário, executa
        /// as ações colaterais pertinentes ao tipo e insere o registro.
        /// </summary>
        /// <param name="mensagem">Mensagem da fila</param>
        /// <param name="codigoColaboradorRequest">Código do colaborador solicitante</param>
        /// <param name="orgId">ID da organização</param>
        /// <param name="itemLoteId">ID do item do lote</param>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se processado com sucesso</returns>
        Task<bool> ProcessarItemHoleriteAsync(FilaMessageDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId);

        /// <summary>
        /// Obtém o sumário de um holerite
        /// </summary>
        /// <param name="arquivoPdf">Arquivo PDF do holerite</param>
        /// <param name="codigoColaborador">Código do colaborador</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Sumário do holerite</returns>
        Task<SumarioHoleriteResult> ObterSumarioHoleriteAsync(byte[] arquivoPdf, string codigoColaborador, int orgId);

        /// <summary>
        /// Busca lotes de holerite por organização
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de lotes de holerite</returns>
        Task<List<LoteFilaResult>> BuscarLotesPorOrgAsync(int orgId);

        /// <summary>
        /// Detalha um lote específico de holerite
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Detalhes do lote de holerite</returns>
        Task<DetalhesLoteHoleriteResult> DetalharLoteAsync(string loteId);

        /// <summary>
        /// Deleta um lote de holerite
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Task</returns>
        Task DeletarLoteAsync(string loteId);

        /// <summary>
        /// Deleta um lote de holerite de adiantamento, incluindo todos os itens de lote e o próprio lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Task</returns>
        Task DeletarLoteAdiantamentoAsync(string loteId, int orgId);
        
        /// <summary>
        /// Deleta um lote de holerite de férias, incluindo todos os itens de lote e o próprio lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Task</returns>
        Task DeletarLoteFeriasAsync(string loteId, int orgId);
        
        /// <summary>
        /// Deleta um lote de  holerite de adiantamento do decimo terceiro, incluindo todos os itens de lote e o próprio lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Task</returns>
        Task DeletarLoteAdiantamentoDecimoTerceiroAsync(string loteId, int orgId);
        
        /// <summary>
        /// Deleta um lote de holerite de décimo terceiro, incluindo todos os itens de lote e o próprio lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Task</returns>
        Task DeletarLoteDecimoTerceiroAsync(string loteId, int orgId);
        
        /// <summary>
        /// Deleta um lote de holerite de informe de rendimentos, incluindo todos os itens de lote e o próprio lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Task</returns>
        Task DeletarLoteInformeDeRendimentoAsync(string loteId, int orgId);

        /// <summary>
        /// Reenfileira os itens de lote informados para reprocessamento.
        /// Para cada item encontrado, redefine seu status e envia nova mensagem à fila SQS.
        /// Itens não encontrados são reportados no resultado.
        /// </summary>
        /// <param name="itensLoteId">Lista de IDs de itens de lote a reprocessar</param>
        /// <param name="codigoColaborador">CPF do colaborador solicitante</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Resumo do reprocessamento iniciado</returns>
        Task<ReprocessarItensHoleriteResult> ReprocessarItensHoleriteAsync(List<string> itensLoteId, string codigoColaborador, int orgId);
    }
} 