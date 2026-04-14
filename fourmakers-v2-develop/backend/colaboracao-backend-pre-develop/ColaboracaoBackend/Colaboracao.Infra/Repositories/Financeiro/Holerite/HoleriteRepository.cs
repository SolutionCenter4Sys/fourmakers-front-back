using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Financeiro.Holerite;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Colaboracao.Infra.Repositories.Financeiro.Holerite
{
    public class HoleriteRepository : IHoleriteRepository
    {
        private readonly IDBConnection _dapperConnection;

        public HoleriteRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<LoteFilaHoleriteDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila)
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
                        l.sumario AS SumarioHoleriteString
                    FROM tb_lote l
                    INNER JOIN tb_fila f ON l.tb_fila_id = f.id
                    WHERE l.tb_org_id = @OrgId
                    AND f.descricao = @Fila
                    ORDER BY l.data_criacao DESC";

                var lotes = await connection.QueryAsync<LoteFilaHoleriteDTO>(
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

        public async Task<LoteFilaHoleriteDTO> DetalharLoteAsync(string loteId)
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
                        l.sumario AS SumarioHoleriteString
                    FROM tb_lote l WHERE l.id = @LoteId";  

                var lote = await connection.QueryFirstOrDefaultAsync<LoteFilaHoleriteDTO>(detalharLoteSql, new { LoteId = loteId });

                return lote;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InserirHoleriteColaboradorAsync(HoleriteColaboradorDTO holeriteColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string inserirHoleriteColaboradorSql = @"
                    INSERT INTO tb_holerite_colaborador (
                        tb_item_lote_id,
                        codigo_interno_colaborador,
                        objeto_holerite,
                        competencia,
                        cnpj,
                        adiantamento,
                        ferias,
                        decimo_terceiro,
                        decimo_terceiro_adiantamento,
                        informe_rendimentos
                    ) VALUES (
                        @TbItemLoteId,
                        @CodigoInternoColaborador,
                        @ObjetoHoleriteString,
                        @Competencia,
                        @Cnpj,
                        @Adiantamento,
                        @Ferias,
                        @DecimoTerceiro,
                        @DecimoTerceiroAdiantamento,
                        @InformeDeRendimentos
                    )";

                var linhasInseridas = await connection.ExecuteAsync(inserirHoleriteColaboradorSql, holeriteColaborador);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorColaboradorAsync(string codigoInternoColaborador, string competencia, string cnpj, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarHoleritesPorColaboradorSql = @"
                    SELECT 
                        thc.tb_item_lote_id AS TbItemLoteId,
                        thc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        thc.objeto_holerite AS ObjetoHoleriteString,
                        thc.competencia AS Competencia,
                        thc.cnpj AS Cnpj,
                        til.file_path AS HoleritePdf,
                        thc.adiantamento AS Adiantamento,
                        thc.ferias AS Ferias,
                        thc.decimo_terceiro AS DecimoTerceiro,
                        thc.informe_rendimentos AS InformeDeRendimentos
                    FROM tb_holerite_colaborador thc
                        INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE thc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND thc.competencia = @Competencia
                    AND tl.tb_org_id = @OrgId
                    AND thc.cnpj = @Cnpj
                    ORDER BY competencia DESC";

                var holerites = await connection.QueryAsync<HoleriteColaboradorDTO>(buscarHoleritesPorColaboradorSql, new { CodigoInternoColaborador = codigoInternoColaborador, Competencia = competencia, OrgId = orgId, Cnpj = cnpj });

                return holerites.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorCompetenciaAsync(string competencia, string cnpj, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarHoleritesPorColaboradorSql = @"
                    SELECT 
                        thc.tb_item_lote_id AS TbItemLoteId,
                        thc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        thc.objeto_holerite AS ObjetoHoleriteString,
                        thc.competencia AS Competencia,
                        thc.cnpj AS Cnpj,
                        til.file_path AS HoleritePdf,
                        thc.adiantamento AS Adiantamento
                    FROM tb_holerite_colaborador thc
                        INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE 
                    thc.competencia = @Competencia
                    AND tl.tb_org_id = @OrgId
                    AND thc.cnpj = @Cnpj
                    AND thc.adiantamento = 0
                    ORDER BY competencia DESC";

                var holerites = await connection.QueryAsync<HoleriteColaboradorDTO>(buscarHoleritesPorColaboradorSql, new { Competencia = competencia, OrgId = orgId, Cnpj = cnpj });

                return holerites.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeletarHoleritesColaboradorAsync(string codigoInternoColaborador, string competencia, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesColaboradorSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                    WHERE thc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND thc.competencia = @Competencia
                    AND tl.tb_org_id = @OrgId
                    AND thc.adiantamento = 0
                    AND thc.decimo_terceiro = 0
                    AND thc.decimo_terceiro_adiantamento = 0
                    AND thc.ferias = 0";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesColaboradorSql, new 
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
        
        public async Task<List<HoleriteColaboradorDTO>> BuscarHoleritesPorColaboradorPorAnoAsync(string codigoInternoColaborador, int ano, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarHoleritesPorColaboradorSql = @"
                    SELECT DISTINCT
                        thc.tb_item_lote_id AS TbItemLoteId,
                        thc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        thc.objeto_holerite AS ObjetoHoleriteString,
                        thc.competencia AS Competencia,
                        thc.cnpj AS Cnpj,
                        til.file_path AS HoleritePdf,
                        thc.assinado AS Assinado,
                        thc.assinado_em AS AssinadoEm,
                        thc.adiantamento AS Adiantamento,
                        thc.ferias AS Ferias,
                        thc.decimo_terceiro AS DecimoTerceiro,
                        thc.decimo_terceiro_adiantamento AS DecimoTerceiroAdiantamento,
                        thc.informe_rendimentos AS InformeDeRendimentos
                    FROM tb_holerite_colaborador thc
                        INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                        LEFT JOIN tb_conciliacao_folhaponto_colaborador tcfc on tcfc.codigo_interno_colaborador = thc.codigo_interno_colaborador 
                        LEFT JOIN tb_item_lote til2 ON tcfc.tb_item_lote_id = til2.id
                        LEFT JOIN tb_conciliacao_folhaponto tcf ON til2.tb_lote_id  = tcf.tb_lote_id AND tcf.cnpj = thc.cnpj AND tcf.competencia = thc.competencia
                    WHERE thc.codigo_interno_colaborador = @CodigoInternoColaborador
                    AND YEAR(STR_TO_DATE(thc.competencia, '%m/%Y')) = @Ano
                    AND tl.tb_org_id = @OrgId
                    AND (tcf.aprovado = 1 OR thc.adiantamento = 1 OR thc.ferias = 1 OR thc.decimo_terceiro = 1 OR thc.decimo_terceiro_adiantamento = 1 OR thc.informe_rendimentos = 1)
                    ORDER BY competencia DESC";

                var holerites = await connection.QueryAsync<HoleriteColaboradorDTO>(buscarHoleritesPorColaboradorSql, new { CodigoInternoColaborador = codigoInternoColaborador, Ano = ano, OrgId = orgId });

                return holerites.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<HoleriteColaboradorDTO> BuscarHoleriteColaboradorPorItemLoteIdAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarHoleritesPorColaboradorSql = @"
                    SELECT 
                        thc.tb_item_lote_id AS TbItemLoteId,
                        thc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        thc.objeto_holerite AS ObjetoHoleriteString,
                        thc.competencia AS Competencia,
                        thc.cnpj AS Cnpj,
                        til.file_path AS HoleritePdf,
                        thc.assinado AS Assinado,
                        thc.assinado_em AS AssinadoEm
                    FROM tb_holerite_colaborador thc
                        INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                        INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
                        INNER JOIN tb_conciliacao_folhaponto_colaborador tcfc on tcfc.codigo_interno_colaborador = tcfc.codigo_interno_colaborador 
                        INNER JOIN tb_item_lote til2 ON tcfc.tb_item_lote_id = til2.id
                        INNER JOIN tb_conciliacao_folhaponto tcf ON til2.tb_lote_id  = tcf.tb_lote_id
                    WHERE thc.tb_item_lote_id = @ItemLoteId
                        AND tcf.aprovado = 1
                    ORDER BY competencia DESC
                    LIMIT 1;
                ";

                var holerite = await connection.QuerySingleOrDefaultAsync<HoleriteColaboradorDTO>(buscarHoleritesPorColaboradorSql, new { ItemLoteId = itemLoteId});

                return holerite;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssinarHoleriteColaborador(string loteItemID)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                UPDATE tb_holerite_colaborador
                SET 
                    assinado = 1,
                    assinado_em = NOW()
                WHERE tb_item_lote_id = @ItemLoteId
            ";
    
            await connection.ExecuteAsync(query, new { ItemLoteId = loteItemID });
        }

        public async Task<bool> DeletarHoleritesAdiantamentoPorLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesAdiantamentoSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    WHERE til.tb_lote_id = @LoteId
                    AND thc.adiantamento = 1";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesAdiantamentoSql, new { LoteId = loteId });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<bool> DeletarHoleritesFeriasPorLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesFeriasSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    WHERE til.tb_lote_id = @LoteId
                    AND thc.ferias = 1";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesFeriasSql, new { LoteId = loteId });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeletarHoleritesDecimoTerceiroPorLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesDecimoTerceiroSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    WHERE til.tb_lote_id = @LoteId
                    AND thc.decimo_terceiro = 1";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesDecimoTerceiroSql, new { LoteId = loteId });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<bool> DeletarHoleritesInformeDeRendimentosPorLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesDecimoTerceiroSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    WHERE til.tb_lote_id = @LoteId
                    AND thc.informe_rendimentos = 1";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesDecimoTerceiroSql, new { LoteId = loteId });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<bool> DeletarHoleritesAdiantamentoDecimoTerceiroPorLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarHoleritesDecimoTerceiroSql = @"
                    DELETE thc FROM tb_holerite_colaborador thc
                    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
                    WHERE til.tb_lote_id = @LoteId
                    AND thc.decimo_terceiro_adiantamento = 1";

                var linhasDeletadas = await connection.ExecuteAsync(deletarHoleritesDecimoTerceiroSql, new { LoteId = loteId });

                return linhasDeletadas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AtualizarHoleriteColaboradorAsync(string itemLoteId, string objetoHoleriteString)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                UPDATE tb_holerite_colaborador
                SET objeto_holerite = @ObjetoHoleriteString
                WHERE tb_item_lote_id = @ItemLoteId
            ";
            await connection.ExecuteAsync(query, new { ItemLoteId = itemLoteId, ObjetoHoleriteString = objetoHoleriteString });
            return true;
        }

        public async Task<bool> ExisteHoleriteColaboradorPorItemLoteIdAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT COUNT(1)
                FROM tb_holerite_colaborador
                WHERE tb_item_lote_id = @ItemLoteId
            ";
            var count = await connection.ExecuteScalarAsync<int>(query, new { ItemLoteId = itemLoteId });
            return count > 0;
        }

        public async Task<List<HoleriteLiquidoConciliadoExternoDTO>> ListarHoleritesLiquidosConciliacaoFolhaPontoAprovadaAsync(
            int orgId,
            int mes,
            int ano,
            string codDiretoria)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                const string sql = @"
SELECT DISTINCT
    tco.cod_diretoria AS CodDiretoria,
    tco.diretoria AS NomeDiretoria,
    tc.nome_completo AS Nome,
    tc.documento_colaborador AS Cpf,
    JSON_UNQUOTE(JSON_EXTRACT(til.retorno, '$.Holerite.Totais.Liquido')) AS ValorLiquido,
    thc.competencia AS Competencia
FROM tb_holerite_colaborador thc
    INNER JOIN tb_colaborador_org tco ON tco.codigo_interno_colaborador = thc.codigo_interno_colaborador AND tco.tb_org_id = @OrgId
    INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
    INNER JOIN tb_item_lote til ON thc.tb_item_lote_id = til.id
    INNER JOIN tb_lote tl ON til.tb_lote_id = tl.id
    LEFT JOIN tb_conciliacao_folhaponto_colaborador tcfc ON tcfc.codigo_interno_colaborador = thc.codigo_interno_colaborador
    LEFT JOIN tb_item_lote til2 ON tcfc.tb_item_lote_id = til2.id
    LEFT JOIN tb_conciliacao_folhaponto tcf ON til2.tb_lote_id = tcf.tb_lote_id AND tcf.cnpj = thc.cnpj AND tcf.competencia = thc.competencia
WHERE
    MONTH(STR_TO_DATE(thc.competencia, '%m/%Y')) = @Mes
    AND YEAR(STR_TO_DATE(thc.competencia, '%m/%Y')) = @Ano
    AND tl.tb_org_id = @OrgId
    AND tcf.aprovado = 1
    AND (@CodDiretoria IS NULL OR tco.cod_diretoria = @CodDiretoria)
ORDER BY thc.competencia DESC";

                var rows = await connection.QueryAsync<HoleriteLiquidoConciliadoExternoDTO>(
                    sql,
                    new { OrgId = orgId, Mes = mes, Ano = ano, CodDiretoria = codDiretoria });

                return rows.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
} 