using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorSugestaoRepository : IColaboradorSugestaoRepository
    {
        private IConnectionStringCore _connectionString;

        public ColaboradorSugestaoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<string>> ListarEmpresasRelacionadas(string nomeEmpresa, int limite, int cursor, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @$"
                                        SELECT
                                            nome,
                                            data_criacao
                                            codigo_interno_colaborador_criacao,
                                            tb_org_id
                                        FROM
                                            tb_empresa_relacionada_sugestao
                                        WHERE
                                            nome LIKE @NomeEmpresa and tb_org_id = @OrgId
                                        ORDER BY
                                            nome
                                        LIMIT
                                            @Limite
                                        OFFSET
                                            @Cursor;
                                ";

                    var result = await _connection.QueryAsync<string>(sql, new
                    {
                        NomeEmpresa = $"%{nomeEmpresa}%",
                        Limite = limite,
                        Cursor = cursor,
                        OrgId = orgId,
                    });

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<string> GetEmpresaPorNome(string nomeEmpresa, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @$"
                                    SELECT
                                        nome
                                    FROM
                                        tb_empresa_relacionada_sugestao
                                    WHERE
                                        nome = @NomeEmpresa
                                        AND tb_org_id = @OrgId;
            ";

                    var result = await _connection.QueryFirstOrDefaultAsync<string>(sql, new
                    {
                        NomeEmpresa = nomeEmpresa?.ToUpperInvariant(),
                        OrgId = orgId,
                    });

                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> AddEmpresaSugestao(string nome, string codigoInternoColaboradorCriacao, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @$"
                                    INSERT INTO
                                        tb_empresa_relacionada_sugestao (nome, codigo_interno_colaborador_criacao, tb_org_id)
                                    VALUES
                                        (@Nome, @CodigoInternoColaborador, @OrgId);
                                 ";

                    var result = await _connection.ExecuteAsync(sql, new
                    {
                        Nome = nome.ToUpperInvariant(),
                        CodigoInternoColaborador = codigoInternoColaboradorCriacao,
                        OrgId = orgId,
                    });

                    return result > 0;
                }
                catch (Exception ex)
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