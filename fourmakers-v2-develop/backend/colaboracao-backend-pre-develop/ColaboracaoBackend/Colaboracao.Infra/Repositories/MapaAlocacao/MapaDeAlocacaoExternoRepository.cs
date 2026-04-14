using Colaboracao.Core.Interfaces;
using Core.Domain.MapaAlocacao;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public class MapaDeAlocacaoExternoRepository : IMapaDeAlocacaoExternoRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public MapaDeAlocacaoExternoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<CadastroMapaAlocacaoDTO>> BuscarAlocacoesPorMesAno(int mes, int ano, int orgId, string codigoColaborador = null, string codigoProjeto = null)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    _connection.Open();

                    var inicioMes = new DateTime(ano, mes, 1);
                    var fimMes = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

                    var parametros = new DynamicParameters();
                    parametros.Add("@OrgId", orgId);
                    parametros.Add("@InicioMes", inicioMes);
                    parametros.Add("@FimMes", fimMes);

                    var condicaoWhere = "";

                    if (!string.IsNullOrEmpty(codigoColaborador))
                    {
                        condicaoWhere += " AND tcpa.codigo_colaborador = @CodigoColaborador";
                        parametros.Add("@CodigoColaborador", codigoColaborador);
                    }

                    if (!string.IsNullOrEmpty(codigoProjeto))
                    {
                        condicaoWhere += " AND tcpa.codigo_projeto = @CodigoProjeto";
                        parametros.Add("@CodigoProjeto", codigoProjeto);
                    }

                    var sql = $@"
                        SELECT
                            tcpa.id AS PeriodoAlocadoId,
                            tcpa.codigo_colaborador AS CodigoColaborador,
                            tcpa.codigo_interno_colaborador AS CpfColaborador,
                            tc.nome_completo AS ColaboradorNome,
                            tcpa.codigo_projeto AS CodigoProjeto,
                            tpo.projeto AS NomeProjeto,
                            tcpa.data_inicio AS DataInicio,
                            tcpa.data_fim AS DataFim,
                            tcpa.quantidade_horas AS QuantidadeHoras,
                            tcpa.inclui_fimdesemana AS IncluiFimDeSemana
                        FROM
                            tb_colaborador_periodo_alocacao tcpa
                        JOIN
                            tb_colaborador_org tco ON tcpa.codigo_colaborador = tco.cod_colaborador_externo
                            AND tco.tb_org_id = tcpa.tb_org_id
                        JOIN
                            tb_colaborador tc ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                        JOIN
                            tb_projeto_org tpo ON tpo.cod_projeto = tcpa.codigo_projeto AND tpo.tb_org_id = tcpa.tb_org_id
                        WHERE
                            tcpa.tb_org_id = @OrgId
                            AND tcpa.ativo = 1
                            AND tcpa.data_inicio <= @FimMes
                            AND tcpa.data_fim >= @InicioMes
                            {condicaoWhere}
                        ORDER BY
                            tcpa.codigo_colaborador, tcpa.codigo_projeto, tcpa.data_inicio";

                    var result = await _connection.QueryAsync<CadastroMapaAlocacaoDTO>(sql, parametros);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar alocacoes por mes/ano", ex);
            }
        }

        public async Task<DateTime[]> GetFeriadosPorOrgId(int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    _connection.Open();

                    var sql = @"SELECT data FROM tb_feriado WHERE tb_org_id = @OrgId AND ativo = 1";

                    var result = await _connection.QueryAsync<DateTime>(sql, new { OrgId = orgId });
                    return result.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar feriados", ex);
            }
        }
    }
}
