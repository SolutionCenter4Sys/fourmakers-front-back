using ApiClient.Domain;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Colaboracao.Infra.Repositories
{
    public class VistoColaboradorRepository : IVistoColaboradorRepository
    {
        private readonly string _connectionString;

        public VistoColaboradorRepository()
        {
            this._connectionString = String.Format(
                @"server={0};database={1};uid={2};pwd={3};ConvertZeroDateTime=True",
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_HOSTNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.GCOLB_DATABASE),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_USERNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_PASSWORD)
            );
        }

        public async Task<int> AdicionarVistoColaborador(
            string cpfColaborador,
            VistoColaboradorDTO vistoColaboradorDTO)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    string dtValidade = vistoColaboradorDTO.Validade.ToString("yyyy-MM-dd");

                    string sql = $@"INSERT INTO tb_colaborador_visto
                    (
                        codigo_interno_colaborador,
                        tb_pais_id,
                        validade,
                        data_criacao
                    )
                    VALUES(
                        @CpfColaborador,
                        @IdPais,
                        @Validade,
                        CURRENT_TIMESTAMP);

                    SELECT LAST_INSERT_ID();";

                    var result = await _connection.ExecuteScalarAsync<int>(sql,
                        new
                        {
                            CpfColaborador = cpfColaborador,
                            IdPais = vistoColaboradorDTO.IdPais,
                            Validade = dtValidade
                        }
                    );

                    return result;
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

        public async Task<bool> AlterarVistoColaborador(
            string cpfColaborador,
            VistoColaboradorDTO vistoColaboradorDTO)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                bool result = true;

                try
                {
                    _connection.Open();

                    string dtValidade = vistoColaboradorDTO.Validade.ToString("yyyy-MM-dd");

                    string sql = $@"UPDATE tb_colaborador_visto
                        SET
                            codigo_interno_colaborador = @CpfColaborador,
                            tb_pais_id = @IdPais,
                            validade = @Validade,
                            data_alteracao=CURRENT_TIMESTAMP
                        WHERE
                            id = @Id
                            AND codigo_interno_colaborador = @CpfColaborador;";

                    await _connection.ExecuteAsync(sql,
                        new
                        {
                            Id = vistoColaboradorDTO.Id,
                            CpfColaborador = cpfColaborador,
                            IdPais = vistoColaboradorDTO.IdPais,
                            Validade = dtValidade
                        }
                    );
                }
                catch (Exception)
                {
                    result = false;
                    throw;
                }
                finally
                {
                    _connection.Close();
                }

                return result;
            }
        }

        public async Task<bool> RemoverVistoColaborador(
            string cpfColaborador,
            int idVistoColaborador)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                bool result = true;

                try
                {
                    _connection.Open();

                    string sql = $@"DELETE
                        FROM
                            tb_colaborador_visto
                        WHERE
                            id = @Id
                            AND codigo_interno_colaborador = @CpfColaborador;";

                    await _connection.ExecuteAsync(sql,
                        new
                        {
                            Id = idVistoColaborador,
                            CpfColaborador = cpfColaborador
                        }
                    );
                }
                catch (Exception)
                {
                    result = false;
                    throw;
                }
                finally
                {
                    _connection.Close();
                }

                return result;
            }
        }

        public async Task<List<VistoColaboradorDTO>> ObterVistosPorCpfColaborador(
            string cpfColaborador)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    var listVistoColaboradorResult = new List<VistoColaboradorDTO>();

                    string sql = $@"SELECT
                            tcp.id,
                            tcp.codigo_interno_colaborador,
                            tcp.tb_pais_id,
                            tcp.validade,
                            tn.descricao
                        FROM tb_colaborador_visto tcp
                        JOIN tb_pais tn ON tcp.tb_pais_id = tn.id
                        WHERE
                            tcp.codigo_interno_colaborador = '{cpfColaborador}'
                        ORDER BY
                            tcp.id DESC;";

                    var result = await _connection.QueryAsync<tb_colaborador_visto, tb_pais, tb_colaborador_visto>(
                        sql, (colaboradorVisto, pais) =>
                        {
                            colaboradorVisto.tb_pais = pais;
                            return colaboradorVisto;
                        },
                        splitOn: "descricao");

                    var resultListDB = result.ToList();
                    if (resultListDB.Any())
                    {
                        listVistoColaboradorResult.AddRange(
                            resultListDB.Select(x => new VistoColaboradorDTO
                            {
                                Id = x.id,
                                IdPais = x.tb_pais_id,
                                Validade = x.validade,
                                DescricaoPais = x.tb_pais.Descricao,
                            }).ToList()
                        );
                    }

                    return listVistoColaboradorResult;
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