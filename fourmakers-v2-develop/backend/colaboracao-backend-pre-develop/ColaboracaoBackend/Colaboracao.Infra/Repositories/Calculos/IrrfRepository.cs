using Core.DomainModel.Calculos;
using DataTransferObject.Domain.Calculos;
using System;
using System.Threading.Tasks;
using Colaboracao.Core.Impl;
using Dapper;

namespace Colaboracao.Infra.Repositories.Calculos
{
    public class IrrfRepository : IIrrfRepository
    {
        public async Task<IrrfTabelaDTO> ObterTabelaIrrfPorAnoAsync(int ano)
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
                        faixa_1_deducao AS Faixa1Deducao,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_aliquota AS Faixa2Aliquota,
                        faixa_2_deducao AS Faixa2Deducao,
                        faixa_3_min AS Faixa3Min,
                        faixa_3_max AS Faixa3Max,
                        faixa_3_aliquota AS Faixa3Aliquota,
                        faixa_3_deducao AS Faixa3Deducao,
                        faixa_4_min AS Faixa4Min,
                        faixa_4_max AS Faixa4Max,
                        faixa_4_aliquota AS Faixa4Aliquota,
                        faixa_4_deducao AS Faixa4Deducao,
                        faixa_5_min AS Faixa5Min,
                        faixa_5_max AS Faixa5Max,
                        faixa_5_aliquota AS Faixa5Aliquota,
                        faixa_5_deducao AS Faixa5Deducao,
                        deducao_por_dependente AS DeducaoPorDependente,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_irrf 
                    WHERE ano = @Ano AND ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<IrrfTabelaDTO>(query, new { Ano = ano });
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

        public async Task<IrrfTabelaDTO> ObterTabelaIrrfVigenteAsync()
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
                        faixa_1_deducao AS Faixa1Deducao,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_aliquota AS Faixa2Aliquota,
                        faixa_2_deducao AS Faixa2Deducao,
                        faixa_3_min AS Faixa3Min,
                        faixa_3_max AS Faixa3Max,
                        faixa_3_aliquota AS Faixa3Aliquota,
                        faixa_3_deducao AS Faixa3Deducao,
                        faixa_4_min AS Faixa4Min,
                        faixa_4_max AS Faixa4Max,
                        faixa_4_aliquota AS Faixa4Aliquota,
                        faixa_4_deducao AS Faixa4Deducao,
                        faixa_5_min AS Faixa5Min,
                        faixa_5_max AS Faixa5Max,
                        faixa_5_aliquota AS Faixa5Aliquota,
                        faixa_5_deducao AS Faixa5Deducao,
                        deducao_por_dependente AS DeducaoPorDependente,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_irrf 
                    WHERE ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<IrrfTabelaDTO>(query);
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