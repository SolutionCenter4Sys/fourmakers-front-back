using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Core.Domain.Apontamento
{
    public interface IFolhaPontoRepository
    {
        /// <summary>
        /// Busca todos os lotes de uma organização
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="tipoFila">Tipo da fila</param>
        /// <returns>Lista de lotes da organização</returns>
        Task<List<LoteFilaFolhaDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila);

        /// <summary>
        /// Obtém o lote pelo ID
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Lote encontrado</returns>
        Task<LoteFilaFolhaDTO> DetalharLoteAsync(string loteId);

        /// <summary>
        /// Insere uma folha de ponto de colaborador
        /// </summary>
        /// <param name="folhaPontoColaborador">Dados da folha de ponto do colaborador</param>
        /// <returns>True se inserido com sucesso</returns>
        Task<bool> InserirFolhaPontoColaboradorAsync(FolhaPontoColaboradorDTO folhaPontoColaborador);

        /// <summary>
        /// Busca folhas de ponto de colaborador por colaborador
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="competencia">Competência (mes/ano)</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de folhas de ponto encontradas</returns>
        Task<List<FolhaPontoColaboradorDTO>> BuscarFolhasPontoPorColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId);

        /// <summary>
        /// Deleta todas as folhas de ponto de um colaborador específico de uma organização em uma determinada competência
        /// </summary>
        /// <param name="codigoInternoColaborador">Código interno do colaborador</param>
        /// <param name="competencia">Competência (mes/ano)</param>
        /// <param name="orgId">ID da organização</param>
        /// <returns>True se deletado com sucesso</returns>
        Task<bool> DeletarFolhasPontoColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId);

        /// <summary>
        /// Busca folhas de ponto de uma organização e uma competência
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <param name="competencia">Competência (mes/ano)</param>
        /// <returns>Lista de folhas de ponto encontradas</returns>
        Task<List<FolhaPontoColaboradorDTO>> BuscarFolhasPontoPorOrgECompetenciaAsync(int orgId, string cnpj, string competencia);

        /// <summary>
        /// Lista as unidades de uma organização
        /// </summary>
        /// <param name="orgId">ID da organização</param>
        /// <returns>Lista de unidades encontradas</returns>
        Task<List<KeyValuePair<string, string>>> ListarUnidadesPorOrg(int orgId);

        Task<FolhaPontoColaboradorDTO> BuscarFolhaPontoColaboradorPorLoteIdAsync(int orgId, string competencia, string codigoInternoColaborador);
    }
} 