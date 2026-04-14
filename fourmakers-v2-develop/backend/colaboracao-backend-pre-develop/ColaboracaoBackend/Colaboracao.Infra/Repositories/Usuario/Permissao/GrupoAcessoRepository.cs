using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.Permissao;
using Dapper;
using DataTransferObject.Domain.Usuario.Permissao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Permissao
{
    public class GrupoAcessoRepository : IGrupoAcessoRepository
    {
        private IConnectionStringCore _connectionString;

        public GrupoAcessoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<GrupoAcessoDTO>> ListarGruposAcesso(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = $@"
                        SELECT
                            id AS Id,
                            descricao AS Descricao,
                            data_criacao AS DataCriacao,
                            data_alteracao AS DataAlteracao,
                            ativo AS Ativo,
                            tb_org_id AS OrgId,
                            acesso_todos_clientes AS AcessoTodosClientes
                        FROM
                            tb_grupo_acesso tga
                        WHERE
                            tga.ativo = 1
                            AND tga.tb_org_id = @OrgId;";

                    var grupoAcessoDBResult = await _connection.QueryAsync<GrupoAcessoDTO>(sql, new { OrgId = orgId });

                    return grupoAcessoDBResult;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os grupos de acesso da organização", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<GrupoAcessoFuncionalidadeSistemaResult>> ListarGruposAcessoFuncionalidadesSistema(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @"
                        SELECT
                            tga.id AS GrupoAcessoId,
                            tga.descricao AS GrupoAcessoDescricao,
                            tga.data_criacao AS GrupoAcessoDataCriacao,
                            tga.data_alteracao AS GrupoAcessoDataAlteracao,
                            tga.ativo AS GrupoAcessoAtivo,
                            tga.tb_org_id AS GrupoAcessoOrgId,
                            tga.acesso_todos_clientes AS GrupoAcessoAcessoTodosClientes,
                            tfs.id AS FuncionalidadeSistemaId,
                            tfs.descricao AS FuncionalidadeSistemaDescricao,
                            tfs.data_criacao AS FuncionalidadeSistemaDataCriacao,
                            tfs.data_alteracao AS FuncionalidadeSistemaDataAlteracao,
                            tfs.ativo AS FuncionalidadeSistemaAtivo
                        FROM
                            tb_grupo_acesso tga
                        LEFT JOIN
                            tb_grupo_acesso_funcionalidade_sistema tgafs ON tga.id = tgafs.tb_grupo_acesso_id
                        LEFT JOIN
                            tb_funcionalidade_sistema tfs ON tgafs.tb_funcionalidade_sistema_id = tfs.id
                        WHERE
                            tga.ativo = 1 and tga.tb_org_id = @OrgId;";

                    var result = await _connection.QueryAsync<GrupoAcessoFuncionalidadeSistemaResult>(sql, new { OrgId = orgId });

                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar grupos de acesso e funcionalidades do sistema", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task AdicionarGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfAlterador)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    string sql = @"
                    INSERT INTO tb_grupo_acesso_funcionalidade_sistema (tb_grupo_acesso_id, tb_funcionalidade_sistema_id, ativo)
                    VALUES (@GrupoAcessoId, @FuncionalidadeSistemaId, 1);
                    SELECT LAST_INSERT_ID();";

                    var id = await _connection.ExecuteScalarAsync<int>(sql, new { GrupoAcessoId = grupoAcessoId, FuncionalidadeSistemaId = funcionalidadeSistemaId }, transaction);

                    // Buscar o objeto criado para log
                    var objetoCriado = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso_funcionalidade_sistema WHERE id = @Id",
                        new { Id = id },
                        transaction);

                    // Inserir log
                    await InserirLogGrupoAcessoFuncionalidadeSistema(id, "CREATE", null, objetoCriado, cpfAlterador, _connection, transaction);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao associar funcionalidade ao grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task RemoverGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfAlterador)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    // Buscar objeto antes da exclusão
                    var objetoAnterior = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso_funcionalidade_sistema WHERE tb_grupo_acesso_id = @GrupoAcessoId AND tb_funcionalidade_sistema_id = @FuncionalidadeSistemaId",
                        new { GrupoAcessoId = grupoAcessoId, FuncionalidadeSistemaId = funcionalidadeSistemaId },
                        transaction);

                    if (objetoAnterior != null)
                    {
                        string sql = @"
                        DELETE FROM tb_grupo_acesso_funcionalidade_sistema
                        WHERE tb_grupo_acesso_id = @GrupoAcessoId AND tb_funcionalidade_sistema_id = @FuncionalidadeSistemaId;";

                        await _connection.ExecuteAsync(sql, new { GrupoAcessoId = grupoAcessoId, FuncionalidadeSistemaId = funcionalidadeSistemaId }, transaction);

                        // Inserir log
                        await InserirLogGrupoAcessoFuncionalidadeSistema((int)objetoAnterior.id, "DELETE", objetoAnterior, null, cpfAlterador, _connection, transaction);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao remover associação de funcionalidade do grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
        public async Task<bool> GrupoAcessoNaoPertenceOrg(int grupoAcessoId, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @" SELECT
                                        COUNT(1)
                                    FROM
                                        tb_grupo_acesso
                                    WHERE
                                        id = @GrupoAcessoId
                                        AND tb_org_id = @OrgId;";

                    var count = await _connection.ExecuteScalarAsync<int>(sql, new { GrupoAcessoId = grupoAcessoId, OrgId = orgId });

                    return count == 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao validar se Grupo Acesso pertence a Org", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> GrupoAcessoEstaRelacionadoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @" SELECT
                                        COUNT(1)
                                    FROM
                                        tb_grupo_acesso_funcionalidade_sistema
                                    WHERE
                                        tb_grupo_acesso_id = @GrupoAcessoId
                                        AND tb_funcionalidade_sistema_id = @FuncionalidadeSistemaId;";

                    var count = await _connection.ExecuteScalarAsync<int>(sql, new { GrupoAcessoId = grupoAcessoId, FuncionalidadeSistemaId = funcionalidadeSistemaId });

                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao validar se Grupo Acesso já está relacionado à Funcionalidade do Sistema", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<int> CriarGrupoAcesso(string descricao, int orgId, string cpfAlterador, bool acessoTodosClientes)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    string sql = @"
                        INSERT INTO tb_grupo_acesso (descricao, ativo, nivel, tb_org_id, acesso_todos_clientes)
                        VALUES (@Descricao, 1, 0, @OrgId, @AcessoTodosClientes);
                        SELECT LAST_INSERT_ID();";

                    var grupoAcessoId = await _connection.ExecuteScalarAsync<int>(sql, new { Descricao = descricao, OrgId = orgId, AcessoTodosClientes = acessoTodosClientes ? 1 : 0 }, transaction);

                    // Buscar o objeto criado para log
                    var objetoCriado = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso WHERE id = @Id",
                        new { Id = grupoAcessoId },
                        transaction);

                    // Inserir log
                    await InserirLogGrupoAcesso(grupoAcessoId, "CREATE", null, objetoCriado, cpfAlterador, _connection, transaction);

                    await transaction.CommitAsync();
                    return grupoAcessoId;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao criar grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task AtualizarGrupoAcesso(int grupoAcessoId, string descricao, string cpfAlterador, bool acessoTodosClientes)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    // Buscar objeto antes da alteração
                    var objetoAnterior = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso WHERE id = @Id",
                        new { Id = grupoAcessoId },
                        transaction);

                    string sql = @"
                        UPDATE tb_grupo_acesso
                        SET descricao = @Descricao,
                            acesso_todos_clientes = @AcessoTodosClientes,
                            data_alteracao = CURRENT_TIMESTAMP
                        WHERE id = @GrupoAcessoId;";

                    await _connection.ExecuteAsync(sql, new { GrupoAcessoId = grupoAcessoId, Descricao = descricao, AcessoTodosClientes = acessoTodosClientes ? 1 : 0 }, transaction);

                    // Buscar objeto após a alteração
                    var objetoAtualizado = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso WHERE id = @Id",
                        new { Id = grupoAcessoId },
                        transaction);

                    // Inserir log
                    await InserirLogGrupoAcesso(grupoAcessoId, "UPDATE", objetoAnterior, objetoAtualizado, cpfAlterador, _connection, transaction);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        private async Task InserirLogGrupoAcesso(int grupoAcessoId, string acao, dynamic objetoAnterior, dynamic objetoAtualizado, string cpfAlterador, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            var logId = Guid.NewGuid().ToString();
            var objetoJson = objetoAnterior != null ? JsonSerializer.Serialize(objetoAnterior) : "{}";
            var alteracoesJson = objetoAtualizado != null ? JsonSerializer.Serialize(objetoAtualizado) : "{}";

            var logSql = @"
                INSERT INTO tb_grupo_acesso_log 
                (id, tb_grupo_acesso_id, acao, tb_colaborador_codigo_interno_colaborador_alterador, 
                 data_alteracao, objeto, alteracoes)
                VALUES 
                (@Id, @GrupoAcessoId, @Acao, @CpfAlterador, 
                 CURRENT_TIMESTAMP, @Objeto, @Alteracoes)";

            await connection.ExecuteAsync(logSql, new
            {
                Id = logId,
                GrupoAcessoId = grupoAcessoId,
                Acao = acao,
                CpfAlterador = cpfAlterador,
                Objeto = objetoJson,
                Alteracoes = alteracoesJson
            }, transaction);
        }

        public async Task AdicionarClienteAoGrupoAcesso(int grupoAcessoId, string codigoCliente, int orgId, string cpfAlterador)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    // Verificar se já existe
                    var existente = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso_cliente WHERE tb_grupo_acesso_id = @GrupoAcessoId AND codigo_cliente = @CodigoCliente AND tb_org_id = @OrgId",
                        new { GrupoAcessoId = grupoAcessoId, CodigoCliente = codigoCliente, OrgId = orgId },
                        transaction);

                    string sql = @"
                        INSERT INTO tb_grupo_acesso_cliente (tb_grupo_acesso_id, codigo_cliente, tb_org_id, ativo)
                        VALUES (@GrupoAcessoId, @CodigoCliente, @OrgId, 1)
                        ON DUPLICATE KEY UPDATE ativo = 1;";

                    await _connection.ExecuteAsync(sql, new { GrupoAcessoId = grupoAcessoId, CodigoCliente = codigoCliente, OrgId = orgId }, transaction);

                    // Buscar o objeto após inserção/atualização
                    var objetoAtualizado = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso_cliente WHERE tb_grupo_acesso_id = @GrupoAcessoId AND codigo_cliente = @CodigoCliente AND tb_org_id = @OrgId",
                        new { GrupoAcessoId = grupoAcessoId, CodigoCliente = codigoCliente, OrgId = orgId },
                        transaction);

                    // Inserir log
                    var acao = existente != null ? "UPDATE" : "CREATE";
                    await InserirLogGrupoAcessoCliente((int)objetoAtualizado.id, acao, existente, objetoAtualizado, cpfAlterador, _connection, transaction);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao adicionar cliente ao grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task RemoverTodosClientesDoGrupoAcesso(int grupoAcessoId, int orgId, string cpfAlterador)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();
                    using var transaction = await _connection.BeginTransactionAsync();

                    // Buscar objetos antes da exclusão
                    var objetosAnteriores = await _connection.QueryAsync<dynamic>(
                        "SELECT * FROM tb_grupo_acesso_cliente WHERE tb_grupo_acesso_id = @GrupoAcessoId AND tb_org_id = @OrgId",
                        new { GrupoAcessoId = grupoAcessoId, OrgId = orgId },
                        transaction);

                    string sql = @"
                        DELETE FROM tb_grupo_acesso_cliente
                        WHERE tb_grupo_acesso_id = @GrupoAcessoId
                          AND tb_org_id = @OrgId;";

                    await _connection.ExecuteAsync(sql, new { GrupoAcessoId = grupoAcessoId, OrgId = orgId }, transaction);

                    // Inserir log para cada objeto removido
                    foreach (var objeto in objetosAnteriores)
                    {
                        await InserirLogGrupoAcessoCliente((int)objeto.id, "DELETE", objeto, null, cpfAlterador, _connection, transaction);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao remover clientes do grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        private async Task InserirLogGrupoAcessoFuncionalidadeSistema(int id, string acao, dynamic objetoAnterior, dynamic objetoAtualizado, string cpfAlterador, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            var logId = Guid.NewGuid().ToString();
            var objetoJson = objetoAnterior != null ? JsonSerializer.Serialize(objetoAnterior) : "{}";
            var alteracoesJson = objetoAtualizado != null ? JsonSerializer.Serialize(objetoAtualizado) : "{}";

            var logSql = @"
                INSERT INTO tb_grupo_acesso_funcionalidade_sistema_log 
                (id, tb_grupo_acesso_funcionalidade_sistema_id, acao, tb_colaborador_codigo_interno_colaborador_alterador, 
                 data_alteracao, objeto, alteracoes)
                VALUES 
                (@Id, @IdRegistro, @Acao, @CpfAlterador, 
                 CURRENT_TIMESTAMP, @Objeto, @Alteracoes)";

            await connection.ExecuteAsync(logSql, new
            {
                Id = logId,
                IdRegistro = id,
                Acao = acao,
                CpfAlterador = cpfAlterador,
                Objeto = objetoJson,
                Alteracoes = alteracoesJson
            }, transaction);
        }

        private async Task InserirLogGrupoAcessoCliente(int id, string acao, dynamic objetoAnterior, dynamic objetoAtualizado, string cpfAlterador, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            var logId = Guid.NewGuid().ToString();
            var objetoJson = objetoAnterior != null ? JsonSerializer.Serialize(objetoAnterior) : "{}";
            var alteracoesJson = objetoAtualizado != null ? JsonSerializer.Serialize(objetoAtualizado) : "{}";

            var logSql = @"
                INSERT INTO tb_grupo_acesso_cliente_log 
                (id, tb_grupo_acesso_cliente_id, acao, tb_colaborador_codigo_interno_colaborador_alterador, 
                 data_alteracao, objeto, alteracoes)
                VALUES 
                (@Id, @IdRegistro, @Acao, @CpfAlterador, 
                 CURRENT_TIMESTAMP, @Objeto, @Alteracoes)";

            await connection.ExecuteAsync(logSql, new
            {
                Id = logId,
                IdRegistro = id,
                Acao = acao,
                CpfAlterador = cpfAlterador,
                Objeto = objetoJson,
                Alteracoes = alteracoesJson
            }, transaction);
        }

        public async Task<bool> DescricaoGrupoAcessoJaExiste(string descricao, int orgId, int? grupoAcessoIdExcluir = null)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @"
                        SELECT COUNT(1)
                        FROM tb_grupo_acesso
                        WHERE descricao = @Descricao
                          AND tb_org_id = @OrgId";

                    if (grupoAcessoIdExcluir.HasValue)
                    {
                        sql += " AND id != @GrupoAcessoIdExcluir";
                    }

                    sql += ";";

                    var parametros = new { Descricao = descricao, OrgId = orgId, GrupoAcessoIdExcluir = grupoAcessoIdExcluir };
                    var count = await _connection.ExecuteScalarAsync<int>(sql, parametros);

                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao validar se descrição do grupo de acesso já existe", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> ClienteExiste(string codigoCliente, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @"
                        SELECT COUNT(1)
                        FROM tb_cliente_org
                        WHERE codigo_cliente = @CodigoCliente
                          AND tb_org_id = @OrgId
                          AND ativo = 1;";

                    var count = await _connection.ExecuteScalarAsync<int>(sql, new { CodigoCliente = codigoCliente, OrgId = orgId });

                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao validar se cliente existe", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<List<string>> ListarTodosClientesAtivosPorOrg(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @"
                        SELECT codigo_cliente
                        FROM tb_cliente_org
                        WHERE tb_org_id = @OrgId
                          AND ativo = 1
                        ORDER BY codigo_cliente;";

                    var codigosClientes = await _connection.QueryAsync<string>(sql, new { OrgId = orgId });

                    return codigosClientes.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar clientes da organização", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<PessoaGrupoAcessoDTO>> ListarPessoasPorGrupoAcesso(int orgId, int? grupoId = null)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @"
                        SELECT DISTINCT
                            tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                            tc.nome_completo AS Nome,
                            tu.email AS Email,
                            tu.id AS UsuarioId,
                            tuga.data_criacao AS DataInsercaoGrupo,
                            tga.id AS GrupoAcessoId,
                            tga.descricao AS GrupoAcessoDescricao,
                            tga.ativo AS GrupoAcessoAtivo,
                            tga.tb_org_id AS GrupoAcessoOrgId,
                            tga.data_criacao AS GrupoAcessoDataCriacao,
                            tga.data_alteracao AS GrupoAcessoDataAlteracao,
                            tga.acesso_todos_clientes AS GrupoAcessoAcessoTodosClientes
                        FROM
                            tb_colaborador tc
                        INNER JOIN
                            tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        INNER JOIN
                            tb_usuario_grupo_acesso tuga ON tuga.tb_usuario_id = tu.id
                        INNER JOIN
                            tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                        WHERE
                            tuga.ativo = 1
                            AND tga.ativo = 1
                            AND tga.tb_org_id = @OrgId
                            AND tu.tb_org_id = @OrgId";

                    if (grupoId.HasValue)
                    {
                        sql += " AND tga.id = @GrupoId";
                    }

                    sql += @"
                        ORDER BY
                            tc.nome_completo, tga.descricao;";

                    var resultado = await _connection.QueryAsync<dynamic>(sql, new { OrgId = orgId, GrupoId = grupoId });

                    var pessoas = resultado
                        .GroupBy(r => new { 
                            CodigoInternoColaborador = (string)r.CodigoInternoColaborador, 
                            Nome = (string)r.Nome,
                            Email = (string)r.Email,
                            UsuarioId = (long)r.UsuarioId,
                            DataInsercaoGrupo = r.DataInsercaoGrupo != null ? (DateTime?)r.DataInsercaoGrupo : null
                        })
                        .Select(g => new PessoaGrupoAcessoDTO
                        {
                            CodigoInternoColaborador = g.Key.CodigoInternoColaborador,
                            Nome = g.Key.Nome,
                            Email = g.Key.Email,
                            UsuarioId = g.Key.UsuarioId,
                            DataInsercaoGrupo = g.Key.DataInsercaoGrupo,
                            GruposAcesso = g
                                .Select(r => new GrupoAcessoDTO
                                {
                                    Id = (int)r.GrupoAcessoId,
                                    Descricao = (string)r.GrupoAcessoDescricao,
                                    Ativo = Convert.ToBoolean(r.GrupoAcessoAtivo),
                                    OrgId = (int)r.GrupoAcessoOrgId,
                                    DataCriacao = r.GrupoAcessoDataCriacao != null ? (DateTime?)r.GrupoAcessoDataCriacao : null,
                                    DataAlteracao = r.GrupoAcessoDataAlteracao != null ? (DateTime?)r.GrupoAcessoDataAlteracao : null,
                                    AcessoTodosClientes = Convert.ToBoolean(r.GrupoAcessoAcessoTodosClientes)
                                })
                                .GroupBy(ga => ga.Id)
                                .Select(gag => gag.First())
                                .OrderBy(ga => ga.Descricao)
                                .ToList()
                        })
                        .ToList();

                    return pessoas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar pessoas por grupo de acesso", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}