using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Colaborador.Vistos;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.ModeloTrabalho;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class EstatisticasRepository : IEstatisticasRepository
    {
        private readonly string _connectionString;
        private readonly IDBConnection _dapperConnection;

        public EstatisticasRepository(IDBConnection dapperConnection)
        {
            this._connectionString = String.Format(
                @"server={0};database={1};uid={2};pwd={3};ConvertZeroDateTime=True",
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_HOSTNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.GCOLB_DATABASE),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_USERNAME),
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_PASSWORD)
            );
            _dapperConnection = dapperConnection;
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasEtniaPorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_etnia", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasIdadePorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_idade", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasTempoServicoPorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_tempo_servico", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.OrderBy(x => x.quantidade_total).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasOrientacaoSexualPorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_orientacao_sexual", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasEscolaridadePorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_escolaridade", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EstatisticasProcDTO> GetProcedureEstatisticasGeneroPorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    IEnumerable<EstatisticasProcDTO> ret = connection.Query<EstatisticasProcDTO>("spr_estatisticas_genero", new { p_org_id = orgId }, commandType: CommandType.StoredProcedure).ToList();
                    connection.Close();

                    return ret.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TotalizadoresDTO> ListTotalizadoresPorUnidade(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Diretoria AS Descricao, Count AS Total FROM vw_totalizadores_unidades WHERE tb_org_id = @orgId";

                    IEnumerable<TotalizadoresDTO> query = connection.Query<TotalizadoresDTO>(sql, new { orgId });

                    connection.Close();

                    var ret = query.ToList();

                    ret.Add(new TotalizadoresDTO()
                    {
                        Descricao = "Todas",
                        Total = ret.Sum(q => q.Total)
                    });

                    return ret.OrderBy(x => x.Descricao).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<LocalizacaoSumarioDTO>> ListarLocalizacaoSumario(int orgId)
        {
            using (IDbConnection _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    string sqlEstados = @$"
                                            SELECT
                                                COUNT(te.estado) AS qtd,
                                                te.estado
                                            FROM
                                                tb_colaborador_org tco
                                            JOIN
                                                tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                            JOIN
                                                tb_endereco te ON te.id = tc.endereco_id
                                            WHERE
                                                tco.tb_org_id = {orgId} AND tco.ativo = 1
                                            GROUP BY
                                                te.estado
                                        ;";

                    string sqlCidades = @$"
                                            SELECT
                                                COUNT(te.cidade) AS qtd,
                                                te.cidade,
                                                te.estado
                                            FROM
                                                tb_colaborador_org tco
                                            JOIN
                                                tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                            JOIN
                                                tb_endereco te ON te.id = tc.endereco_id
                                            WHERE
                                                tco.tb_org_id = {orgId} AND tco.ativo = 1
                                            GROUP BY
                                                te.cidade
                                        ;";

                    _connection.Open();

                    var resultEstados = await _connection.QueryAsync<dynamic>(sqlEstados);
                    var resultCidades = await _connection.QueryAsync<dynamic>(sqlCidades);

                    var result = resultEstados
                                .Where(x => !String.IsNullOrEmpty(x.estado))
                                .Select(x => new LocalizacaoSumarioDTO
                                {
                                    Estado = x.estado,
                                    QtdUsuarios = (int)x.qtd,
                                    Cidades = resultCidades.Where(y => y.estado == x.estado && !String.IsNullOrEmpty(y.cidade)).Select(cidade => new CidadesSumarioDTO()
                                    {
                                        Estado = x.estado,
                                        Cidade = cidade.cidade,
                                        QtdUsuarios = (int)cidade.qtd
                                    }).ToList()
                                })
                                .OrderByDescending(x => x.QtdUsuarios)
                                .ToList();

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }

        public async Task<List<CargoColaboradorSumario>> ListarCargoSumario(int orgId)
        {
            using (IDbConnection _connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    string sql = @$"
                                    SELECT
                                        COUNT(cargo) AS qtd,
                                        codigo_cargo,
                                        cargo
                                    FROM
                                        tb_colaborador_org
                                    WHERE
                                        cargo IS NOT NULL AND cargo <> '' AND ativo = 1 AND tb_org_id = {orgId}
                                    GROUP BY
                                        codigo_cargo, cargo
                                ;";

                    _connection.Open();

                    var resultdb = await _connection.QueryAsync<dynamic>(sql);

                    var result = resultdb
                                .Select(x => new CargoColaboradorSumario
                                {
                                    CodigoCargo = x.codigo_cargo,
                                    Cargo = x.cargo,
                                    QtdUsuarios = (int)x.qtd
                                })
                                .OrderByDescending(x => x.QtdUsuarios)
                                .ToList();

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
            }
        }

        public async Task<EstatisticasModeloTrabalhoResult> GetEstatisticasModeloTrabalhoPorOrg(int orgId)
        {
            try
            {
                using (IDbConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var sql = @"
                                SELECT
                                    modelo_trabalho,
                                    COUNT(*) AS qtdUsuarios,
                                    CASE WHEN modelo_trabalho = 'H�brido' THEN dias_por_semana ELSE NULL END AS diasPorSemana
                                FROM
                                    tb_colaborador_org
                                WHERE
                                    tb_org_id = @OrgId
                                    AND ativo = 1
                                GROUP BY
                                    modelo_trabalho, dias_por_semana
                                ORDER BY
                                    modelo_trabalho;
                    ";

                    var result = await connection.QueryAsync<dynamic>(sql, new { OrgId = orgId });

                    var response = new EstatisticasModeloTrabalhoResult();

                    response.ModelosTrabalho.Add(new ModeloTrabalhoDTO
                    {
                        ModeloTrabalho = ModeloTrabalhoConst.Remoto100Porcento,
                        QtdUsuarios = 0,
                        DiasPorSemana = null
                    });

                    response.ModelosTrabalho.Add(new ModeloTrabalhoDTO
                    {
                        ModeloTrabalho = "100% Presencial",
                        QtdUsuarios = 0,
                        DiasPorSemana = null
                    });

                    var modeloHibrido = new ModeloTrabalhoDTO
                    {
                        ModeloTrabalho = "H�brido",
                        QtdUsuarios = 0,
                        DiasPorSemana = new List<DiasPorSemanaDTO>
                    {
                    new DiasPorSemanaDTO { Dias = 1, QtdUsuarios = 0 },
                    new DiasPorSemanaDTO { Dias = 2, QtdUsuarios = 0 },
                    new DiasPorSemanaDTO { Dias = 3, QtdUsuarios = 0 },
                    new DiasPorSemanaDTO { Dias = 4, QtdUsuarios = 0 }
                        }
                    };

                    response.ModelosTrabalho.Add(modeloHibrido);

                    response.ModelosTrabalho.Add(new ModeloTrabalhoDTO
                    {
                        ModeloTrabalho = "N�o Respondeu",
                        QtdUsuarios = 0,
                        DiasPorSemana = null
                    });

                    foreach (var item in result)
                    {
                        if (string.IsNullOrEmpty(item.modelo_trabalho))
                        {
                            response.ModelosTrabalho.First(x => x.ModeloTrabalho == "N�o Respondeu").QtdUsuarios = item.qtdUsuarios;
                        }
                        else if (item.modelo_trabalho == ModeloTrabalhoConst.Remoto100Porcento)
                        {
                            response.ModelosTrabalho.First(x => x.ModeloTrabalho == ModeloTrabalhoConst.Remoto100Porcento).QtdUsuarios = item.qtdUsuarios;
                        }
                        else if (item.modelo_trabalho == ModeloTrabalhoConst.Presencial100Porcento)
                        {
                            response.ModelosTrabalho.First(x => x.ModeloTrabalho == ModeloTrabalhoConst.Presencial100Porcento).QtdUsuarios = item.qtdUsuarios;
                        }
                        else if (item.modelo_trabalho == ModeloTrabalhoConst.Hibrido)
                        {
                            var hibrido = response.ModelosTrabalho.First(x => x.ModeloTrabalho == ModeloTrabalhoConst.Hibrido);
                            hibrido.QtdUsuarios += item.qtdUsuarios;

                            if (item.diasPorSemana == 1)
                                hibrido.DiasPorSemana.First(x => x.Dias == 1).QtdUsuarios = item.qtdUsuarios;
                            else if (item.diasPorSemana == 2)
                                hibrido.DiasPorSemana.First(x => x.Dias == 2).QtdUsuarios = item.qtdUsuarios;
                            else if (item.diasPorSemana == 3)
                                hibrido.DiasPorSemana.First(x => x.Dias == 3).QtdUsuarios = item.qtdUsuarios;
                            else if (item.diasPorSemana == 4)
                                hibrido.DiasPorSemana.First(x => x.Dias == 4).QtdUsuarios = item.qtdUsuarios;
                        }
                    }

                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasCursoDTO>> ListarEstatisticasCursos(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                SELECT
                    tc.descricao AS Descricao,
                    cursoSemestre.semestre AS Semestre,
                    Count(cursoSemestre.codigo_interno_colaborador) AS Colaboradores
                FROM (
                    SELECT tcdc.codigo_interno_colaborador,
                        tcdc.tb_curso_id,
                        Max(tcdc.semestre) AS semestre
                    FROM
                        tb_curso_disciplina_colaborador tcdc
                    GROUP BY
                        tcdc.codigo_interno_colaborador,
                        tcdc.tb_curso_id,
                        tcdc.semestre
                        ) AS cursoSemestre
                    INNER JOIN tb_curso tc
                        ON tc.id = cursoSemestre.tb_curso_id
                        AND tc.tb_org_id = @OrgId
                GROUP BY
                    tc.descricao,
                    cursoSemestre.semestre
            ";

            var resultados = await connection.QueryAsync<EstatisticasCursoDTO, EstatisticasSemestreDTO, EstatisticasCursoDTO>(
                query,
                (curso, semestre) =>
                {
                    if (curso.Semestres == null)
                        curso.Semestres = new List<EstatisticasSemestreDTO>();

                    curso.Semestres.Add(semestre);

                    return curso;
                },
                new { OrgId = orgId },
                splitOn: "Semestre"
            );

            var cursosAgrupados = resultados
                .GroupBy(c => c.Descricao)
                .Select(g => new EstatisticasCursoDTO
                {
                    Descricao = g.Key,
                    Semestres = g.SelectMany(c => c.Semestres).Distinct().OrderBy(x => x.Semestre).ToList(),
                    Total = g.SelectMany(c => c.Semestres).Sum(s => s.Colaboradores)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return cursosAgrupados;
        }

        public async Task<List<EstatisticaCidadaniaDTO>> ListarEstatisticasCidadania(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                SELECT
                    tbc.descricao AS Descricao,
                    tcs.descricao AS Descricao,
                    COUNT(tcc.id) AS Colaboradores
                FROM
                    tb_cidadania tbc
                LEFT JOIN tb_cidadania_colaborador tcc
                    ON tcc.tb_cidadania_id = tbc.id
                LEFT JOIN tb_cidadania_status tcs
                    ON tcs.id = tcc.tb_cidadania_status_id
                LEFT JOIN tb_colaborador_org tco
                    ON tco.codigo_interno_colaborador = tcc.codigo_interno_colaborador
                WHERE tcc.ativo = 1
                AND
                    tco.tb_org_id = @OrgId
                GROUP BY
                    tbc.descricao, tcs.descricao
                ORDER BY
                    tbc.descricao, tcs.descricao;
            ";

            var resultados = await connection.QueryAsync<EstatisticaCidadaniaDTO, EstatisticaCidadaniaStatusDTO, EstatisticaCidadaniaDTO>(
                query,
                (cidadania, status) =>
                {
                    if (cidadania.Status == null)
                        cidadania.Status = new List<EstatisticaCidadaniaStatusDTO>();

                    cidadania.Status.Add(status);

                    return cidadania;
                },
                new { OrgId = orgId },
                splitOn: "Descricao"
            );

            var cidadaniasAgrupadas = resultados
                .GroupBy(c => c.Descricao)
                .Select(g => new EstatisticaCidadaniaDTO
                {
                    Descricao = g.Key,
                    Status = g.SelectMany(c => c.Status).Distinct().OrderBy(x => x.Descricao).ToList(),
                    Total = g.SelectMany(c => c.Status).Sum(s => s.Colaboradores)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return cidadaniasAgrupadas;
        }

        public async Task<List<EstatisticaVistoDTO>> ListarEstatisticasVisto(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                SELECT
                    tp.Descricao AS Descricao,
                    COUNT(tbc.id) AS Total
                FROM
                    tb_colaborador_visto tbc
                JOIN tb_pais tp
                    ON tp.id = tbc.tb_pais_id
                JOIN tb_colaborador_org tco
                    ON tco.codigo_interno_colaborador = tbc.codigo_interno_colaborador
                WHERE tco.ativo = 1
                AND
                    tco.tb_org_id = @OrgId
                GROUP BY
                    tp.Descricao
                ORDER BY
                    tp.Descricao;
            ";

            var parametros = new
            {
                OrgId = orgId
            };

            var resultados = await connection.QueryAsync<EstatisticaVistoDTO>(query, parametros);

            return resultados.ToList();
        }
    }
}