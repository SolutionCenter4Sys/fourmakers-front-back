using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Core.Domain.Apontamento
{
    public interface ILoteRepository
    {
        /// <summary>
        /// Cria um novo lote para processamento
        /// </summary>
        /// <param name="quantidadePaginas">Quantidade de páginas do documento</param>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <param name="orgId">ID da organização</param>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="tipoFila">Tipo da fila</param>
        /// <param name="sumario">Sumário em JSON</param>
        /// <returns>ID do lote criado</returns>
        Task<string> CriarLoteAsync(int quantidadePaginas, string filePath, int orgId, long usuarioId, TipoFilaEnum tipoFila, string sumario);

        /// <summary>
        /// Atualiza um lote com o resultado do processamento
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="aprovadoParaProcessamento">Indica se o lote foi aprovado para processamento</param>
        /// <param name="dataFinalizacao">Data de finalização do processamento</param>
        /// <param name="sumario">Sumário em JSON</param>
        /// <returns>True se atualizado com sucesso</returns>
        Task<bool> AtualizarLoteAsync(string loteId, bool aprovadoParaProcessamento, DateTime? dataFinalizacao, string sumario = null);

        /// <summary>
        /// Deleta um lote pelo ID
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarLoteAsync(string loteId);

        /// <summary>
        /// Insere um novo item de lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="filePath">Caminho do arquivo da página</param>
        /// <returns>ID do item de lote criado</returns>
        Task<string> InserirItemLoteAsync(string loteId, string filePath);

        /// <summary>
        /// Obtém os itens do lote pelo ID
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Itens do lote</returns>
        Task<List<ItemLoteDTO>> ListarItensLoteAsync(string loteId);

        /// <summary>
        /// Atualiza um item de lote com o resultado do processamento
        /// </summary>
        /// <param name="itemLoteId">ID do item de lote</param>
        /// <param name="sucesso">Indica se o processamento foi bem-sucedido</param>
        /// <param name="retorno">Retorno do processamento em JSON</param>
        /// <param name="dataFinalizacao">Data de finalização do processamento</param>
        /// <returns>True se atualizado com sucesso</returns>
        Task<bool> AtualizarItemLoteAsync(string itemLoteId, bool sucesso, string retorno, DateTime? dataFinalizacao = null);

        /// <summary>
        /// Obtém o total de itens de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Total de itens do lote</returns>
        Task<int> GetTotalItensLoteAsync(string loteId);

        /// <summary>
        /// Obtém o total de itens processados de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Total de itens processados do lote</returns>
        Task<int> GetTotalItensProcessadosLoteAsync(string loteId);

        /// <summary>
        /// Obtém o total de itens processados com erro de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Total de itens processados com erro do lote</returns>
        Task<int> GetTotalItensProcessadosComErroLoteAsync(string loteId);

        /// <summary>
        /// Obtém um item de lote pelo seu ID
        /// </summary>
        /// <param name="itemLoteId">ID do item de lote</param>
        /// <returns>Item de lote encontrado ou null</returns>
        Task<ItemLoteDTO> ObterItemLotePorIdAsync(string itemLoteId);

        /// <summary>
        /// Redefine o status de um item de lote para pendente,
        /// limpando data de finalização, resultado e flag de sucesso
        /// </summary>
        /// <param name="itemLoteId">ID do item de lote</param>
        Task ResetarItemLoteAsync(string itemLoteId);
    }
} 