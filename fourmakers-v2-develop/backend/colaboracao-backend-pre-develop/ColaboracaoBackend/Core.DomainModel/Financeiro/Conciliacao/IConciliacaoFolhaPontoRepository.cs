using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Conciliacao;

namespace Core.Domain.Financeiro.Conciliacao
{
    public interface IConciliacaoFolhaPontoRepository
    {
        /// <summary>
        /// Cria uma nova conciliação de folha ponto
        /// </summary>
        /// <param name="conciliacaoFolhaPonto">Dados da conciliação de folha ponto</param>
        /// <returns>True se criado com sucesso</returns>
        Task<bool> CriarConciliacaoFolhaPontoAsync(ConciliacaoFolhaPontoDTO conciliacaoFolhaPonto);

        /// <summary>
        /// Cria uma nova conciliação de folha ponto para um colaborador
        /// </summary>
        /// <param name="conciliacaoFolhaPontoColaborador">Dados da conciliação de folha ponto do colaborador</param>
        /// <returns>True se criado com sucesso</returns>
        Task<bool> CriarConciliacaoFolhaPontoColaboradorAsync(ConciliacaoFolhaPontoColaboradorDTO conciliacaoFolhaPontoColaborador);

        /// <summary>
        /// Cria uma nova divergência na conciliação de folha ponto
        /// </summary>
        /// <param name="conciliacaoFolhaPontoDivergencia">Dados da divergência</param>
        /// <returns>True se criado com sucesso</returns>
        Task<bool> CriarConciliacaoFolhaPontoDivergenciaAsync(ConciliacaoFolhaPontoDivergenciaDTO conciliacaoFolhaPontoDivergencia);

        /// <summary>
        /// Valida se o CNPJ pertence ao projeto da organização
        /// </summary>
        /// <param name="cnpj">CNPJ do projeto</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>True se o CNPJ pertence ao projeto da organização</returns>
        Task<bool> ValidaCnpjProjetoOrgAsync(string cnpj, int orgId);

        /// <summary>
        /// Obtém a modalidade de pagamento de horas extras
        /// </summary>
        /// <param name="cnpj">CNPJ do projeto</param>
        /// <param name="orgId">ID da organização</returns>
        Task<string> ObterModalidadePagamentoHorasExtrasAsync(string cnpj, int orgId);

        /// <summary>
        /// Obtém o nome da empresa
        /// </summary>
        /// <param name="cnpj">CNPJ do projeto</param>
        /// <param name="orgId">ID da organização</returns>
        Task<string> ObterNomeEmpresaAsync(string cnpj, int orgId);

        /// <summary>
        /// Busca os lotes de conciliação de folha ponto
        /// </summary>
        /// <param name="orgId">ID da organização</returns>
        Task<List<LoteFilaConciliacaoDTO>> BuscarLotesPorOrgAsync(int orgId);

        /// <summary>
        /// Busca os itens de conciliação de folha ponto
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Lista de itens de conciliação de folha ponto</returns>
        Task<List<ItemConciliacaoColaboradorDTO>> BuscarItensConciliacaoColaboradorAsync(string loteId);

        /// <summary>
        /// Busca a conciliação de folha ponto por item do lote
        /// </summary>
        /// <param name="itemLoteId">ID do item do lote</param>
        /// <returns>Dados da conciliação de folha ponto</returns>
        Task<ConciliacaoFolhaPontoDTO> BuscarConciliacaoFolhaPontoPorItemLoteAsync(string itemLoteId);

        /// <summary>
        /// Busca a conciliação de folha ponto por lote
        /// </summary>
        /// <param name="itemLoteId">ID do item do lote</param>
        /// <returns>Dados da conciliação de folha ponto</returns>
        Task<ConciliacaoFolhaPontoDTO> BuscarConciliacaoFolhaPontoPorLoteAsync(string itemLoteId);

        /// <summary>
        /// Verifica se o lote contém divergências
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se o lote contém divergências</returns>
        Task<bool> GetLoteContemDivergenciasAsync(string loteId);

        /// <summary>
        /// Atualiza o status de aprovação da conciliação de folha ponto
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="loteId">ID do lote</param>
        /// <param name="aprovado">Status de aprovação (true = aprovado, false = não aprovado)</param>
        /// <returns>True se atualizado com sucesso</returns>
        Task<bool> AtualizarStatusAprovacaoAsync(int orgId, string loteId, bool aprovado);

        /// <summary>
        /// Inativa todas as conciliações da organização para um determinado CNPJ e competência
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="cnpj">CNPJ do projeto</param>
        /// <param name="competencia">Competência (formato MM/AAAA)</param>
        /// <returns>True se inativado com sucesso</returns>
        Task<bool> InativarConciliacoesPorCnpjCompetenciaAsync(int orgId, string cnpj, string competencia);

        /// <summary>
        /// Desaprova a conciliação de folha ponto de uma organização para um CNPJ e competência específicos
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="cnpj">CNPJ do projeto</param>
        /// <param name="competencia">Competência (formato MM/AAAA)</param>
        /// <returns>True se desaprovado com sucesso</returns>
        Task<bool> DesaprovarConciliacaoPorCnpjCompetenciaAsync(int orgId, string cnpj, string competencia);
    }
} 