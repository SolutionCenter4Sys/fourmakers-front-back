using Core.DomainModel.Calculos;
using DataTransferObject.Domain.Calculos;
using System;
using System.Threading.Tasks;
using Colaboracao.Core.Impl;
using Dapper;

namespace Colaboracao.Infra.Repositories.Calculos
{
    public class IrrfReducaoRepository : IIrrfReducaoRepository
    {
        public async Task<IrrfReducaoDTO> ObterReducaoPorAnoAsync(int ano)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id AS Id,
                        ano AS Ano,
                        faixa_1_max AS Faixa1Max,
                        faixa_1_desconto_maximo AS Faixa1DescontoMaximo,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_valor_base AS Faixa2ValorBase,
                        faixa_2_coeficiente AS Faixa2Coeficiente,
                        faixa_3_min AS Faixa3Min,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_irrf_reducao 
                    WHERE ano = @Ano AND ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<IrrfReducaoDTO>(query, new { Ano = ano });
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

        public async Task<IrrfReducaoDTO> ObterReducaoVigenteAsync()
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id AS Id,
                        ano AS Ano,
                        faixa_1_max AS Faixa1Max,
                        faixa_1_desconto_maximo AS Faixa1DescontoMaximo,
                        faixa_2_min AS Faixa2Min,
                        faixa_2_max AS Faixa2Max,
                        faixa_2_valor_base AS Faixa2ValorBase,
                        faixa_2_coeficiente AS Faixa2Coeficiente,
                        faixa_3_min AS Faixa3Min,
                        data_criacao AS DataCriacao,
                        data_alteracao AS DataAlteracao,
                        ativo AS Ativo
                    FROM tb_irrf_reducao 
                    WHERE ativo = 1
                    ORDER BY ano DESC
                    LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<IrrfReducaoDTO>(query);
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

