using Colaboracao.Core.Interfaces;
using Core.Domain.ParametroOrg;
using Dapper;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Fourmakers
{
    public class ParametroConfiguracaoRepository : IParametroConfiguracaoRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public ParametroConfiguracaoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        private string SELECT_DEFAULT => @"
                                            SELECT
                                                tpc.id AS Id,
                                                tpc.tb_org_id AS OrgId,
                                                tpc.codigo_interno_colaborador AS ColaboradorOrgCpf,
                                                tpc.tb_grupo_acesso_id AS GrupoAcessoId,
                                                tpc.codigo_parametro AS CodigoParametro,
                                                tpc.valor_parametro AS ValorParametro,
                                                tpc.tb_parametro_nivel_id AS ParametroNivelId,
                                                tpc.data_criacao AS DataCriacao,
                                                tpc.data_alteracao AS DataAlteracao,
                                                tpn.prioridade AS Prioridade
                                            FROM
                                                tb_parametro_configuracao tpc
                                            LEFT JOIN
                                                tb_parametro tp ON tp.codigo_parametro = tpc.codigo_parametro
                                            LEFT JOIN
                                                tb_parametro_nivel tpn ON tpn.id = tpc.tb_parametro_nivel_id
                                            WHERE
                                                tp.tipo_parametro = @TipoParametro
                                                AND (COALESCE(@OrgId,0) = 0 OR tpc.tb_org_id = @OrgId)";

        public string GetParametroConfiguracao(string codigo, string codigoInternoColaborador, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        SELECT
                            valor_parametro
                        FROM
                            tb_parametro_configuracao tpc
                        JOIN
                        	tb_parametro_nivel tpn on tpc.tb_parametro_nivel_id = tpn.id
                        WHERE
                            tpc.codigo_parametro = @Codigo
                            AND tpc.tb_org_id = @OrgId
                            AND (tpc.codigo_interno_colaborador IS NULL OR tpc.codigo_interno_colaborador = @Cpf)
                               AND (tpc.tb_grupo_acesso_id IS NULL
                                   OR EXISTS (
                                       SELECT 1
                                       FROM tb_usuario tu
                                       JOIN tb_usuario_grupo_acesso tga ON tga.tb_usuario_id = tu.id
                                       WHERE
                                           tu.codigo_interno_colaborador = @Cpf
                                           AND tga.tb_grupo_acesso_id = tpc.tb_grupo_acesso_id
                                   )
                               )
                        ORDER BY
                            tpn.prioridade
                        LIMIT 1";

                    var result = _connection.QueryFirstOrDefault<string>(query, new { Codigo = codigo, OrgId = orgId, Cpf = codigoInternoColaborador });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter o parâmetro de configuração.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoDoUsuarioLogado(string cpfRequest, int orgId, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string query = SELECT_DEFAULT;

                    query += @"AND (tpc.codigo_interno_colaborador IS NULL OR tpc.codigo_interno_colaborador = @Cpf)
                               AND (tpc.tb_grupo_acesso_id IS NULL
                                   OR EXISTS (
                                       SELECT 1
                                       FROM tb_usuario tu
                                       JOIN tb_usuario_grupo_acesso tga ON tga.tb_usuario_id = tu.id
                                       WHERE
                                           tu.codigo_interno_colaborador = @Cpf
                                           AND tga.tb_grupo_acesso_id = tpc.tb_grupo_acesso_id
                                   )
                               )";

                    var allResults = await _connection.QueryAsync<ParametroConfiguracaoResult>(query, new { Cpf = cpfRequest, OrgId = orgId, TipoParametro = tipoParametro.ToString() });

                    // filtrar pela maior prioridade (menor número)
                    var filteredResults = allResults
                        .GroupBy(r => new { r.CodigoParametro, r.OrgId })
                        .Select(g => g.OrderBy(r => r.Prioridade).First())
                        .ToList();

                    return filteredResults;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os parâmetros de configuração do usuário logado.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoPorOrg(int orgId, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string query = SELECT_DEFAULT;

                    var allResults = await _connection.QueryAsync<ParametroConfiguracaoResult>(query, new { OrgId = orgId, TipoParametro = tipoParametro.ToString() });

                    return allResults;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os parâmetros de configuração do usuário logado.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<ParametroConfiguracaoResult>> ListarParametroConfiguracaoPorCodigoParametro(string codigoParametro, int orgId, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string query = SELECT_DEFAULT + " AND tpc.codigo_parametro = @CodigoParametro";

                    var result = await _connection.QueryAsync<ParametroConfiguracaoResult>(query, new { CodigoParametro = codigoParametro, OrgId = orgId, TipoParametro = tipoParametro.ToString() });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter o parâmetro de configuração pelo Codigo.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroConfiguracaoResult> ObterParametroConfiguracaoPorId(string id, TipoParametroEnum tipoParametro, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string query = SELECT_DEFAULT + " AND tpc.id = @Id";

                    var result = await _connection.QueryFirstOrDefaultAsync<ParametroConfiguracaoResult>(query, new { Id = id, OrgId = orgId, TipoParametro = tipoParametro.ToString() });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter o parâmetro de configuração pelo ID.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroConfiguracaoResult> InserirParametroConfiguracao(ParametroConfiguracaoRepositoryInput input, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        INSERT INTO tb_parametro_configuracao
                        (id, tb_org_id, codigo_interno_colaborador, tb_grupo_acesso_id, codigo_parametro, valor_parametro, tb_parametro_nivel_id, tb_usuario_id_criacao, tb_usuario_id_alteracao)
                        VALUES
                        (@Id, @OrgId, @ColaboradorOrgCpf, @GrupoAcessoId, @CodigoParametro, @ValorParametro, @ParametroNivelId, @UsuarioIdAlteracao, @UsuarioIdAlteracao)";

                    int rowsAffected = await _connection.ExecuteAsync(query, input);

                    ParametroConfiguracaoResult result = null;

                    if (rowsAffected > 0)
                    {
                        result = await ObterParametroConfiguracaoPorId(input.Id.ToString(), tipoParametro, input.OrgId);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir o parâmetro de configuração.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroConfiguracaoResult> AtualizarParametroConfiguracao(ParametroConfiguracaoRepositoryInput input, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"
                        UPDATE
                            tb_parametro_configuracao
                        SET
                            codigo_interno_colaborador = @ColaboradorOrgCpf,
                            tb_grupo_acesso_id = @GrupoAcessoId,
                            codigo_parametro = @CodigoParametro,
                            valor_parametro = @ValorParametro,
                            tb_parametro_nivel_id = @ParametroNivelId,
                            tb_usuario_id_alteracao = @UsuarioIdAlteracao
                        WHERE
                            id = @Id AND tb_org_id = @OrgId";
                    int rowsAffected = await _connection.ExecuteAsync(query, input);

                    ParametroConfiguracaoResult result = null;

                    if (rowsAffected > 0)
                    {
                        result = await ObterParametroConfiguracaoPorId(input.Id.ToString(), tipoParametro, input.OrgId);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar o parâmetro de configuração.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> DeletarParametroConfiguracao(string id, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = @"DELETE FROM
                                          tb_parametro_configuracao
                                     WHERE
                                          id = @Id AND tb_org_id = @OrgId";
                    int rowsAffected = await _connection.ExecuteAsync(query, new { Id = id, OrgId = orgId });
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao deletar o parâmetro de configuração.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}