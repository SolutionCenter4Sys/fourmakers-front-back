using Colaboracao.Core.Interfaces;
using Core.Domain.Apontamento;
using Dapper;
using DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo;
using System;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Apontamento
{
    public class PeriodoFechadoRepository : IPeriodoFechadoRepository
    {
        private IConnectionStringCore _connectionString;

        public PeriodoFechadoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public FecharAlterarPeriodoDTO CriarDataPeriodoFechado(DateTime dataFim, int orgId, string cpf)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"INSERT INTO
                                        tb_apontamento_periodo_fechado
                                        (
                                            `data_fim`,
                                            `tb_org_id`,
                                            `codigo_interno_colaborador_criacao`,
                                            `codigo_interno_colaborador_alteracao`
                                        )
                                   VALUES
                                   (
                                        @DataFim,
                                        @OrgId,
                                        @Cpf,
                                        @Cpf
                                   );";

                    string sqlLog = @"INSERT INTO
                                        tb_apontamento_periodo_fechado_log
                                        (
                                            tb_apontamento_periodo_fechado_id,
                                            data_fim_nova,
                                            tb_org_id,
                                            codigo_interno_colaborador_criacao,
                                            codigo_interno_colaborador_alteracao
                                        )
                                        SELECT
                                            id,
                                            data_fim,
                                            tb_org_id,
                                            codigo_interno_colaborador_criacao,
                                            codigo_interno_colaborador_alteracao
                                        FROM
                                            tb_apontamento_periodo_fechado
                                        WHERE
                                            id = LAST_INSERT_ID();";

                    string sqlConcatenado = sql + Environment.NewLine + sqlLog;

                    var parametros = new DynamicParameters();
                    parametros.Add("@DataFim", dataFim);
                    parametros.Add("@OrgId", orgId);
                    parametros.Add("@Cpf", cpf);

                    var novoPeriodo = _connection.Execute(
                        sqlConcatenado,
                        parametros);

                    return new FecharAlterarPeriodoDTO
                    {
                        DataFim = dataFim,
                        DataCriacao = DateTime.Now
                    };
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public FecharAlterarPeriodoDTO AlterarDataPeriodoFechado(DateTime dataFim, int orgId, string cpf, int? id)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"UPDATE
                                        tb_apontamento_periodo_fechado
                                   SET
                                        `data_fim` = @DataFimNova,
                                        `codigo_interno_colaborador_alteracao` = @Cpf
                                   WHERE
                                        (`id` = @Id)
                                   AND
                                        tb_org_id = @OrgId;";

                    string sqlLog = @"INSERT INTO
                                        tb_apontamento_periodo_fechado_log
                                        (
                                            tb_apontamento_periodo_fechado_id,
                                            data_fim_anterior,
                                            data_fim_nova,
                                            tb_org_id,
                                            codigo_interno_colaborador_criacao,
                                            codigo_interno_colaborador_alteracao
                                        )
                                        SELECT
                                            id,
                                            data_fim,
                                            @DataFimNova,
                                            tb_org_id,
                                            codigo_interno_colaborador_criacao,
                                            codigo_interno_colaborador_alteracao
                                        FROM
                                            tb_apontamento_periodo_fechado
                                        WHERE
                                            id = @Id;";

                    string sqlConcatenado = sqlLog + Environment.NewLine + sql;

                    var parametros = new DynamicParameters();
                    parametros.Add("@DataFimNova", dataFim);
                    parametros.Add("@OrgId", orgId);
                    parametros.Add("@Cpf", cpf);
                    parametros.Add("@Id", id);

                    var novoPeriodo = _connection.Execute(
                        sqlConcatenado,
                        parametros);

                    return new FecharAlterarPeriodoDTO
                    {
                        DataFim = dataFim,
                        DataAlteracao = DateTime.Now
                    };
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public int ExistePeriodo(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"SELECT
                                        `id`
                                   FROM
                                        tb_apontamento_periodo_fechado
                                   WHERE
                                        tb_org_id = @OrgId;";

                    var parametros = new DynamicParameters();
                    parametros.Add("@OrgId", orgId);

                    var buscaId = _connection.QueryFirstOrDefault(
                        sql,
                        parametros);

                    return buscaId?.id ?? 0;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<BuscaPeriodoFechadoResult> BuscaPeriodoFechado(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string sql = @"SELECT
                                        *
                                   FROM
                                        tb_apontamento_periodo_fechado
                                   WHERE
                                        tb_org_id = @OrgId;";

                    var parametros = new DynamicParameters();
                    parametros.Add("@OrgId", orgId);

                    var buscaPeriodo = await _connection.QueryFirstOrDefaultAsync(sql, parametros);

                    return new BuscaPeriodoFechadoResult
                    {
                        DataFim = buscaPeriodo?.data_fim ?? null,
                        Id = buscaPeriodo?.id ?? null
                    };
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}