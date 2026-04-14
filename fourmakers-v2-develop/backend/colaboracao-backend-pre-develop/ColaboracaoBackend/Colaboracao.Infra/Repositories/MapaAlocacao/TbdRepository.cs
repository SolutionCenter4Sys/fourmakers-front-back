using ApiClient.Domain;
using Colaboracao.Helper;
using Core.Domain.MapaAlocacao;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Tbd;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public class TbdRepository : ITbdRepository
    {
        private readonly string _connectionString;
        private string SelectPadrao
        {
            get => $@"SELECT
                       tta.cod_tbd_alocado,
                       tta.descricao,
                       tta.codigo_interno_colaborador_gestor,
                       tta.cod_diretoria,
                       tta.diretoria,
                       tta.cod_departamento,
                       tta.departamento,
                       tta.tb_org_id,
                       tta.data_criacao,
                       tta.data_alteracao,
                       tco.cod_colaborador_externo
                   FROM
                       tb_tbd_alocado tta
                   LEFT JOIN
                       tb_colaborador_org tco ON tta.codigo_interno_colaborador_gestor = tco.codigo_interno_colaborador
                       AND tco.tb_org_id = tta.tb_org_id
                   WHERE
                       tta.tb_org_id = @OrgId" + Environment.NewLine; //newline necessário para nao dar erro na concatenacao
        }

        public TbdRepository()
        {
            this._connectionString = String.Format(
                @"server={0};database={1};uid={2};pwd={3};ConvertZeroDateTime=True",
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_HOSTNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.GCOLB_DATABASE),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_USERNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_PASSWORD)
            );
        }

        public async Task<IEnumerable<TbdAlocacaoDTO>> ListarTbd(string codGestor, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    var sql = SelectPadrao;

                    if (codGestor.EhStringValidaEDiferenteDeZero())
                    {
                        sql += $@"AND tco.cod_colaborador_externo = @CodGestor";
                    }

                    sql += @" GROUP BY
                                  tta.descricao;";

                    var listarTbds = await _connection.QueryAsync<dynamic>(sql, new
                    {
                        CodGestor = codGestor,
                        OrgId = orgId
                    });

                    return listarTbds.Select(x => new TbdAlocacaoDTO
                    {
                        CodTbdAlocado = x.cod_tbd_alocado,
                        Descricao = x.descricao,
                        CpfGestor = x.codigo_interno_colaborador_gestor,
                        CodDiretoria = x.cod_diretoria,
                        DescricaoDiretoria = x.diretoria,
                        CodDepartamento = x.cod_departamento,
                        DescricaoDepartamento = x.departamento,
                        OrgId = x.tb_org_id,
                        DataCriacao = x.data_criacao.ToString("dd-MM-yyyy"),
                        DataAlteracao = x.data_alteracao.ToString("dd-MM-yyyy"),
                        CodGestor = x.cod_colaborador_externo
                    }).ToList().OrderBy(x => x.Descricao);
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
        public async Task<TbdAlocacaoDTO> ObterTbdPorCodigo(int orgId, int codTbd)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    var sql = SelectPadrao;
                    sql += $@"AND tta.cod_tbd_alocado = @CodTbd";

                    var listarTbd = await _connection.QueryAsync<dynamic>(sql, new
                    {
                        OrgId = orgId,
                        CodTbd = codTbd
                    });

                    var tbdExistente = listarTbd.FirstOrDefault();
                    if (tbdExistente == null)
                    {
                        return null;
                    }
                    return new TbdAlocacaoDTO
                    {
                        CodTbdAlocado = tbdExistente.cod_tbd_alocado,
                        Descricao = tbdExistente.descricao,
                        CpfGestor = tbdExistente.codigo_interno_colaborador_gestor,
                        CodDiretoria = tbdExistente.cod_diretoria,
                        DescricaoDiretoria = tbdExistente.diretoria,
                        CodDepartamento = tbdExistente.cod_departamento,
                        DescricaoDepartamento = tbdExistente.departamento,
                        OrgId = tbdExistente.tb_org_id,
                        DataCriacao = tbdExistente.data_criacao.ToString("dd-MM-yyyy"),
                        DataAlteracao = tbdExistente.data_alteracao.ToString("dd-MM-yyyy"),
                        CodGestor = tbdExistente.cod_colaborador_externo
                    };
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um erro inesperado: " + e.Message);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<TbdAlocacaoDTO> InserirTbd(TbdAlocacaoParam param, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                await _connection.OpenAsync();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string sqlInsercao = @"
                    INSERT INTO tb_tbd_alocado (descricao, codigo_interno_colaborador_gestor, cod_diretoria, diretoria, tb_org_id, cod_departamento, departamento)
                    VALUES (@Descricao,
                            (SELECT codigo_interno_colaborador FROM tb_colaborador_org WHERE cod_colaborador_externo = @CodGestor AND tb_org_id = @OrgId),
                            @CodDiretoria, @DescricaoDiretoria, @OrgId, @CodDepartamento, @Departamento);

                    SELECT
                        cod_tbd_alocado,
                        descricao,
                        codigo_interno_colaborador_gestor,
                        cod_diretoria,
                        diretoria,
                        tb_org_id,
                        data_criacao,
                        data_alteracao,
                        (SELECT cod_colaborador_externo FROM tb_colaborador_org WHERE codigo_interno_colaborador = tta.codigo_interno_colaborador_gestor AND tb_org_id = tta.tb_org_id) AS cod_colaborador_externo
                    FROM tb_tbd_alocado tta
                    WHERE cod_tbd_alocado = LAST_INSERT_ID();";

                        var result = await _connection.QuerySingleAsync<dynamic>(sqlInsercao, new
                        {
                            Descricao = param.Descricao,
                            CodGestor = param.CodGestor,
                            CodDiretoria = param.CodDiretoria,
                            DescricaoDiretoria = param.DescricaoDiretoria,
                            OrgId = orgId,
                            CodDepartamento = param.CodDepartamento,
                            Departamento = param.DescricaoDepartamento
                        }, transaction);

                        transaction.Commit();

                        return new TbdAlocacaoDTO
                        {
                            CodTbdAlocado = result.cod_tbd_alocado,
                            Descricao = result.descricao,
                            CpfGestor = result.codigo_interno_colaborador_gestor,
                            CodDiretoria = result.cod_diretoria,
                            DescricaoDiretoria = result.diretoria,
                            CodDepartamento = result.cod_departamento,
                            DescricaoDepartamento = result.departamento,
                            CodGestor = result.cod_colaborador_externo,
                            OrgId = result.tb_org_id,
                            DataCriacao = result.data_criacao.ToString("dd-MM-yyyy"),
                            DataAlteracao = result.data_alteracao.ToString("dd-MM-yyyy"),
                        };
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
            }
        }

        public async Task<TbdAlocacaoDTO> AtualizarTbd(TbdAlocacaoParam param, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    string sqlAtualizacao = $@" UPDATE
                                                    tb_tbd_alocado  tta
                                                SET
                                                    tta.descricao = @Descricao,
                                                    tta.cod_diretoria = @CodDiretoria,
                                                    tta.diretoria = @Diretoria,
                                                    tta.cod_departamento = @CodDepartamento,
                                                    tta.departamento = @Departamento
                                                WHERE
                                                    tta.cod_tbd_alocado = @CodTbdAlocado;";

                    var result = await _connection.ExecuteAsync(sqlAtualizacao, new
                    {
                        Descricao = param.Descricao,
                        CodDiretoria = param.CodDiretoria,
                        Diretoria = param.DescricaoDiretoria,
                        OrgId = orgId,
                        CodTbdAlocado = param.CodTbdAlocado,
                        CodDepartamento = param.CodDepartamento,
                        Departamento = param.DescricaoDepartamento
                    });

                    // Verifica se o update afetou alguma linha
                    if (result == 0)
                    {
                        throw new Exception("TBD para atualização não foi encontrado.");
                    }

                    var sql = "select * from tb_tbd_alocado tta where";
                    sql += @" tta.cod_tbd_alocado = @CodTbdAlocado";

                    var tbdAtualizada = await _connection.QueryAsync<dynamic>(sql, new
                    {
                        OrgId = orgId,
                        CodTbdAlocado = param.CodTbdAlocado
                    });

                    var tbdExistente = tbdAtualizada.FirstOrDefault();

                    if (tbdExistente == null)
                    {
                        throw new Exception("Nenhum registro encontrado após a atualização.");
                    }

                    return new TbdAlocacaoDTO
                    {
                        CodTbdAlocado = tbdExistente.cod_tbd_alocado,
                        Descricao = tbdExistente.descricao,
                        CpfGestor = tbdExistente.codigo_interno_colaborador_gestor,
                        CodDiretoria = tbdExistente.cod_diretoria,
                        DescricaoDiretoria = tbdExistente.diretoria,
                        CodDepartamento = tbdExistente.cod_departamento,
                        DescricaoDepartamento = tbdExistente.departamento,
                        CodGestor = tbdExistente.cod_colaborador_externo,
                        OrgId = tbdExistente.tb_org_id,
                        DataCriacao = tbdExistente.data_criacao.ToString("dd-MM-yyyy"),
                        DataAlteracao = tbdExistente.data_alteracao.ToString("dd-MM-yyyy"),
                    };
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

        public async Task<StatusResult> DeletarTbd(int codTbd, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                var statusResult = new StatusResult();
                try
                {
                    string sql = @"DELETE FROM tb_tbd_alocado WHERE cod_tbd_alocado = @CodTbdAlocado AND tb_org_id = @OrgId";
                    await _connection.ExecuteAsync(sql, new { CodTbdAlocado = codTbd, OrgId = orgId });
                    return new StatusResult();
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

        public async Task<bool> ExisteColaboradorAtivo(string codGestor, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    string sql = $@"SELECT
                                        tco.cod_colaborador_externo
                                    FROM
                                        tb_colaborador_org tco
                                    WHERE
                                       ativo = 1 AND tb_org_id = @OrgId
                                    LIMIT 1;";

                    var lista = await _connection.QueryAsync<string>(sql, new
                    {
                        CodGestor = codGestor,
                        OrgId = orgId
                    });

                    return lista.Any();
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

        public bool ExisteAlocacaoNoTbd(int codTbd, int orgId)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    string sql = $@"SELECT
                                        COUNT(*)
                                    FROM
                                        tb_colaborador_periodo_alocacao tca
                                    WHERE
                                        tca.cod_tbd_alocado = @CodTbd
                                        AND tca.tb_org_id = @OrgId
                                        AND tca.ativo = 1;";

                    var result = _connection.ExecuteScalar<int>(sql, new
                    {
                        CodTbd = codTbd,
                        OrgId = orgId
                    });

                    return result > 0;
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
    }
}