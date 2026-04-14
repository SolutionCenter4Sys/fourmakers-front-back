using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Core.Domain.Financeiro.Holerite
{
    public interface IHoleriteRepository
    {
        /// <summary>
        /// Busca lotes de holerite por organização e tipo de fila
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="tipoFila">Tipo da fila</param>
        /// <returns>Lista de lotes de holerite</returns>
        Task<List<LoteFilaHoleriteDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila);

        /// <summary>
        /// Detalha um lote específico de holerite
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Detalhes do lote de holerite</returns>
        Task<LoteFilaHoleriteDTO> DetalharLoteAsync(string loteId);

        /// <summary>
        /// Insere um holerite de colaborador
        /// </summary>
        /// <param name="holeriteColaborador">Dados do holerite do colaborador</param>
        /// <returns>True se inserido com sucesso</returns>
        Task<bool> InserirHoleriteColaboradorAsync(HoleriteColaboradorDTO holeriteColaborador);

        /// <summary>
        /// Busca holerites de um colaborador por competência e organização
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="competencia">Competência no formato MM/YYYY</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de holerites do colaborador</returns>
        Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorColaboradorAsync(string codigoInternoColaborador, string competencia, string cnpj, int orgId);

        /// <summary>
        /// Deleta todos os holerites de um colaborador em uma competência específica
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="competencia">Competência no formato MM/YYYY</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId);

        /// <summary>
        /// Lista todos os holerites de um colaborador perante ao ano atribuido
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="ano">Ano</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de holerites do colaborador</returns>
        Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorColaboradorPorAnoAsync(string codigoInternoColaborador, int ano, int orgId);

        /// <summary>
        /// Traz o Holerite por ItemLoteID do colaborador
        /// </summary>
        /// <param name="itemLoteId">Codigo do item no lote</param>
        /// <returns>Holerite do colaborador por Item Lote ID</returns>
        Task<HoleriteColaboradorDTO> BuscarHoleriteColaboradorPorItemLoteIdAsync(string itemLoteId);
        
        /// <summary>
        /// Assina um Holerite do Colaborador
        /// </summary>
        /// <param name="itemLoteId">Codigo do item no lote</param>
        /// <returns>Assina Holerite do colaborador por Item Lote ID</returns>
        Task AssinarHoleriteColaborador(string itemLoteId);

        /// <summary>
        /// Deleta todos os holerites de adiantamento de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesAdiantamentoPorLoteAsync(string loteId);
        
        /// <summary>
        /// Deleta todos os holerites de ferias de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesFeriasPorLoteAsync(string loteId);
        
        /// <summary>
        /// Deleta todos os holerites de decimo terceiro de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesDecimoTerceiroPorLoteAsync(string loteId);
        
        /// <summary>
        /// Deleta todos os holerites de informes de rendimentos de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesInformeDeRendimentosPorLoteAsync(string loteId);
        
        /// <summary>
        /// Deleta todos os holerites de adiantamento do decimo terceiro de um lote
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarHoleritesAdiantamentoDecimoTerceiroPorLoteAsync(string loteId);
        
        Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorCompetenciaAsync(string competencia, string cnpj, int orgId);
        /// <summary>
        /// Atualiza um holerite de colaborador
        /// </summary>
        /// <param name="itemLoteId">Codigo do item no lote</param>
        /// <param name="objetoHoleriteString">Objeto holerite string</param>
        /// <returns>True se atualizado com sucesso</returns>
        Task<bool> AtualizarHoleriteColaboradorAsync(string itemLoteId, string objetoHoleriteString);

        /// <summary>
        /// Verifica se existe um holerite vinculado ao item de lote informado
        /// </summary>
        /// <param name="itemLoteId">ID do item do lote</param>
        /// <returns>True se o registro existe</returns>
        Task<bool> ExisteHoleriteColaboradorPorItemLoteIdAsync(string itemLoteId);

        /// <summary>
        /// Lista holerites com líquido extraído do retorno do item de lote, filtrados por competência (mês/ano),
        /// organização e conciliação de folha ponto aprovada. Filtro opcional por código de diretoria.
        /// </summary>
        Task<List<HoleriteLiquidoConciliadoExternoDTO>> ListarHoleritesLiquidosConciliacaoFolhaPontoAprovadaAsync(
            int orgId,
            int mes,
            int ano,
            string codDiretoria);
    }
} 