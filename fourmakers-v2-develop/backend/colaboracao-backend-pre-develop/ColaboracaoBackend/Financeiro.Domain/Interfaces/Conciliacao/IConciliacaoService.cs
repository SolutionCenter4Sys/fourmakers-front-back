using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace Financeiro.Domain.Interfaces.Conciliacao
{
    public interface IConciliacaoService
    {
        /// <summary>
        /// Cria um novo lote de conciliação de folha ponto
        /// </summary>
        /// <param name="cnpj">CNPJ da empresa</param>
        /// <param name="competencia">Competência no formato MM/YYYY</param>
        /// <param name="orgId">ID da organização</param>
        /// <param name="codigoInternoSolicitante">Código interno do solicitante</param>
        /// <returns>Resultado da criação do lote</returns>
        Task<SumarioConciliacaoResult> CriarLoteConciliacaoAsync(string cnpj, string competencia, int orgId, string codigoInternoSolicitante);

        /// <summary>
        /// Busca os lotes de conciliação de folha ponto
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="orgId">ID da organização</returns>
        Task<List<SumarioConciliacaoResult>> BuscarLotesPorOrgAsync(string codigoInternoColaborador, int orgId);

        /// <summary>
        /// Busca os itens de conciliação de folha ponto
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</returns>
        Task<ConciliacaoLoteResult> BuscarItensConciliacaoColaboradorAsync(string loteId, int orgId);

        /// <summary>
        /// Processa o item de conciliação de folha ponto
        /// </summary>
        /// <param name="retornoAnaliseHoleriteDTO">Retorno da análise do holerite</param>
        /// <param name="orgId">ID da organização</param>
        /// <param name="itemLoteId">ID do item de lote</param>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        Task ProcessarItemConciliacaoAsync(RetornoAnaliseHoleriteDTO retornoAnaliseHoleriteDTO, int orgId, string itemLoteId, string codigoInternoColaborador);

        /// <summary>
        /// Exporta as divergências de um lote para um arquivo Excel
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Arquivo Excel com as divergências</returns>
        Task<ApiGenericResult<FileContentResult>> ExportarDivergenciasLoteAsync(string loteId, int orgId);

        /// <summary>
        /// Lista o status de vigência da conciliação para uma organização
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de CNPJs e vigências com status de processamento</returns>
        Task<List<StatusVigenciaConciliacaoDTO>> ListarStatusVigenciaConciliacaoAsync(int orgId);

        /// <summary>
        /// Aprova uma conciliação de folha ponto
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se aprovado com sucesso</returns>
        Task<bool> AprovarConciliacaoAsync(int orgId, string loteId);
    }
} 