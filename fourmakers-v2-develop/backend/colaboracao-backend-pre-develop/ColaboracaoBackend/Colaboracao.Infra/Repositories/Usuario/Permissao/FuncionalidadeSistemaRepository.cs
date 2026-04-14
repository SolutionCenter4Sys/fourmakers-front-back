using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.Permissao;
using Dapper;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario.Permissao
{
    public class FuncionalidadeSistemaRepository : IFuncionalidadeSistemaRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public FuncionalidadeSistemaRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<FuncionalidadeSistemaDTO>> GetFuncionalidadeSistemaPorUsuario(int usuarioId, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"
                        SELECT DISTINCT
                            tfs.id AS Id,
                            tfs.descricao AS Descricao,
                            tfs.data_criacao AS DataCriacao,
                            tfs.data_alteracao AS DataAlteracao,
                            tfs.ativo AS Ativo
                        FROM
                            tb_usuario_grupo_acesso tuga
                        JOIN
                            tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                        JOIN
                            tb_grupo_acesso_funcionalidade_sistema tgafs ON tga.id = tgafs.tb_grupo_acesso_id
                        JOIN
                            tb_funcionalidade_sistema tfs ON tgafs.tb_funcionalidade_sistema_id = tfs.id
                        WHERE
                            tuga.tb_usuario_id = @UsuarioId AND tga.tb_org_id = @OrgId";

                    var result = await _connection.QueryAsync<FuncionalidadeSistemaDTO>(sql, new { UsuarioId = usuarioId, OrgId = orgId });

                    return result.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> ValidaAcessoFuncionalidade(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidade)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sqlAcessoValido = @"
                            SELECT
                                COUNT(1)
                            FROM
                                tb_usuario_grupo_acesso tuga
                            JOIN
                                tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                            JOIN
                                tb_grupo_acesso_funcionalidade_sistema tgafs ON tga.id = tgafs.tb_grupo_acesso_id
                            WHERE
                                tuga.tb_usuario_id = (SELECT tu.id FROM tb_usuario tu WHERE tu.codigo_interno_colaborador = @Cpf and tu.tb_org_id = @OrgId)
                                AND tga.tb_org_id = @OrgId AND tuga.ativo = 1 AND tga.nivel = 0
                                AND tgafs.tb_funcionalidade_sistema_id = @Funcionalidade";

                    var acessoValido = await _connection.ExecuteScalarAsync<bool>(sqlAcessoValido, new
                    {
                        OrgId = orgId,
                        Funcionalidade = (int)funcionalidade,
                        Cpf = cpf
                    });

                    return acessoValido;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<FuncionalidadeSistemaDTO>> ListarFuncionalidadesSistema()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = $@"
                        SELECT
                            tfs.id AS Id,
                            tfs.descricao AS Descricao,
                            tfs.data_criacao AS DataCriacao,
                            tfs.data_alteracao AS DataAlteracao,
                            tfs.ativo AS Ativo
                        FROM
                            tb_funcionalidade_sistema tfs
                        WHERE
                            tfs.ativo = 1;";

                    var funcionalidadesDBResult = await _connection.QueryAsync<FuncionalidadeSistemaDTO>(sql);

                    return funcionalidadesDBResult;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar as funcionalidades do sistema", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<IEnumerable<PessoaFuncionalidadeSistemaDTO>> ListarPessoasPorFuncionalidadeSistema(int orgId, int? funcionalidadeId = null)
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
                            tfs.id AS FuncionalidadeSistemaId,
                            tfs.descricao AS FuncionalidadeSistemaDescricao,
                            tfs.ativo AS FuncionalidadeSistemaAtivo,
                            tfs.data_criacao AS FuncionalidadeSistemaDataCriacao,
                            tfs.data_alteracao AS FuncionalidadeSistemaDataAlteracao
                        FROM
                            tb_colaborador tc
                        INNER JOIN
                            tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        INNER JOIN
                            tb_usuario_grupo_acesso tuga ON tuga.tb_usuario_id = tu.id
                        INNER JOIN
                            tb_grupo_acesso tga ON tuga.tb_grupo_acesso_id = tga.id
                        INNER JOIN
                            tb_grupo_acesso_funcionalidade_sistema tgafs ON tga.id = tgafs.tb_grupo_acesso_id
                        INNER JOIN
                            tb_funcionalidade_sistema tfs ON tgafs.tb_funcionalidade_sistema_id = tfs.id
                        WHERE
                            tuga.ativo = 1
                            AND tga.ativo = 1
                            AND tgafs.ativo = 1
                            AND tfs.ativo = 1
                            AND tga.tb_org_id = @OrgId
                            AND tu.tb_org_id = @OrgId";

                    if (funcionalidadeId.HasValue)
                    {
                        sql += " AND tfs.id = @FuncionalidadeId";
                    }

                    sql += @"
                        ORDER BY
                            tc.nome_completo, tfs.descricao;";

                    var resultado = await _connection.QueryAsync<dynamic>(sql, new { OrgId = orgId, FuncionalidadeId = funcionalidadeId });

                    var pessoas = resultado
                        .GroupBy(r => new { 
                            CodigoInternoColaborador = (string)r.CodigoInternoColaborador, 
                            Nome = (string)r.Nome,
                            Email = (string)r.Email,
                            UsuarioId = (long)r.UsuarioId,
                            DataInsercaoGrupo = r.DataInsercaoGrupo != null ? (DateTime?)r.DataInsercaoGrupo : null
                        })
                        .Select(g => new PessoaFuncionalidadeSistemaDTO
                        {
                            CodigoInternoColaborador = g.Key.CodigoInternoColaborador,
                            Nome = g.Key.Nome,
                            Email = g.Key.Email,
                            UsuarioId = g.Key.UsuarioId,
                            DataInsercaoGrupo = g.Key.DataInsercaoGrupo,
                            FuncionalidadesSistema = g
                                .Select(r => new FuncionalidadeSistemaDTO
                                {
                                    Id = (int)r.FuncionalidadeSistemaId,
                                    Descricao = (string)r.FuncionalidadeSistemaDescricao,
                                    Ativo = Convert.ToBoolean(r.FuncionalidadeSistemaAtivo),
                                    DataCriacao = r.FuncionalidadeSistemaDataCriacao != null ? (DateTime?)r.FuncionalidadeSistemaDataCriacao : null,
                                    DataAlteracao = r.FuncionalidadeSistemaDataAlteracao != null ? (DateTime?)r.FuncionalidadeSistemaDataAlteracao : null
                                })
                                .GroupBy(f => f.Id)
                                .Select(fg => fg.First())
                                .OrderBy(f => f.Descricao)
                                .ToList()
                        })
                        .ToList();

                    return pessoas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar pessoas por funcionalidade do sistema", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> FuncionalidadeNaoExiste(int funcionalidadeSistemaId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    await _connection.OpenAsync();

                    string sql = @" SELECT
                                        COUNT(1)
                                    FROM
                                        tb_funcionalidade_sistema
                                    WHERE
                                        id = @FuncionalidadeSistemaId;";

                    var count = await _connection.ExecuteScalarAsync<int>(sql, new { FuncionalidadeSistemaId = funcionalidadeSistemaId });

                    return count == 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao verificar se a funcionalidade do sistema existe", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}