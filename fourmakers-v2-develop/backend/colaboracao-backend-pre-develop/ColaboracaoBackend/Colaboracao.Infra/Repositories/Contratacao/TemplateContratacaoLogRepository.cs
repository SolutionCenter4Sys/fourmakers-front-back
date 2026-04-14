using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Contratacao;
using Dapper;
using DataTransferObject.Domain.Contratacao;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Contratacao
{
    public class TemplateContratacaoLogRepository : ITemplateContratacaoLogRepository
    {
        private readonly IDBConnection _dapperConnection;

        public TemplateContratacaoLogRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<string> InserirLogTemplateAsync(
            Guid templateId, 
            string colaboradorCodigoInterno, 
            AcaoLogTemplateEnum acao, 
            string objetoTemplate = null, 
            string alteracoes = null)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var id = Guid.NewGuid().ToString();
                var query = @"
                    INSERT INTO tb_template_contratacao_log (
                        id,
                        tb_template_contratacao_id,
                        acao,
                        tb_colaborador_codigo_interno_colaborador_alterador,
                        data_alteracao,
                        objeto,
                        alteracoes
                    ) VALUES (
                        @Id,
                        @TemplateId,
                        @Acao,
                        @ColaboradorCodigoInterno,
                        NOW(),
                        @ObjetoTemplate,
                        @Alteracoes
                    )";

                var parametros = new
                {
                    Id = id,
                    TemplateId = templateId.ToString(),
                    Acao = acao.ToString(),
                    ColaboradorCodigoInterno = colaboradorCodigoInterno,
                    ObjetoTemplate = objetoTemplate,
                    Alteracoes = alteracoes
                };

                await connection.ExecuteAsync(query, parametros);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsTemplateAsync(Guid templateId, int limite = 50, int cursor = 0)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_template_contratacao_id as TbTemplateContratacaoId,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as TbColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_template_contratacao_log 
                    WHERE tb_template_contratacao_id = @TemplateId
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var parametros = new
                {
                    TemplateId = templateId.ToString(),
                    Limite = limite,
                    Cursor = cursor
                };

                var logs = await connection.QueryAsync<TemplateContratacaoLogDTO>(query, parametros);
                return logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsPorColaboradorAsync(string colaboradorCodigoInterno, int limite = 50, int cursor = 0)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_template_contratacao_id as TbTemplateContratacaoId,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as TbColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_template_contratacao_log 
                    WHERE tb_colaborador_codigo_interno_colaborador_alterador = @ColaboradorCodigoInterno
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var parametros = new
                {
                    ColaboradorCodigoInterno = colaboradorCodigoInterno,
                    Limite = limite,
                    Cursor = cursor
                };

                var logs = await connection.QueryAsync<TemplateContratacaoLogDTO>(query, parametros);
                return logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TemplateContratacaoLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite = 100, int cursor = 0)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_template_contratacao_id as TbTemplateContratacaoId,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as TbColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_template_contratacao_log 
                    WHERE data_alteracao BETWEEN @DataInicio AND @DataFim
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var parametros = new
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Limite = limite,
                    Cursor = cursor
                };

                var logs = await connection.QueryAsync<TemplateContratacaoLogDTO>(query, parametros);
                return logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
