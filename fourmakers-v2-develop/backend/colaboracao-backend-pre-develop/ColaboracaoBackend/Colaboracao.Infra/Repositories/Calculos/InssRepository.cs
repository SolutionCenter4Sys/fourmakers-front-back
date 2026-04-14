using Core.DomainModel.Calculos;
using DataTransferObject.Domain.Calculos;
using System;
using System.Threading.Tasks;
using Colaboracao.Core.Impl;
using Dapper;

namespace Colaboracao.Infra.Repositories.Calculos
{
    public class InssRepository : IInssRepository
    {
        public async Task<InssTabelaDTO> ObterTabelaInssPorAnoAsync(int ano)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id AS Id,
                        ano AS Ano,
                        faixa_1_min AS Faixa1Min,
                        faixa_1_max AS Faixa1Max,
                        faixa_1_aliquota AS Faixa1Aliquota,
                        COALESCE(faixa_1_parcela_deduzir, 0) AS Faixa1ParcelaDeduzir,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_aliquota AS Faixa2Aliquota,
                        COALESCE(faixa_2_parcela_deduzir, 0) AS Faixa2ParcelaDeduzir,
                        faixa_3_min AS Faixa3Min,
                        faixa_3_max AS Faixa3Max,
                        faixa_3_aliquota AS Faixa3Aliquota,
                        COALESCE(faixa_3_parcela_deduzir, 0) AS Faixa3ParcelaDeduzir,
                        faixa_4_min AS Faixa4Min,
                        faixa_4_max AS Faixa4Max,
                        faixa_4_aliquota AS Faixa4Aliquota,
                        COALESCE(faixa_4_parcela_deduzir, 0) AS Faixa4ParcelaDeduzir,
                        teto_inss AS TetoInss,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_inss 
                    WHERE ano = @Ano AND ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<InssTabelaDTO>(query, new { Ano = ano });
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<InssTabelaDTO> ObterTabelaInssVigenteAsync()
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id AS Id,
                        ano AS Ano,
                        faixa_1_min AS Faixa1Min,
                        faixa_1_max AS Faixa1Max,
                        faixa_1_aliquota AS Faixa1Aliquota,
                        COALESCE(faixa_1_parcela_deduzir, 0) AS Faixa1ParcelaDeduzir,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_aliquota AS Faixa2Aliquota,
                        COALESCE(faixa_2_parcela_deduzir, 0) AS Faixa2ParcelaDeduzir,
                        faixa_3_min AS Faixa3Min,
                        faixa_3_max AS Faixa3Max,
                        faixa_3_aliquota AS Faixa3Aliquota,
                        COALESCE(faixa_3_parcela_deduzir, 0) AS Faixa3ParcelaDeduzir,
                        faixa_4_min AS Faixa4Min,
                        faixa_4_max AS Faixa4Max,
                        faixa_4_aliquota AS Faixa4Aliquota,
                        COALESCE(faixa_4_parcela_deduzir, 0) AS Faixa4ParcelaDeduzir,
                        teto_inss AS TetoInss,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_inss 
                    WHERE ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<InssTabelaDTO>(query);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}