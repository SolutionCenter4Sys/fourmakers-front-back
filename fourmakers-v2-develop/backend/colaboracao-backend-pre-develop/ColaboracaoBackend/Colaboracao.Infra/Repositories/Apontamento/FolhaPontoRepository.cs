using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Apontamento;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Apontamento
{
    public class FolhaPontoRepository : IFolhaPontoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public FolhaPontoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<LoteFilaFolhaDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarLotesSql = @"
                    SELECT 
                        l.id AS Id,
                        l.quantidade_paginas AS QuantidadeDePaginas,
                        l.file_path AS Pdf,
                        l.data_criacao AS DataCriacao,
                        l.data_finalizacao AS DataFinalizacao,
                        l.aprovado_para_processamento AS AprovadoParaProcessamento,
                        l.sumario AS SumarioFolhaPontoString
                    FROM tb_lote l
                    INNER JOIN tb_fila f ON l.tb_fila_id = f.id
                    WHERE l.tb_org_id = @OrgId
                    AND f.descricao = @Fila
                    ORDER BY l.data_criacao DESC";

                var lotes = await connection.QueryAsync<LoteFilaFolhaDTO>(
                    buscarLotesSql, 
                    new { OrgId = orgId, Fila = tipoFila.ToString() }
                );

                return lotes.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<LoteFilaFolhaDTO> DetalharLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string detalharLoteSql = @"
                    SELECT 
                        l.id AS Id,
                        l.quantidade_paginas AS QuantidadeDePaginas,
                        l.file_path AS Pdf,
                        l.data_criacao AS DataCriacao,
                        l.data_finalizacao AS DataFinalizacao,
                        l.aprovado_para_processamento AS AprovadoParaProcessamento,
                        l.sumario AS SumarioFolhaPontoString
                    FROM tb_lote l WHERE l.id = @LoteId";  

                var lote = await connection.QueryFirstOrDefaultAsync<LoteFilaFolhaDTO>(detalharLoteSql, new { LoteId = loteId });

                return lote;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InserirFolhaPontoColaboradorAsync(FolhaPontoColaboradorDTO folhaPontoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inserirFolhaPontoColaboradorSql = @"
                    INSERT INTO tb_folhaponto_colaborador (
                        tb_item_lote_id,
                        codigo_interno_colaborador,
                        objeto_folhaponto,
                        competencia,
                        cnpj
                    ) VALUES (
                        @TbItemLoteId,
                        @CodigoInternoColaborador,
                        @ObjetoFolhaPontoString,
                        @Competencia,
                        @Cnpj
                    )";

                var linhasInseridas = await connection.ExecuteAsync(inserirFolhaPontoColaboradorSql, folhaPontoColaborador);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FolhaPontoColaboradorDTO>> BuscarFolhasPontoPorColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarFolhasPontoPorColaboradorSql = @"
                    SELECT 
                        tfc.tb_item_lote_id AS TbItemLoteId,
                        tfc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tfc.objeto_folhaponto AS ObjetoFolhaPontoString,
                        tfc.competencia AS Competencia,
                        tfc.cnpj AS Cnpj,
                        til.file_path AS FolhaPdf
                    FROM tb_folhaponto_colaborador tfc
                        INNER JOIN tb_item_lote til ON tfc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE tfc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tfc.competencia = @Competencia
                    AND tl.tb_org_id = @OrgId
                    ORDER BY competencia DESC";

                var folhasPonto = await connection.QueryAsync<FolhaPontoColaboradorDTO>(buscarFolhasPontoPorColaboradorSql, new { CodigoInternoColaborador = codigoInternoColaborador, Competencia = competencia, OrgId = orgId });

                return folhasPonto.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FolhaPontoColaboradorDTO>> BuscarFolhasPontoPorOrgECompetenciaAsync(int orgId, string cnpj, string competencia)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarFolhasPontoPorOrgECompetenciaSql = @"
                    SELECT 
                        tfc.tb_item_lote_id AS TbItemLoteId,
                        tfc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tfc.objeto_folhaponto AS ObjetoFolhaPontoString,
                        tfc.competencia AS Competencia,
                        tfc.cnpj AS Cnpj,
                        til.file_path AS FolhaPdf
                    FROM tb_folhaponto_colaborador tfc
                        INNER JOIN tb_item_lote til ON tfc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE tl.tb_org_id = @OrgId
                    AND tfc.competencia = @Competencia
                    AND tfc.cnpj = @Cnpj
                    ORDER BY tfc.competencia DESC";

                var folhasPonto = await connection.QueryAsync<FolhaPontoColaboradorDTO>(buscarFolhasPontoPorOrgECompetenciaSql, new { OrgId = orgId, Competencia = competencia, Cnpj = cnpj });

                return folhasPonto.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<FolhaPontoColaboradorDTO> BuscarFolhaPontoColaboradorPorLoteIdAsync(int orgId, string competencia, string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarFolhasPontoPorOrgECompetenciaSql = @"
                    SELECT 
                        tfc.tb_item_lote_id AS TbItemLoteId,
                        tfc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tfc.objeto_folhaponto AS ObjetoFolhaPontoString,
                        tfc.competencia AS Competencia,
                        tfc.cnpj AS Cnpj,
                        til.file_path AS FolhaPdf
                    FROM tb_folhaponto_colaborador tfc
                        INNER JOIN tb_item_lote til ON tfc.tb_item_lote_id = til.id  
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE tl.tb_org_id = @OrgId
                    AND tfc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tfc.competencia = @Competencia
                    ORDER BY tfc.competencia DESC";

                var folhasPonto = await connection.QuerySingleOrDefaultAsync<FolhaPontoColaboradorDTO>(buscarFolhasPontoPorOrgECompetenciaSql, new { OrgId = orgId, Competencia = competencia, CodigoInternoColaborador = codigoInternoColaborador });

                return folhasPonto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeletarFolhasPontoColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarFolhasPontoColaboradorSql = @"
                    DELETE tfc FROM tb_folhaponto_colaborador tfc
                    INNER JOIN tb_item_lote til ON tfc.tb_item_lote_id = til.id
                    INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE tfc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND tfc.competencia = @Competencia
                    AND tl.tb_org_id = @OrgId";

                var linhasDeletadas = await connection.ExecuteAsync(deletarFolhasPontoColaboradorSql, new 
                { 
                    CodigoInternoColaborador = codigoInternoColaborador, 
                    Competencia = competencia, 
                    OrgId = orgId 
                });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<KeyValuePair<string, string>>> ListarUnidadesPorOrg(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string listarUnidadesPorOrgSql = @"
                    SELECT
                            cod_diretoria AS Id,
                            diretoria AS Descricao
                        FROM tb_colaborador_org
                        WHERE tb_org_id = @OrgId
                        AND ativo = 1
                        AND diretoria IS NOT NULL AND diretoria != ''
                        GROUP BY cod_diretoria";

                var unidades = await connection.QueryAsync<dynamic>(listarUnidadesPorOrgSql, new 
                { 
                    OrgId = orgId 
                });

                return unidades.Select(x => new KeyValuePair<string, string>(x.Id, x.Descricao)).AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
} 