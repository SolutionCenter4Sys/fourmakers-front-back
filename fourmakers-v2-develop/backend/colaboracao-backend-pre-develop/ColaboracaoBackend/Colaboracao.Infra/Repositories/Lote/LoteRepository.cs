using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.Apontamento;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace Colaboracao.Infra.Repositories.Lote
{
    public class LoteRepository : ILoteRepository
    {
        private readonly IDBConnection _dapperConnection;

        public LoteRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> CriarLoteAsync(int quantidadePaginas, string filePath, int orgId, long usuarioId, TipoFilaEnum tipoFila, string sumario)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string buscarFilaSql = @"
                    SELECT id 
                    FROM tb_fila 
                    WHERE descricao = @DescricaoFila";

                var filaId = await connection.QueryFirstOrDefaultAsync<string>(
                    buscarFilaSql, 
                    new { DescricaoFila = tipoFila.ToString() }
                );

                if (string.IsNullOrEmpty(filaId))
                {
                    throw new InvalidOperationException($"Fila com descrição '{tipoFila.ToString()}' não encontrada.");
                }
                string loteId = Guid.NewGuid().ToString();
                string inserirLoteSql = @"
                    INSERT INTO tb_lote (
                        id, 
                        data_criacao, 
                        aprovado_para_processamento, 
                        sumario, 
                        data_finalizacao, 
                        quantidade_paginas, 
                        file_path, 
                        tb_org_id, 
                        tb_usuario_id, 
                        tb_fila_id
                    ) VALUES (
                        @Id, 
                        CURRENT_TIMESTAMP, 
                        0, 
                        @Sumario, 
                        NULL, 
                        @QuantidadePaginas, 
                        @FilePath, 
                        @OrgId, 
                        @UsuarioId, 
                        @FilaId
                    )";

                await connection.ExecuteAsync(inserirLoteSql, new
                {
                    Id = loteId,
                    QuantidadePaginas = quantidadePaginas,
                    FilePath = filePath,
                    OrgId = orgId,
                    UsuarioId = usuarioId,
                    FilaId = filaId,
                    Sumario = sumario
                });

                return loteId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AtualizarLoteAsync(string loteId, bool aprovadoParaProcessamento, DateTime? dataFinalizacao, string sumario = null)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string atualizarLoteSql = @"
                    UPDATE tb_lote  
                    SET 
                        aprovado_para_processamento = @AprovadoParaProcessamento,
                        data_finalizacao = @DataFinalizacao,
                        sumario = IFNULL(@Sumario, sumario)
                    WHERE id = @LoteId";
                    
                var rowsAffected = await connection.ExecuteAsync(atualizarLoteSql, new
                {
                    LoteId = loteId,
                    AprovadoParaProcessamento = aprovadoParaProcessamento ? 1 : 0,
                    DataFinalizacao = dataFinalizacao,
                    Sumario = sumario
                });

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeletarLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string deletarItensLoteSql = @"
                    DELETE FROM tb_item_lote WHERE tb_lote_id = @LoteId";    
                string deletarLoteSql = @"
                    DELETE FROM tb_lote WHERE id = @LoteId";    

                var rowsAffectedItens = await connection.ExecuteAsync(deletarItensLoteSql, new { LoteId = loteId });
                var rowsAffected = await connection.ExecuteAsync(deletarLoteSql, new { LoteId = loteId });

                return rowsAffectedItens > 0 || rowsAffected > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> InserirItemLoteAsync(string loteId, string filePath)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string itemLoteId = Guid.NewGuid().ToString();
                string inserirItemLoteSql = @"
                    INSERT INTO tb_item_lote (
                        id, 
                        data_criacao, 
                        file_path, 
                        data_finalizacao, 
                        sucesso, 
                        retorno, 
                        tb_lote_id
                    ) VALUES (
                        @Id, 
                        CURRENT_TIMESTAMP, 
                        @FilePath, 
                        NULL, 
                        NULL, 
                        NULL, 
                        @LoteId
                    )";

                await connection.ExecuteAsync(inserirItemLoteSql, new
                {
                    Id = itemLoteId,
                    FilePath = filePath,
                    LoteId = loteId
                });

                return itemLoteId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ItemLoteDTO>> ListarItensLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string listarItensLoteSql = @"
                    SELECT 
                        il.id AS Id,
                        il.data_criacao AS DataCriacao,
                        il.file_path AS FilePath,
                        il.data_finalizacao AS DataFinalizacao,
                        il.sucesso AS Sucesso,
                        il.retorno AS Retorno
                    FROM tb_item_lote il WHERE il.tb_lote_id = @LoteId";    

                var itens = await connection.QueryAsync<ItemLoteDTO>(listarItensLoteSql, new { LoteId = loteId });

                return itens.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AtualizarItemLoteAsync(string itemLoteId, bool sucesso, string retorno, DateTime? dataFinalizacao = null)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string atualizarItemLoteSql = @"
                    UPDATE tb_item_lote 
                    SET 
                        data_finalizacao = @DataFinalizacao,
                        sucesso = @Sucesso,
                        retorno = @Retorno
                    WHERE id = @ItemLoteId";

                var rowsAffected = await connection.ExecuteAsync(atualizarItemLoteSql, new
                {
                    ItemLoteId = itemLoteId,
                    DataFinalizacao = dataFinalizacao ?? DateTime.Now,
                    Sucesso = sucesso ? 1 : 0,
                    Retorno = retorno
                });

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetTotalItensLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string getTotalItensLoteSql = @"
                    SELECT COUNT(*) FROM tb_item_lote WHERE tb_lote_id = @LoteId";

                var totalItens = await connection.QueryFirstOrDefaultAsync<int>(getTotalItensLoteSql, new { LoteId = loteId });

                return totalItens;
            }
            catch (Exception)
            {
                throw;  
            }
        }

        public async Task<int> GetTotalItensProcessadosLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string getTotalItensProcessadosLoteSql = @"
                    SELECT COUNT(*) FROM tb_item_lote WHERE tb_lote_id = @LoteId AND data_finalizacao IS NOT NULL";

                var totalItensProcessados = await connection.QueryFirstOrDefaultAsync<int>(getTotalItensProcessadosLoteSql, new { LoteId = loteId });

                return totalItensProcessados;
            }
            catch (Exception)
            {
                throw;  
            }
        }

        public async Task<int> GetTotalItensProcessadosComErroLoteAsync(string loteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string getTotalItensProcessadosComErroLoteSql = @"
                    SELECT COUNT(*) FROM tb_item_lote WHERE tb_lote_id = @LoteId AND data_finalizacao IS NOT NULL AND sucesso = 0;";

                var totalItensProcessadosComErro = await connection.QueryFirstOrDefaultAsync<int>(getTotalItensProcessadosComErroLoteSql, new { LoteId = loteId });

                return totalItensProcessadosComErro;
            }
            catch (Exception)
            {
                throw;  
            }
        }

        public async Task<ItemLoteDTO> ObterItemLotePorIdAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"
                    SELECT
                        il.id AS Id,
                        il.data_criacao AS DataCriacao,
                        il.file_path AS FilePath,
                        il.data_finalizacao AS DataFinalizacao,
                        il.sucesso AS Sucesso,
                        il.retorno AS Retorno,
                        il.tb_lote_id AS LoteId
                    FROM tb_item_lote il
                    WHERE il.id = @ItemLoteId";

                return await connection.QueryFirstOrDefaultAsync<ItemLoteDTO>(sql, new { ItemLoteId = itemLoteId });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ResetarItemLoteAsync(string itemLoteId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"
                    UPDATE tb_item_lote
                    SET
                        data_finalizacao = NULL,
                        sucesso = NULL,
                        retorno = NULL
                    WHERE id = @ItemLoteId";

                await connection.ExecuteAsync(sql, new { ItemLoteId = itemLoteId });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
} 