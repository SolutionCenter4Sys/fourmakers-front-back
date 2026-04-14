using Colaboracao.Core.Interfaces;
using Core.Domain.ParametroOrg;
using Dapper;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.Parametro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Fourmakers
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public ParametroRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        private string SELECT_DEFAULT => @"
            SELECT
                id AS Id,
                nome_parametro AS NomeParametro,
                descricao_parametro AS DescricaoParametro,
                codigo_parametro AS CodigoParametro,
                codigo_modulo_sistema AS CodigoModuloSistema,
                data_criacao AS DataCriacao,
                data_alteracao AS DataAlteracao,
                ativo AS Ativo,
                tipo_parametro AS TipoParametro,
                tb_usuario_id_criacao AS UsuarioIdCriacao,
                tb_usuario_id_alteracao AS UsuarioIdAlteracao
            FROM
                tb_parametro
            WHERE
                tipo_parametro = @TipoParametro";

        public async Task<IEnumerable<ParametroResult>> ListarParametros(TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = SELECT_DEFAULT;
                    var result = await _connection.QueryAsync<ParametroResult>(query, new { TipoParametro = tipoParametro.ToString() });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar os parâmetros.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroResult> ObterParametroPorId(string id, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = SELECT_DEFAULT + " AND id = @Id";
                    var result = await _connection.QueryFirstOrDefaultAsync<ParametroResult>(query, new { Id = id, TipoParametro = tipoParametro.ToString() });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter o parâmetro pelo ID.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroResult> ObterParametroPorCodigo(string codigoParametro, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = SELECT_DEFAULT + " AND codigo_parametro = @CodigoParametro";
                    var result = await _connection.QueryFirstOrDefaultAsync<ParametroResult>(query, new { CodigoParametro = codigoParametro, TipoParametro = tipoParametro.ToString() });
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter o parâmetro pelo ID.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroResult> InserirParametro(ParametroRepositoryInput parametroRepositoryInput, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"
                        INSERT INTO tb_parametro
                        (id, nome_parametro, descricao_parametro, codigo_parametro, codigo_modulo_sistema, ativo, tipo_parametro, tb_usuario_id_criacao, tb_usuario_id_alteracao)
                        VALUES
                        (@Id, @NomeParametro, @DescricaoParametro, @CodigoParametro, @CodigoModuloSistema, @Ativo, @TipoParametro, @UsuarioIdCriacao, @UsuarioIdAlteracao)";

                    var parameters = new
                    {
                        Id = parametroRepositoryInput.Id,
                        NomeParametro = parametroRepositoryInput.NomeParametro,
                        DescricaoParametro = parametroRepositoryInput.DescricaoParametro,
                        CodigoParametro = parametroRepositoryInput.CodigoParametro,
                        CodigoModuloSistema = parametroRepositoryInput.CodigoModuloSistema,
                        Ativo = parametroRepositoryInput.Ativo,
                        TipoParametro = tipoParametro.ToString(),
                        UsuarioIdCriacao = parametroRepositoryInput.UsuarioIdAlteracao,
                        UsuarioIdAlteracao = parametroRepositoryInput.UsuarioIdAlteracao
                    };

                    int rowsAffected = await _connection.ExecuteAsync(query, parameters);
                    if (rowsAffected > 0)
                    {
                        return await ObterParametroPorId(parametroRepositoryInput.Id.ToString(), tipoParametro);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir o parâmetro.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ParametroResult> AtualizarParametro(ParametroRepositoryInput parametroRepositoryInput, TipoParametroEnum tipoParametro)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"
                        UPDATE tb_parametro
                        SET
                            nome_parametro = @NomeParametro,
                            descricao_parametro = @DescricaoParametro,
                            codigo_parametro = @CodigoParametro,
                            codigo_modulo_sistema = @CodigoModuloSistema
                        WHERE
                            id = @Id AND tipo_parametro = @TipoParametro";

                    var parameters = new
                    {
                        Id = parametroRepositoryInput.Id,
                        NomeParametro = parametroRepositoryInput.NomeParametro,
                        DescricaoParametro = parametroRepositoryInput.DescricaoParametro,
                        CodigoParametro = parametroRepositoryInput.CodigoParametro,
                        CodigoModuloSistema = parametroRepositoryInput.CodigoModuloSistema,
                        UsuarioIdAlteracao = parametroRepositoryInput.UsuarioIdAlteracao,
                        TipoParametro = tipoParametro.ToString()
                    };

                    int rowsAffected = await _connection.ExecuteAsync(query, parameters);
                    if (rowsAffected > 0)
                    {
                        return await ObterParametroPorId(parametroRepositoryInput.Id.ToString(), tipoParametro);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar o parâmetro.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> DeletarParametro(string id)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"
                        DELETE FROM tb_parametro
                        WHERE id = @Id";
                    int rowsAffected = await _connection.ExecuteAsync(query, new { Id = id });
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao deletar o parâmetro.", ex);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}