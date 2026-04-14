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
    public class PassaporteColaboradorRepository : IPassaporteColaboradorRepository
    {
        private readonly string _connectionString;

        public PassaporteColaboradorRepository()
        {
            this._connectionString = String.Format(
                @"server={0};database={1};uid={2};pwd={3};ConvertZeroDateTime=True",
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_HOSTNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.GCOLB_DATABASE),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_USERNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_PASSWORD)
            );
        }

        public async Task<int> AdicionarPassaporteColaborador(
            string cpfColaborador,
            PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    string dtValidade = passaporteColaboradorDTO.Validade.ToString("yyyy-MM-dd");

                    string sql = $@"INSERT INTO tb_colaborador_passaporte
                    (
                        codigo_interno_colaborador,
                        tb_nacionalidade_id,
                        validade,
                        data_criacao
                    )
                    VALUES(
                        @CpfColaborador,
                        @IdNacionalidade,
                        @Validade,
                        CURRENT_TIMESTAMP);

                    SELECT LAST_INSERT_ID();";

                    var result = await _connection.ExecuteScalarAsync<int>(sql,
                        new
                        {
                            CpfColaborador = cpfColaborador,
                            IdNacionalidade = passaporteColaboradorDTO.IdNacionalidade,
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

        public async Task<bool> AlterarPassaporteColaborador(
            string cpfColaborador,
            PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                bool result = true;

                try
                {
                    _connection.Open();

                    string dtValidade = passaporteColaboradorDTO.Validade.ToString("yyyy-MM-dd");

                    string sql = $@"UPDATE tb_colaborador_passaporte
                        SET
                            codigo_interno_colaborador = @CpfColaborador,
                            tb_nacionalidade_id = @IdNacionalidade,
                            validade = @Validade,
                            data_alteracao=CURRENT_TIMESTAMP
                        WHERE
                            id = @Id
                            AND codigo_interno_colaborador = @CpfColaborador;";

                    await _connection.ExecuteAsync(sql,
                        new
                        {
                            Id = passaporteColaboradorDTO.Id,
                            CpfColaborador = cpfColaborador,
                            IdNacionalidade = passaporteColaboradorDTO.IdNacionalidade,
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

        public async Task<bool> RemoverPassaporteColaborador(
            string cpfColaborador,
            int idPassaporteColaborador)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                bool result = true;

                try
                {
                    _connection.Open();

                    string sql = $@"DELETE
                        FROM
                            tb_colaborador_passaporte
                        WHERE
                            id = @Id
                            AND codigo_interno_colaborador = @CpfColaborador;";

                    await _connection.ExecuteAsync(sql,
                        new
                        {
                            Id = idPassaporteColaborador,
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

        public async Task<List<PassaporteColaboradorDTO>> ObterPassaportesPorCpfColaborador(
            string cpfColaborador)
        {
            using (var _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    var listPassaporteColaboradorResult = new List<PassaporteColaboradorDTO>();

                    string sql = $@"SELECT
                            tcp.id,
                            tcp.codigo_interno_colaborador,
                            tcp.tb_nacionalidade_id,
                            tcp.validade,
                            tn.descricao
                        FROM tb_colaborador_passaporte tcp
                        JOIN tb_nacionalidade tn ON tcp.tb_nacionalidade_id = tn.id
                        WHERE
                            tcp.codigo_interno_colaborador = '{cpfColaborador}'
                        ORDER BY
                            tcp.id DESC;";

                    var result = await _connection.QueryAsync<tb_colaborador_passaporte, tb_nacionalidade, tb_colaborador_passaporte>(
                        sql, (colaboradorPassaporte, nacionalidade) =>
                        {
                            colaboradorPassaporte.tb_nacionalidade = nacionalidade;
                            return colaboradorPassaporte;
                        },
                        splitOn: "descricao");

                    var resultListDB = result.ToList();
                    if (resultListDB.Any())
                    {
                        listPassaporteColaboradorResult.AddRange(
                            resultListDB.Select(x => new PassaporteColaboradorDTO
                            {
                                Id = x.id,
                                IdNacionalidade = x.tb_nacionalidade_id,
                                Validade = x.validade,
                                DescricaoNacionalidade = x.tb_nacionalidade.Descricao,
                            }).ToList()
                        );
                    }

                    return listPassaporteColaboradorResult;
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