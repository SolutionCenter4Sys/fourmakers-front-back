using Apontamento.Domain.Enums;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.Apontamento;
using Dapper;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ColaboradorEApontamento;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Colaborador;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;
using Microsoft.IdentityModel.Tokens;
using static Dapper.SqlMapper;
using AprovadorResult = DataTransferObject.Domain.Apontamento.ColaboradorEApontamento.AprovadorResult;

namespace Colaboracao.Infra.Repositories.Apontamento
{
    public class ApontamentoRepository : IApontamentoRepository
    {
        private IStringLocalizer<ApontamentoMessage> _stringLocalizer;
        private IDBConnection _dapperConnection;

        public ApontamentoRepository(IStringLocalizer<ApontamentoMessage> stringLocalizer, IDBConnection dapperConnection)
        {
            _stringLocalizer = stringLocalizer;
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<ProjetoAtividadeDTO>> ListarProjetosComAtividadesPorCPF(string cpf, int orgId, bool lancamentoParaOutroColaborador = false)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_codigo_interno_colaborador", cpf);
                parameters.Add("@p_org_id", orgId);
                parameters.Add("@p_lancamento_para_outro_colaborador", lancamentoParaOutroColaborador);

                var resultado = await connection.QueryAsync<dynamic>(
                    "spr_get_projeto_com_atividades_por_cpf",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var atividadesPorColaborador = resultado.GroupBy(r => r.cod_projeto)
                                                        .Select(g => new ProjetoAtividadeDTO
                                                        {
                                                            Cod_projeto = g.First().cod_projeto,
                                                            Projeto = g.First().nome_projeto,
                                                            CodigoCliente = g.First().cod_cliente,
                                                            NomeCliente = g.First().cliente,
                                                            Atividades = g.Select(r => new AtividadeDTO
                                                            {
                                                                Id = r.atividade_id.ToString(),
                                                                Descricao = r.descricao_atividade
                                                            }).DistinctBy(x => x.Id).ToList()
                                                        });

                return atividadesPorColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProjetoAtividadeDTO>> ListarProjetosAtivosPorColaborador(string cpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var projetos = new List<ProjetoAtividadeDTO>();

                string sql = $@"SELECT
                                    cpo.cod_colaborador,
                                    cpo.cod_projeto,
                                    cpo.tb_org_id,
                                    po.projeto as nome_projeto
                                FROM
                                    tb_colaborador_projeto_org cpo
                                JOIN
									tb_colaborador_org co on cpo.cod_colaborador = co.cod_colaborador_externo and cpo.tb_org_id = co.tb_org_id
                                JOIN
                                    tb_projeto_org as po ON cpo.cod_projeto = po.cod_projeto and cpo.tb_org_id = po.tb_org_id
                                WHERE
                                    co.codigo_interno_colaborador = @CPF
                                    AND cpo.tb_org_id = @OrgId
                                ORDER BY po.projeto";

                var result = await connection.QueryAsync<tb_colaborador_projeto_org>(sql,
                    new
                    {
                        CPF = cpf,
                        OrgId = orgId
                    }
                );

                var resultListDB = result.ToList();
                if (resultListDB != null && resultListDB.Any())
                {
                    projetos.AddRange(
                        resultListDB.Select(x => new ProjetoAtividadeDTO
                        {
                            Cod_projeto = x.cod_projeto,
                            Projeto = x.nome_projeto
                        }).ToList()
                    );
                }

                return projetos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ApontamentoMensalDTO>> ListarApontamentosMensaisPorVigenciaColaborador(int mes, int ano, string cpf, string codColaborador, int orgId, string cpfGerente,
            bool soProjetosDesteGerente = false)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var atividades = new List<ApontamentoMensalDTO>();

                string whereGerente = "";
                string gerentePrioridade = "";

                if (soProjetosDesteGerente)
                {
                    whereGerente = $@"
	                        AND tipo_gerente = 'GerenteProjeto' AND codigo_interno_colaborador_gerente = @CpfGerente";
                }
                else
                {
                    gerentePrioridade = ",vam.id_apontamento,row_number() OVER (PARTITION BY vam.id_apontamento ORDER BY vam.cod_gerente) as gerente_prioridade";
                }

                string sql = $@"SELECT
	                                    vam.cod_projeto,
	                                    vam.gerente,
                                        vam.cod_gerente,
	                                    vam.total_horas,
	                                    vam.mes,
	                                    vam.ano,
                                        vam.nome_projeto,
                                        vam.atividade,
	                                    vam.apontamento_reprovado,
	                                    vam.codigo_interno_colaborador,
	                                    vam.tb_org_id,
                                        vam.cod_status_mensal,
                                        vam.descricao_status_mensal,
                                        vam.status_apontamento_grupo_prioridade,
                                        vam.nome_cliente,
                                        vam.cod_cliente
                                        {gerentePrioridade}
                                    FROM
                                        vw_apontamento_mensal vam
                                    WHERE
	                                    vam.codigo_interno_colaborador = @CpfColaborador
	                                    AND vam.tb_org_id = @OrgId
	                                    AND vam.mes = @Mes
	                                    AND vam.ano = @Ano
                                        {whereGerente}
                                    ORDER BY vam.total_horas DESC
                     ";

                if (!soProjetosDesteGerente)
                {
                    sql = $@"SELECT
                                    *
                                 FROM
                                 (
                                    {sql}
                                 ) AS subquery
                                 WHERE
                                    subquery.gerente_prioridade = 1";
                }

                sql += ";";

                var result = await connection.QueryAsync<vw_apontamento_mensal>(sql,
                    new
                    {
                        Mes = mes,
                        Ano = ano,
                        CpfColaborador = cpf,
                        CodColaborador = codColaborador,
                        OrgId = orgId,
                        CpfGerente = cpfGerente
                    }
                );

                var resultListDB = result.ToList();

                if (resultListDB != null && resultListDB.Any())
                {
                    atividades.AddRange(
                        resultListDB.Select(x => new ApontamentoMensalDTO
                        {
                            Projeto = new ProjetoApontamentoMensalDTO()
                            {
                                CodProjeto = x.cod_projeto,
                                NomeProjeto = x.nome_projeto,
                                Gerente = x.gerente ?? _stringLocalizer.GetStringOuVazio("MANAGER_NOT_INFORMED"),
                                CodCliente = x.cod_cliente,
                                NomeCliente = x.nome_cliente
                            },
                            Horas = x.total_horas,
                            ApontamentoReprovado = Convert.ToBoolean(x.apontamento_reprovado),
                            CodStatusGrupoMensal = x.cod_status_mensal,
                            Aprovadores = null,
                            //DescricaoStatusMensal = x.descricao_status_mensal,
                            PrioridadeStatusApontamentoGrupo = x.status_apontamento_grupo_prioridade,
                            Observacao = x.observacao,
                            Atividade = x.atividade
                        }).ToList()
                    );
                }

                return atividades;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<AprovadorApontamentoMensalDTO>> ListarAprovadoresApontamentosMensaisColaborador(int mes, int ano, string cpf, string codProjeto, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var aprovadores = new List<AprovadorApontamentoMensalDTO>();

                string sqlAprovadores = @"SELECT DISTINCT
                                                    tca.codigo_interno_colaborador_justificativa AS cpf, tc.nome_completo AS nome
                                                FROM
                                                    tb_colaborador_apontamento tca
                                                LEFT JOIN
                                                    tb_colaborador tc
                                                    ON
                                                        tca.codigo_interno_colaborador_justificativa = tc.codigo_interno_colaborador
                                                WHERE
                                                    tca.tb_projeto_org_cod_projeto = @CodProjeto
                                                AND
                                                    MONTH(tca.data) = @Mes
                                                AND
                                                    YEAR(tca.data) = @Ano
                                                AND
                                                    tca.codigo_interno_colaborador = @Cpf
                                                AND
                                                    tca.tb_org_id = @OrgId
                                                AND
                                                    tca.codigo_interno_colaborador_justificativa IS NOT NULL;";

                var result = await connection.QueryAsync<AprovadorApontamentoMensalDTO>(sqlAprovadores,
                    new
                    {
                        CodProjeto = codProjeto,
                        Mes = mes,
                        Ano = ano,
                        Cpf = cpf,
                        OrgId = orgId
                    }
                );

                var resultListDB = result.Distinct().ToList();
                if (resultListDB != null && resultListDB.Any())
                {
                    aprovadores.AddRange(resultListDB);
                }
                return aprovadores;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ApontamentoColaboradorDTO>> ListarApontamentosPorVigenciaColaborador(int mes, int ano, string cpf, string codColaborador, int orgId,
            string cpfGerente, bool soProjetosDesteGerente = false)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var atividades = new List<ApontamentoColaboradorDTO>();

                string whereGerente = "";
                string gerentePrioridade = "";

                if (soProjetosDesteGerente)
                {
                    whereGerente = $@"
                    AND (
    	                vgha.cod_interno_gestor = @CpfGerente
    	                OR
    	                (tipo_gerente = 'GerenteProjeto' AND codigo_interno_colaborador_gerente = @CpfGerente)
                    )";
                }
                else
                {
                    gerentePrioridade = ", row_number() OVER(PARTITION BY vca.tb_colaborador_apontamento_id, vca.data_registro, vca.cod_projeto, vca.cod_status_apontamento, vca.atividade_descricao, vca.data_justificativa ORDER BY vca.cod_gerente) as gerente_prioridade";
                }
                
                var groupBy = @"
                        Group by
                        vca.tb_colaborador_apontamento_id";

                string sql = $@"    SELECT
	                                        vca.tb_colaborador_apontamento_id,
	                                        vca.horas,
	                                        vca.justificativa,
	                                        vca.data_justificativa,
	                                        vca.nome_usuario_justificativa,
	                                        vca.data_registro,
	                                        vca.mes,
	                                        vca.ano,
	                                        vca.numero_semana,
	                                        vca.numero_semana_dia,
	                                        vca.tipo_apontamento,
	                                        vca.status_apontamento_grupo,
                                            vca.cod_status_apontamento_grupo,
                                            vca.status_apontamento,
	                                        vca.cod_projeto,
	                                        vca.nome_projeto,
                                            vca.cod_cliente,
                                            vca.nome_cliente,
                                            vca.permite_apont_sem_alocacao,
                                            vca.permite_apont_sem_alocacao_outro_colab,
	                                        vca.gerente,
	                                        vca.atividade_id,
	                                        vca.atividade_descricao,
	                                        vca.codigo_interno_colaborador,
	                                        vca.tb_org_id,
                                            vca.observacao
                                            {gerentePrioridade}
                                        FROM
                                            vw_colaborador_apontamento vca
                                        LEFT JOIN vw_gestor_hierarquico_aprovadores vgha
	                                        ON vgha.cod_interno_colaborador = vca.codigo_interno_colaborador and vgha.tb_org_id = vca.tb_org_id
                                        WHERE
	                                        vca.codigo_interno_colaborador = @CpfColaborador
	                                        AND vca.tb_org_id = @OrgId
	                                        AND vca.mes = @Mes
	                                        AND vca.ano = @Ano
                                            {whereGerente}
                                            {groupBy}
                                        ";

                if (!soProjetosDesteGerente)
                {
                    sql = $@"SELECT
                                    *
                                 FROM
                                 (
                                    {sql}
                                 ) AS subquery
                                 WHERE
                                    subquery.gerente_prioridade = 1";
                }

                var result = await connection.QueryAsync<vw_colaborador_apontamento>(sql,
                new
                {
                    Mes = mes,
                    Ano = ano,
                    CpfColaborador = cpf,
                    CodColaborador = codColaborador,
                    OrgId = orgId,
                    CpfGerente = cpfGerente
                }
            );

                var resultListDB = result.ToList();
                if (resultListDB != null && resultListDB.Any())
                {
                    atividades.AddRange(
                        resultListDB.Select(x => new ApontamentoColaboradorDTO
                        {
                            Projeto = new ProjetoDTO()
                            {
                                CodProjeto = x.cod_projeto,
                                NomeProjeto = x.nome_projeto,
                                Gerente = x.gerente ?? _stringLocalizer.GetStringOuVazio("MANAGER_NOT_INFORMED"),
                                NomeCliente = x.nome_cliente,
                                CodCliente = x.cod_cliente,
                                PermiteApontamentoSemAlocacao = x.permite_apont_sem_alocacao,
                                PermiteApontamentoSemAlocacaoParaOutroColaborador = x.permite_apont_sem_alocacao_outro_colab
                            },
                            Atividade = new AtividadeDTO()
                            {
                                Id = x.atividade_id.ToString(),
                                Descricao = x.atividade_descricao
                            },
                            TbColaboradorApontamentoId = x.tb_colaborador_apontamento_id.ToString(),
                            Horas = x.horas,
                            Justificativa = x.justificativa,
                            DataUsuarioJustificativa = x.data_justificativa,
                            NomeUsuarioJustificativa = x.nome_usuario_justificativa,
                            Data = x.data_registro,
                            CodStatusApontamentoGrupo = x.cod_status_apontamento_grupo,
                            NumeroSemana = x.numero_semana,
                            NumeroSemanaDia = x.numero_semana_dia,
                            Observacao = x.observacao
                        }).ToList()
                    );
                }

                return atividades;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeletarApontamento(string colaborador_apontamento_id)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"DELETE FROM
                                   tb_colaborador_apontamento
                               WHERE
                                   id = @Id;";

                var result = connection.Execute(sql, new { Id = colaborador_apontamento_id });

                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int AtualizarStatusApontamentoAprovadoOuReprovadoPorIds(List<string> ids, int codStatusApontamento, string justificativa, string cpfRequest)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"
                                    UPDATE
                                        tb_colaborador_apontamento
                                    SET
                                        justificativa = @Justificativa,
                                        codigo_interno_colaborador_alteracao = @CpfRequest,
                                        codigo_interno_colaborador_justificativa = @CpfRequest,
                                        tb_status_apontamento_id = (
                                            SELECT
                                                id
                                            FROM
                                                tb_status_apontamento
                                            WHERE
                                                cod_status_apontamento = @CodStatusApontamento),
                                                codigo_interno_colaborador_justificativa = @CpfRequest,
                                                data_justificativa = NOW()
                                    WHERE
                                        id IN @Ids;";

                var result = connection.Execute(sql, new
                {
                    Justificativa = justificativa,
                    CodStatusApontamento = codStatusApontamento,
                    Ids = ids,
                    CpfRequest = cpfRequest
                });

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ColaboradorApontamentoDTO> GetApontamentoById(string colaboradorApontamentoId, string idioma)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
                                    tca.id,
                                    tca.horas,
									tca.justificativa,
									tca.data,
									tca.numero_semana,
									tca.numero_semana_dia,
									tca.tb_atividade_id,
									tca.tb_status_apontamento_id,
									tca.tipo_apontamento_id,
									tca.tb_vigencia_id,
									tca.tb_projeto_org_cod_projeto,
									tca.tb_org_id,
									tca.codigo_interno_colaborador,
									tca.observacao,
									tca.data_criacao,
									tca.data_alteracao,
									tca.codigo_interno_colaborador_criacao,
									tca.codigo_interno_colaborador_alteracao,
									tca.codigo_interno_colaborador_justificativa,
									tca.data_justificativa,
                                    tsa.descricao AS tb_status_descricao,
                                    COALESCE(tt.traducao,tsag.descricao) AS tb_status_grupo_descricao,
                                    tsag.cod_status_grupo AS tb_status_grupo_codigo,
                                    tca.data_alteracao
                                FROM
                                    tb_colaborador_apontamento tca
                                JOIN
                                    tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
                                JOIN
                                    tb_status_apontamento_grupo tsag ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
								LEFT JOIN
									tb_traducoes tt ON tsag.cod_status_grupo = tt.valor_chave
									AND tt.tabela_chave = 'tb_status_apontamento_grupo.cod_status_grupo'
									AND tt.idioma = @Idioma
                                WHERE
                                    tca.id = @Id;";

                var apontamento = await connection.QuerySingleOrDefaultAsync<tb_colaborador_apontamento>(sql, new { Id = colaboradorApontamentoId, Idioma = idioma });

                if (apontamento == null)
                    return null;

                var apontamentoRetorno = new ColaboradorApontamentoDTO()
                {
                    Id = apontamento.id.ToString(),
                    Horas = apontamento.horas,
                    Justificativa = apontamento.justificativa,
                    Data = apontamento.data,
                    NumeroSemana = apontamento.numero_semana,
                    NumeroSemanaDia = apontamento.numero_semana_dia,
                    AtividadeId = apontamento.tb_atividade_id.ToString(),
                    StatusApontamentoId = apontamento.tb_status_apontamento_id.ToString(),
                    TipoApontamento = apontamento.tipo_apontamento_id,
                    VigenciaId = apontamento.tb_vigencia_id.ToString(),
                    ProjetoCodigo = apontamento.tb_projeto_org_cod_projeto,
                    OrgId = apontamento.tb_org_id,
                    ColaboradorCpf = apontamento.codigo_interno_colaborador,
                    StatusDescricao = apontamento.tb_status_descricao,
                    StatusGrupoDescricao = apontamento.tb_status_grupo_descricao,
                    StatusGrupoCodigo = apontamento.tb_status_grupo_codigo,
                    Observacao = apontamento.observacao,
                    DataAlteracao = apontamento.data_alteracao
                };

                return apontamentoRetorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ColaboradorApontamentoDTO>> GetApontamentosColaborador(string colaboradorCpf, int orgId, string projetoId = null, string atividadeId = null,
            DateTime? dataRegistro = null, List<DateTime> periodo = null, int? mes = null, int? ano = null)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                if (periodo == null)
                {
                    periodo = new List<DateTime>();
                }

                string sql = $@"
                                    SELECT
                                        a.*,
                                        s.descricao AS tb_status_descricao,
                                        tsag.descricao AS tb_status_grupo_descricao,
                                        tsag.cod_status_grupo AS tb_status_grupo_codigo
                                    FROM
                                        tb_colaborador_apontamento a
                                    JOIN
                                        tb_status_apontamento s ON a.tb_status_apontamento_id = s.id
                                    JOIN
                                        tb_status_apontamento_grupo tsag ON s.tb_cod_status_grupo = tsag.cod_status_grupo
                                    WHERE
                                        a.codigo_interno_colaborador = @ColaboradorCpf
                                        AND a.tb_org_id = @OrgId
                                        AND (@ProjetoId IS NULL OR a.tb_projeto_org_cod_projeto = @ProjetoId)
                                        AND (@AtividadeId IS NULL OR a.tb_atividade_id = @AtividadeId)
                                        AND (@DataRegistro IS NULL OR a.data = @DataRegistro)
                                        {(periodo.Count > 0 ? "AND a.data IN @Periodo" : "")}
                                        AND (@Mes IS NULL OR MONTH(a.data) = @Mes)
                                        AND (@Ano IS NULL OR YEAR(a.data) = @Ano)
                                    GROUP BY 
                                        a.id;
                                    ";

                var apontamentos = await connection.QueryAsync<tb_colaborador_apontamento>(sql,
                    new
                    {
                        ColaboradorCpf = colaboradorCpf,
                        OrgId = orgId,
                        ProjetoId = projetoId,
                        AtividadeId = atividadeId,
                        DataRegistro = dataRegistro,
                        Periodo = periodo,
                        Mes = mes,
                        Ano = ano
                    }
                );

                var apontamentosRetorno = apontamentos.Select(apontamento => new ColaboradorApontamentoDTO()
                {
                    Id = apontamento.id.ToString(),
                    Horas = apontamento.horas,
                    Justificativa = apontamento.justificativa,
                    Data = apontamento.data,
                    NumeroSemana = apontamento.numero_semana,
                    NumeroSemanaDia = apontamento.numero_semana_dia,
                    AtividadeId = apontamento.tb_atividade_id.ToString(),
                    StatusApontamentoId = apontamento.tb_status_apontamento_id.ToString(),
                    TipoApontamento = apontamento.tipo_apontamento_id,
                    VigenciaId = apontamento.tb_vigencia_id.ToString(),
                    ProjetoCodigo = apontamento.tb_projeto_org_cod_projeto,
                    OrgId = apontamento.tb_org_id,
                    ColaboradorCpf = apontamento.codigo_interno_colaborador,
                    StatusDescricao = apontamento.tb_status_descricao,
                    StatusGrupoDescricao = apontamento.tb_status_grupo_descricao,
                    StatusGrupoCodigo = apontamento.tb_status_grupo_codigo,
                    Observacao = apontamento.observacao,
                    DataAlteracao = apontamento.data_alteracao
                }).ToList();

                return apontamentosRetorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VigenciaSimplesDTO>> GetVigenciaColaborador(string colaboradorCpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
										a.codigo_interno_colaborador as cpf,
                                        MONTH(a.data) as mes,
                                        YEAR(a.data) as ano
                                    FROM
                                        tb_colaborador_apontamento a
                                    WHERE
                                        a.codigo_interno_colaborador = @ColaboradorCpf AND a.tb_org_id = @OrgId
									GROUP BY
										cpf, mes, ano";

                var vigencias = await connection.QueryAsync<(string cpf, int mes, int ano)>(sql,
                new
                {
                    ColaboradorCpf = colaboradorCpf,
                    OrgId = orgId
                }
                );

                var vigenciaSimplesDTOs = vigencias.Select(vig => new VigenciaSimplesDTO()
                {
                    Mes = vig.mes,
                    Ano = vig.ano
                }).ToList();

                return vigenciaSimplesDTOs;
            }
            catch (Exception)
            {
                throw;
            }
        }

       

        public async Task<List<VigenciaSimplesDTO>> GetVigenciaApontamentoGerenteDeProjetos(string gerenteCpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
										co.codigo_interno_colaborador as cpf_gerente,
										MONTH(a.data) as mes,
										YEAR(a.data) as ano
                                    FROM
										tb_colaborador_apontamento a
                                    JOIN
										tb_projeto_org po on a.tb_projeto_org_cod_projeto = po.cod_projeto
                                    JOIN
										tb_projeto_gerente tpg on tpg.cod_projeto = po.cod_projeto AND tpg.tipo_gerente = 'GerenteProjeto'
                                    JOIN
										tb_colaborador_org co on tpg.cod_colaborador_gerente = co.cod_colaborador_externo
                                    WHERE
										co.codigo_interno_colaborador = @GerenteCpf AND a.tb_org_id = @OrgId
									GROUP BY
										cpf_gerente, mes, ano";

                var vigencias = await connection.QueryAsync<(string cpf, int mes, int ano)>(sql,
                new
                {
                    GerenteCpf = gerenteCpf,
                    OrgId = orgId
                }
                );

                var vigenciaSimplesDTOs = vigencias.Select(vig => new VigenciaSimplesDTO()
                {
                    Mes = vig.mes,
                    Ano = vig.ano
                }).ToList();

                return vigenciaSimplesDTOs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ColaboradorApontamentoDTO> InserirApontamento(ColaboradorApontamentoDTO colaboradorApontamentoDTO, string idioma, string cpfRequest)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"INSERT INTO `tb_colaborador_apontamento` (
                                        `id`,
                                        `horas`,
                                        `justificativa`,
                                        `data`,
                                        `numero_semana`,
                                        `numero_semana_dia`,
                                        `tb_atividade_id`,
                                        `tb_status_apontamento_id`,
                                        `tipo_apontamento_id`,
                                        `tb_vigencia_id`,
                                        `tb_projeto_org_cod_projeto`,
                                        `tb_org_id`,
                                        `codigo_interno_colaborador`,
                                        `observacao`,
                                        `codigo_interno_colaborador_criacao`,
                                        `codigo_interno_colaborador_alteracao`,
                                        `codigo_interno_colaborador_justificativa`
                                    )
                                    VALUES (
                                        @Id,
                                        @Horas,
                                        @Justificativa,
                                        @Data,
                                        @NumeroSemana,
                                        @NumeroSemanaDia,
                                        @AtividadeId,
                                        @StatusApontamentoId,
                                        @TipoApontamento,
                                        @VigenciaId,
                                        @ProjetoCodigo,
                                        @OrgId,
                                        @ColaboradorCpf,
                                        @Observacao,
                                        @CpfRequest,
                                        @CpfRequest,
                                        @CodigoColaboradorJustificativa
                                    );";

                colaboradorApontamentoDTO.CpfRequest = cpfRequest;
                var linhasInseridas = await connection.ExecuteAsync(sql, colaboradorApontamentoDTO);

                if (linhasInseridas > 0)
                {
                    var apontamento = await GetApontamentoById(colaboradorApontamentoDTO.Id, idioma);
                    return apontamento;
                }

                return new ColaboradorApontamentoDTO();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir dados: " + ex.Message);
                throw;
            }
        }

        public async Task<List<StatusApontamentoResult>> ListarStatus(string idioma)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
                                        tsa.id AS id_status_apontamento,
                                        tsa.descricao AS descricao_apontamento,
                                        tsa.cod_status_apontamento,
                                        tsa.tb_cod_status_grupo AS cod_status_grupo,
                                        tsag.id AS id_status_apontamento_grupo,
                                        COALESCE(tt.traducao,tsag.descricao) as descricao_grupo
                                        -- tsag.descricao as descricao_grupo
                                    FROM
                                        tb_status_apontamento tsa
                                    JOIN
                                        tb_status_apontamento_grupo tsag ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
                                    LEFT JOIN
										tb_traducoes tt ON tsa.tb_cod_status_grupo = tt.valor_chave
										AND tt.tabela_chave = 'tb_status_apontamento_grupo.cod_status_grupo'
										AND tt.idioma = @Idioma
                                    ORDER BY
                                        cod_status_apontamento ASC;";

                var status = await connection.QueryAsync<StatusApontamentoResult>(sql, new { Idioma = idioma });

                return status.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VigenciaDTO>> ListarVigencia()
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
                                        *
                                    FROM
                                        tb_vigencia;";

                var vigenciaResult = await connection.QueryAsync<tb_vigencia>(sql);

                var vigencias = vigenciaResult.Select(x => new VigenciaDTO()
                {
                    Id = x.id.ToString(),
                    Mes = x.mes,
                    Ano = x.ano
                }).ToList();

                return vigencias;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ColaboradorApontamentoDTO> EditarApontamento(ColaboradorApontamentoDTO colaboradorApontamentoDTO, string idioma, string cpfRequest)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"UPDATE
                                `tb_colaborador_apontamento`
                            SET
                                `horas` = @Horas,
                                `justificativa` = @Justificativa,
                                `data` = @Data,
                                `numero_semana` = @NumeroSemana,
                                `numero_semana_dia` = @NumeroSemanaDia,
                                `tb_atividade_id` = @AtividadeId,
                                `tb_status_apontamento_id` = @StatusApontamentoId,
                                `tipo_apontamento_id` = @TipoApontamento,
                                `tb_vigencia_id` = @VigenciaId,
                                `tb_projeto_org_cod_projeto` = @ProjetoCodigo,
                                `tb_org_id` = @OrgId,
                                `codigo_interno_colaborador` = @ColaboradorCpf,
                                `observacao` = @Observacao,
                                `codigo_interno_colaborador_alteracao` = @CpfRequest,
                                `codigo_interno_colaborador_justificativa` = @CodigoColaboradorJustificativa
                            WHERE
                                `id` = @Id;";

                colaboradorApontamentoDTO.CpfRequest = cpfRequest;
                var linhasAtualizadas = await connection.ExecuteAsync(sql, colaboradorApontamentoDTO);

                if (linhasAtualizadas > 0)
                {
                    var apontamento = await GetApontamentoById(colaboradorApontamentoDTO.Id, idioma);
                    return apontamento;
                }

                return new ColaboradorApontamentoDTO();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InserirApontamentoLog(ColaboradorApontamentoLogDTO colaboradorApontamentoLogDTO)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"INSERT INTO `tb_colaborador_apontamento_log` (
                                        `id`,
                                        `data_criacao`,
                                        `colaborador_apontamento_id`,
                                        `tb_status_apontamento_anterior_id`,
                                        `tb_status_apontamento_novo_id`,
                                        `justificativa`,
                                        `horas_anterior`,
                                        `horas_novo`,
                                        `horas_reprovadas`,
                                        `tb_vigencia_id`,
                                        `tb_projeto_org_cod_projeto`,
                                        `tb_org_id`,
                                        `codigo_interno_colaborador`,
                                        `numero_semana`,
                                        `numero_semana_dia`,
                                        `tb_atividade_id`,
                                        `codigo_interno_colaborador_criacao`
                                    )
                                    VALUES (
                                        @Id,
                                        @DataCriacao,
                                        @ColaboradorApontamentoId,
                                        @TbStatusApontamentoAnteriorId,
                                        @TbStatusApontamentoNovoId,
                                        @Justificativa,
                                        @HorasAnterior,
                                        @HorasNovo,
                                        @HorasReprovadas,
                                        @TbVigenciaId,
                                        @TbProjetoOrgCodProjeto,
                                        @TbOrgId,
                                        @TbColaboradorOrgTbColaboradorCpf,
                                        @NumeroSemana,
                                        @NumeroSemanaDia,
                                        @TbAtividadeId,
                                        @TbColaboradorCpfCriacao
                                    );";

                var linhasInseridas = await connection.ExecuteAsync(sql, colaboradorApontamentoLogDTO);

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InserirLogsAprovacaoOuReprovacaoApontamentosGerenteDeProjeto(List<string> ids, string justificativa, int codStatus, string cpfRequest)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var horasReprovadas = codStatus == (int)EnumStatusApontamento.Aprovado ? "null AS horas_anterior" : "ca.horas AS horas_anterior";

                string sql = $@"INSERT INTO tb_colaborador_apontamento_log (
                                        id,
                                        data_criacao,
                                        colaborador_apontamento_id,
                                        tb_status_apontamento_anterior_id,
                                        tb_status_apontamento_novo_id,
                                        justificativa,
                                        horas_anterior,
                                        horas_novo,
                                        horas_reprovadas,
                                        tb_vigencia_id,
                                        tb_projeto_org_cod_projeto,
                                        tb_org_id,
                                        codigo_interno_colaborador,
                                        numero_semana,
                                        numero_semana_dia,
                                        tb_atividade_id,
                                        codigo_interno_colaborador_criacao
                                    )
                                    SELECT
                                        UUID(),
                                        NOW(),
                                        ca.id AS colaborador_apontamento_id,
                                        ca.tb_status_apontamento_id AS tb_status_apontamento_anterior_id,
                                        (SELECT id FROM tb_status_apontamento WHERE cod_status_apontamento = @CodStatus) AS tb_status_apontamento_novo_id,
                                        @Justificativa,
                                        ca.horas AS horas_anterior,
                                        null AS horas_novo,
                                        {horasReprovadas},
                                        ca.tb_vigencia_id,
                                        ca.tb_projeto_org_cod_projeto,
                                        ca.tb_org_id,
                                        ca.codigo_interno_colaborador,
                                        ca.numero_semana,
                                        ca.numero_semana_dia,
                                        ca.tb_atividade_id,
                                        @CpfCriacao
                                    FROM
                                        tb_colaborador_apontamento ca
                                    WHERE
                                        ca.id IN @Ids;";

                var linhasInseridas = await connection.ExecuteAsync(sql, new
                {
                    Ids = ids,
                    Justificativa = justificativa,
                    CodStatus = codStatus,
                    CpfCriacao = cpfRequest
                });

                return linhasInseridas > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProjetoGerenteResult>> ListarProjetosGerenteDeProjetos(string cpf, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT
                                        tpg.cod_projeto,
                                        tpo.projeto,
                                        tpo.permite_apont_sem_alocacao
                                    FROM
                                        tb_projeto_gerente tpg
                                    JOIN
                                        tb_colaborador_org tco ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tpg.tb_org_id = tco.tb_org_id
                                    JOIN
                                        tb_projeto_org tpo ON tpo.cod_projeto = tpg.cod_projeto and tpg.tb_org_id = tpo.tb_org_id
                                    WHERE
                                        tco.codigo_interno_colaborador = @Cpf
                                        AND tpg.tb_org_id = @OrgId
                                        AND tpg.tipo_gerente = 'GerenteProjeto'
                                    ORDER BY
                                        tpo.projeto;";

                var projetosGerente = await connection.QueryAsync<ProjetoGerenteResult>(sql, new
                {
                    Cpf = cpf,
                    OrgId = orgId
                });

                return projetosGerente.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ApontamentoReduzidoDTO>> GetIdsApontamentosPorCodStatusApontamento(List<string> ids, int codStatusApontamento, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"
                                    SELECT
                                        tca.id,
                                        tca.data,
                                        tca.codigo_interno_colaborador,
                                        tpo.cod_projeto,
                                        tpo.projeto,
										ta.descricao as atividade_descricao,
										tc.nome_completo as nome_completo_colaborador,
                                        tu.email as email_colaborador
                                    FROM
                                        tb_colaborador_apontamento tca
                                    JOIN
                                        tb_status_apontamento tsa ON tca.tb_status_apontamento_id = tsa.id
									JOIN
                                        tb_projeto_org tpo ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto and tca.tb_org_id = tpo.tb_org_id
                                    LEFT JOIN
                                        tb_atividade ta ON tca.tb_atividade_id = ta.id and tca.tb_org_id = ta.tb_org_id
                                    LEFT JOIN
                                        tb_colaborador tc ON tca.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                    LEFT JOIN
                                        tb_usuario tu ON tca.codigo_interno_colaborador = tu.codigo_interno_colaborador
                                    WHERE
                                        tca.id IN @Ids
                                    AND
                                        tsa.cod_status_apontamento = @CodStatusApontamento
                                    AND
                                        tca.tb_org_id = @OrgId
                                    GROUP BY 
                                        tca.id;";

                var result = await connection.QueryAsync<dynamic>(sql, new
                {
                    Ids = ids,
                    CodStatusApontamento = codStatusApontamento,
                    OrgId = orgId
                });

                var list = new List<ApontamentoReduzidoDTO>();

                foreach (var item in result)
                {
                    list.Add(new ApontamentoReduzidoDTO()
                    {
                        Id = item.id.ToString(),
                        Data = (DateTime)item.data,
                        Cpf = item.codigo_interno_colaborador,
                        CodProjeto = item.cod_projeto,
                        NomeProjeto = item.projeto,
                        AtividadeDescricao = item.atividade_descricao,
                        NomeCompletoColaborador = item.nome_completo_colaborador,
                        EmailColaborador = item.email_colaborador
                    });
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<(Guid id_apontamento, string cod_projeto, string cpf_gerente, int tb_org_id, string cod_gestor_hierarquico)>> GetCodProjetoECpfGerenteByIdsApontamentos(List<string> ids, string? cpfGestor = null)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"SELECT ca.id AS id_apontamento, pg.cod_projeto, co.codigo_interno_colaborador AS cpf_gerente, ca.tb_org_id, vwgha.cod_interno_gestor AS cod_gestor_hierarquico
                                    FROM
                                        tb_colaborador_apontamento ca
                                    JOIN
                                        tb_projeto_gerente pg ON ca.tb_projeto_org_cod_projeto = pg.cod_projeto and ca.tb_org_id = pg.tb_org_id
                                    JOIN
                                        tb_colaborador_org co ON pg.cod_colaborador_gerente = co.cod_colaborador_externo and ca.tb_org_id = co.tb_org_id
                                    LEFT JOIN (
                                        SELECT cod_interno_colaborador, cod_interno_gestor, tb_org_id
                                        FROM vw_gestor_hierarquico_aprovadores
                                        WHERE @CpfGestor is null OR @CpfGestor = '' OR cod_interno_gestor =  @CpfGestor
                                    ) AS vwgha ON vwgha.cod_interno_colaborador = ca.codigo_interno_colaborador AND vwgha.tb_org_id = ca.tb_org_id
                                    WHERE
                                        ca.id IN @IdsApontamentos
                                    GROUP BY 
                                        ca.id, 
                                        pg.cod_projeto, 
                                        co.codigo_interno_colaborador, 
                                        ca.tb_org_id, 
                                        vwgha.cod_interno_gestor;
                            ";

                var result = await connection.QueryAsync<(Guid id_apontamento, string cod_projeto, string cpf_gerente, int tb_org_id, string cod_gestor_hierarquico)>(sql, new
                {
                    IdsApontamentos = ids,
                    CpfGestor = cpfGestor
                });

                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(int mes, int ano, string codProjeto, string cpfGerenteDeProjeto, int orgId, List<string>? diretorias)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var inClauseUnidades = diretorias.BuildInClauseOrNull();
                string sql = $@"SELECT
                                         tc.nome_completo AS nome_colaborador,
                                         vam.codigo_interno_colaborador_gerente,
                                         vam.codigo_interno_colaborador AS codigo_interno_colaborador,
                                         tco.cod_colaborador_externo as codigo_colaborador,
                                         tco.ativo,
                                         vam.tb_org_id
                                    FROM
                                         vw_apontamento_mensal vam
                                         JOIN tb_colaborador tc ON vam.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                         LEFT JOIN tb_colaborador_org tco ON vam.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                                                                AND vam.tb_org_id = tco.tb_org_id
                                         LEFT JOIN vw_gestor_hierarquico_aprovadores vwha ON vwha.tb_org_id = vam.tb_org_id
                                                                                AND vwha.cod_interno_colaborador = vam.codigo_interno_colaborador
                                    WHERE
                                        (
                                            vam.codigo_interno_colaborador_gerente = @CpfGerenteDeProjeto
                                            OR vwha.cod_interno_gestor =  @CpfGerenteDeProjeto
                                        )
                                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                                        AND vam.tb_org_id = @OrgId";

                if (codProjeto.EhStringValidaEDiferenteDeZero())
                {
                    sql += $@"
                            AND vam.cod_projeto = @CodProjeto";
                }

                if (mes > 0 && ano > 0)
                {
                    sql += $@"
                            AND vam.mes = @Mes AND vam.ano = @Ano";
                }

                sql += @"
							GROUP BY
                                         tc.nome_completo, vam.codigo_interno_colaborador, tco.cod_colaborador_externo, tco.ativo, vam.tb_org_id
                            ORDER BY
                                 tc.nome_completo;";

                var projetosColaboradoresVinculados = await connection.QueryAsync<dynamic>(sql, new
                {
                    CpfGerenteDeProjeto = cpfGerenteDeProjeto,
                    CodProjeto = codProjeto,
                    Mes = mes,
                    Ano = ano,
                    OrgId = orgId,
                });

                return projetosColaboradoresVinculados.Select(x => new ColaboradoresVinculadosGerenteDTO
                {
                    ColaboradorCpf = x.codigo_interno_colaborador,
                    NomeColaborador = x.nome_colaborador,
                    OrgId = x.tb_org_id
                });
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao listar colaboradores (com apontamentos) vinculados ao gerente de projeto .", e);
            }
        }

        public async Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(string cpf, int orgId, List<string>? diretorias)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var inClauseUnidades = diretorias.BuildInClauseOrNull();
                
                string sql = $@"SELECT DISTINCT
                                        vgco.codigo_interno_colaborador_gestor,
                                        vgco.nome_completo_gestor,
                                        vgco.tb_org_id
                                    FROM
                                        tb_colaborador_org tco
                                    JOIN
                                        tb_projeto_gerente tpg
                                        ON tpg.cod_colaborador_gerente = tco.cod_colaborador_externo
                                        AND tpg.tb_org_id = tco.tb_org_id
                                    JOIN
                                        tb_colaborador_apontamento tca
                                        ON tpg.cod_projeto = tca.tb_projeto_org_cod_projeto
                                        AND tca.tb_org_id = tpg.tb_org_id
                                    JOIN
                                        vw_gestores_colaboradores_org vgco
                                        ON vgco.codigo_interno_colaborador_subordinado = tca.codigo_interno_colaborador
                                        AND vgco.tb_org_id = tca.tb_org_id
                                    WHERE
                                        tco.codigo_interno_colaborador = @Cpf
                                        AND tco.tb_org_id = @OrgId
                                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                                    ORDER BY
                                        nome_completo_gestor;";

                var gerentesAdm = await connection.QueryAsync<dynamic>(sql, new
                {
                    Cpf = cpf,
                    OrgId = orgId
                });

                return gerentesAdm.Select(x => new ColaboradoresVinculadosGerenteDTO
                {
                    ColaboradorCpf = x.codigo_interno_colaborador_gestor,
                    NomeColaborador = x.nome_completo_gestor,
                    OrgId = x.tb_org_id
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>> ListarProjetosVisaoGerenteDeProjeto(string cpf, int orgId, string codProjeto, int mes, int ano, string cpfColaborador, int codStatusGrupo, string cpfGerenteAdm, List<string>? diretorias)
        {
            var ret = new ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>();
            var connection = _dapperConnection.GetConnection();
            try
            {
                
                var inClauseUnidades = diretorias.BuildInClauseOrNull();
                
                string sql = $@"SELECT
                                         vam.mes,
                                         vam.ano,
                                         vam.cod_projeto,
                                         vam.nome_projeto,
                                         vam.data_fim_projeto,
                                         vam.codigo_interno_colaborador AS codigo_interno_colaborador,
                                         tc.nome_completo AS nome_colaborador,
                                         vgco.codigo_interno_colaborador_subordinado AS codigo_interno_colaborador_gestor_adm,
                                         vgco.nome_completo_gestor AS nome_gestor_adm,
                                         vam.tb_org_id AS org_id,
                                         vam.cod_status_mensal,
                                         vam.descricao_status_mensal,
                                         sum(vam.total_horas) AS total_horas,
                                         vam.nome_cliente,
                                         vam.cod_cliente,
                                         tco.ativo,
                                         tco.data_admissao,
                                         tco.data_inativacao
                                    FROM
                                         vw_apontamento_mensal vam
                                         JOIN tb_colaborador tc ON vam.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                         LEFT JOIN vw_gestores_colaboradores_org vgco ON vam.codigo_interno_colaborador = vgco.codigo_interno_colaborador_subordinado
                                                                                AND vam.tb_org_id = vgco.tb_org_id
                                         LEFT JOIN tb_colaborador_org tco ON vam.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                                                                AND vam.tb_org_id = tco.tb_org_id
                                         LEFT JOIN vw_gestor_hierarquico_aprovadores vwha ON vwha.tb_org_id = vam.tb_org_id
                                                                                AND vwha.cod_interno_colaborador = tc.codigo_interno_colaborador
                                         LEFT JOIN tb_modelo_contratacao_org tmco ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao
                                                                                AND tmco.tb_org_id = tco.tb_org_id
                                    WHERE
                                        (
                                            vam.codigo_interno_colaborador_gerente = @Cpf
                                            OR vwha.cod_interno_gestor = @Cpf
                                        )
                                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                                         AND vam.tb_org_id = @OrgId
                                         AND vam.mes = @Mes
                                         AND vam.ano = @Ano
                                         AND (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)";
                if (!string.IsNullOrEmpty(codProjeto))
                {
                    sql += $@"
                            AND vam.cod_projeto =  @CodProjeto";
                }

                if (!string.IsNullOrEmpty(cpfColaborador))
                {
                    sql += $@"
                             AND vam.codigo_interno_colaborador = @CpfColaborador";
                }

                if (!string.IsNullOrEmpty(cpfGerenteAdm))
                {
                    sql += $@"
                             AND vgco.codigo_interno_colaborador_gestor = @CpfGerenteAdm";
                }

                if (codStatusGrupo > 0)
                {
                    sql += $@"
                             AND vam.cod_status_mensal = @CodStatusMensal";
                }
                sql += @"
                    GROUP BY
					     vam.mes,
                         vam.ano,
                         vam.cod_projeto,
                         vam.nome_projeto,
                         vam.data_fim_projeto,
                         vam.codigo_interno_colaborador,
                         tc.nome_completo,
                         vgco.codigo_interno_colaborador_subordinado,
                         vgco.nome_completo_gestor,
                         vam.tb_org_id,
                         vam.cod_status_mensal,
                         vam.descricao_status_mensal,
                         vam.nome_cliente,
                         vam.cod_cliente,
                         tco.ativo,
                         tco.data_admissao,
                         tco.data_inativacao,
                         vam.codigo_interno_colaborador_gerente,
	                     vam.gerente,
	                     vam.cod_gerente";

                var queryParams = new
                {
                    Cpf = cpf,
                    OrgId = orgId,
                    CodProjeto = codProjeto,
                    Mes = mes,
                    Ano = ano,
                    CpfColaborador = cpfColaborador,
                    CodStatusMensal = codStatusGrupo,
                    CpfGerenteAdm = cpfGerenteAdm
                };

                var projetosGerente = await connection.QueryAsync<dynamic>(sql, queryParams);

                ret.Lista = projetosGerente.Select(x => new ProjetoVisaoGerenteDTO
                {
                    Mes = x.mes,
                    Ano = x.ano,
                    NomeProjeto = x.nome_projeto,
                    CodProjeto = x.cod_projeto,
                    DataFimProjeto = x.data_fim_projeto?.ToString("dd/MM/yyyy"),
                    CpfColaborador = x.codigo_interno_colaborador,
                    Ativo = x.ativo == 1,
                    DataInativacao = x.data_inativacao?.ToString("dd/MM/yyyy"),
                    DataAdmissao = x.data_admissao?.ToString("dd/MM/yyyy"),
                    NomeColaborador = x.nome_colaborador,
                    NomeGestorAdm = x.nome_gestor_adm,
                    OrgId = x.org_id,
                    CodStatusMensal = x.cod_status_mensal,
                    DescricaoStatusMensal = x.descricao_status_mensal,
                    TotalHoras = (int)x.total_horas,
                    CodCliente = x.cod_cliente,
                    NomeCliente = x.nome_cliente
                }).OrderBy(x => x.NomeColaborador)
                    .DistinctBy(x => (x.CodProjeto, x.CpfColaborador,  x.CodCliente, x.DescricaoStatusMensal));


                ret.Totalizador = new TotalizadorApontamentosDTO
                {
                    QuantidadeTotalColaboradores = ret.Lista.Select(x => x.CpfColaborador).Distinct().Count(),
                    QuantidadeTotalProjetos = ret.Lista.Select(x => x.CodProjeto).Distinct().Count(),
                    SomaHoras = ret.Lista.Sum(x => x.TotalHoras)
                };

                var somaHorasAprovadas = ret.Lista
                    .Where(x => x.CodStatusMensal == 2)
                    .Sum(x => x.TotalHoras);

                var somaHorasPendentes = ret.Lista
                    .Where(x => x.CodStatusMensal == 1)
                    .Sum(x => x.TotalHoras);

                var somaHorasReprovadas = ret.Lista
                    .Where(x => x.CodStatusMensal == 3)
                    .Sum(x => x.TotalHoras);
                
                var quantidadeTotalColaboradores = await CalcularQuantidadeTotal(cpf, codProjeto, orgId);
                var quantidadeApontaram = ret.Lista.Select(x => x.CpfColaborador).Distinct().Count();

                ret.TotalizadorBigNumbers = new TotalizadorApontamentosBigNumbersDTO
                {
                    QuantidadeTotalColaboradores = quantidadeTotalColaboradores.ToIntOuZero(),
                    SomaHorasLancadas = (somaHorasAprovadas + somaHorasPendentes).ToDecimalOuZero(),
                    SomaHorasAprovadas = somaHorasAprovadas.ToDecimalOuZero(),
                    SomaHorasPendentes = somaHorasPendentes.ToDecimalOuZero(),
                    SomaHorasReprovdas = somaHorasReprovadas.ToDecimalOuZero(),
                    QuantidadeTotalNaoApontado = $"{quantidadeTotalColaboradores - quantidadeApontaram}/{quantidadeTotalColaboradores}"
                };

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> CalcularQuantidadeTotal(string cpf, string codProjeto, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                if (!string.IsNullOrEmpty(codProjeto))
                {
                    var sqlPermiteApontamento = $@" SELECT
                                                        permite_apont_sem_alocacao
                                                    FROM
                                                        tb_projeto_org
                                                    WHERE
                                                        tb_org_id = @OrgId and cod_projeto = @CodProjeto";

                    var permiteApontamento = await connection.QueryFirstOrDefaultAsync<int>(sqlPermiteApontamento, new { OrgId = orgId, CodProjeto = codProjeto });

                    var sql = "";
                    if (permiteApontamento == 1)
                    {
                        // Contar colaboradores da organização
                        sql = $@"SELECT
                                    COUNT(*) AS total_associados
                                 FROM
                                    tb_colaborador_org
                                 WHERE
                                    tb_org_id = @OrgId";
                    }
                    else
                    {
                        // Contar colaboradores associados ao projeto
                        sql = $@"SELECT
                                    COUNT(DISTINCT cod_colaborador) AS total_associados
                                  FROM
                                    tb_colaborador_projeto_org
                                  WHERE
                                    cod_projeto = @CodProjeto AND tb_org_id = @OrgId";
                    }

                    var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { CodProjeto = codProjeto, OrgId = orgId });
                    return result != null ? (int)result.total_associados : 0;
                }
                else
                {
                    // Contar colaboradores da organização
                    var sqlColaboradores = $@" SELECT
                                                   COUNT(*) AS total_colaboradores
                                               FROM
                                                   tb_colaborador_org
                                               WHERE
                                                   tb_org_id = @OrgId";

                    var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sqlColaboradores, new { OrgId = orgId });
                    return result != null ? (int)result.total_colaboradores : 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FeriadoDTO> ListarFeriados(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = $@"    SELECT
                                            *
                                        FROM
                                            tb_feriado
                                        WHERE
                                            ativo = 1
                                            AND tb_org_id = @OrgId;";

                var feriado = connection.Query<tb_feriado>(sql,
                new
                {
                    OrgId = orgId
                });

                return feriado.Select(x => new FeriadoDTO()
                {
                    Nome = x.nome,
                    Data = x.data,
                    Tipo = (TipoFeriado)Enum.Parse(typeof(TipoFeriado), x.tipo)
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
      
        public async Task<dynamic> RelatorioApontamento(int? mes, int? ano, int orgId, string cpfGerente, List<string>? diretorias)
        {
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync("SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            var query = $@"
                SELECT
                    co.cod_colaborador_externo AS CodColaborador,
                    c.nome_completo AS Colaborador,
                    po.cod_cliente AS CodCliente,
                    tclo.nome_cliente AS Cliente,
                    po.cod_projeto AS CodProjeto,
                    po.projeto AS Projeto,
                    po.status AS `Status do Projeto`,
                    co.departamento AS Departamento,
                    atv.descricao AS Atividade,
                    sag.descricao AS StatusAprovacao,
                    tco_modificador.cod_colaborador_externo AS CodigoAprovador,
                    CASE 
                        WHEN sa.tb_cod_status_grupo IN (1, 4) THEN (
                            SELECT GROUP_CONCAT(tc.nome_completo SEPARATOR ', ')
                            FROM tb_projeto_gerente tpg2
                            JOIN tb_colaborador_org tco2
                                ON tco2.cod_colaborador_externo = tpg2.cod_colaborador_gerente
                               AND tco2.tb_org_id = tpg2.tb_org_id
                            JOIN tb_colaborador tc
                                ON tco2.codigo_interno_colaborador = tc.codigo_interno_colaborador
                            WHERE tpg2.cod_projeto = po.cod_projeto
                              AND tpg2.tb_org_id = ca.tb_org_id
                        )
                        ELSE c_modificador.nome_completo
                    END AS Aprovador,
                    ca.justificativa AS Justificativa,
                    -- ✅ HORAS SEM DUPLICAÇÃO
                    REPLACE(
                        CONVERT(ROUND(SUM(ca.horas) / 60, 2), CHAR),
                        '.', ','
                    ) AS Horas,
                    CONCAT(@Ano, '-', LPAD(@Mes, 2, '0'), '-', LPAD(WEEK(ca.data) + 1, 2, '0')) AS Semana,
                    DATE_FORMAT(ca.data, '%d/%m/%Y') AS Data,
                    ca.observacao AS `Resumo das atividades`,
                    co.modelo_contratacao AS ModeloContratacao,
                    co.empresa_relacionada AS EmpresaRelacionada,
                    c.contato_principal AS ContatoPrincipal
                FROM tb_colaborador_apontamento ca
                JOIN tb_colaborador c
                    ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
                JOIN tb_colaborador_org co
                    ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador
                   AND ca.tb_org_id = co.tb_org_id
                JOIN tb_projeto_org po
                    ON ca.tb_projeto_org_cod_projeto = po.cod_projeto
                   AND ca.tb_org_id = po.tb_org_id
                LEFT JOIN tb_cliente_org tclo
                    ON po.cod_cliente = tclo.codigo_cliente
                   AND po.tb_org_id = tclo.tb_org_id
                JOIN tb_atividade atv
                    ON ca.tb_atividade_id = atv.id
                JOIN tb_status_apontamento sa
                    ON ca.tb_status_apontamento_id = sa.id
                JOIN tb_status_apontamento_grupo sag
                    ON sag.cod_status_grupo = sa.tb_cod_status_grupo
                LEFT JOIN tb_colaborador c_modificador
                    ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
                LEFT JOIN tb_colaborador_org tco_modificador
                    ON tco_modificador.codigo_interno_colaborador = c_modificador.codigo_interno_colaborador
                   AND tco_modificador.tb_org_id = ca.tb_org_id
                INNER JOIN tb_modelo_contratacao_org tmco
                    ON tmco.codigo_modelo_contratacao = co.codigo_modelo_contratacao
                   AND tmco.tb_org_id = co.tb_org_id
                WHERE ca.tb_org_id = @OrgId
                  AND ca.data >= STR_TO_DATE(CONCAT(@Ano, '-', LPAD(@Mes, 2, '0'), '-01'), '%Y-%m-%d')
                  AND ca.data <  DATE_ADD(
                                    STR_TO_DATE(CONCAT(@Ano, '-', LPAD(@Mes, 2, '0'), '-01'), '%Y-%m-%d'),
                                    INTERVAL 1 MONTH
                                )
                  AND (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)
                  -- ✅ FILTRO DE GERENTE SEM JOIN
                  AND (
                        @CodigoInternoGerente IS NULL
                        OR EXISTS (
                            SELECT 1
                            FROM tb_projeto_gerente tpg
                            JOIN tb_colaborador_org tpog
                                ON tpog.cod_colaborador_externo = tpg.cod_colaborador_gerente
                               AND tpog.tb_org_id = tpg.tb_org_id
                            WHERE tpg.cod_projeto = po.cod_projeto
                              AND tpg.tb_org_id = ca.tb_org_id
                              AND tpog.codigo_interno_colaborador = @CodigoInternoGerente
                        )
                      )
                  {(inClauseUnidades == null ? "" : $"AND co.cod_diretoria IN {inClauseUnidades}")}
                GROUP BY
                    co.cod_colaborador_externo,
                    c.nome_completo,
                    po.cod_cliente,
                    tclo.nome_cliente,
                    po.cod_projeto,
                    po.projeto,
                    po.status,
                    co.departamento,
                    atv.descricao,
                    sag.descricao,
                    tco_modificador.cod_colaborador_externo,
                    Aprovador,
                    ca.justificativa,
                    ca.data,
                    ca.observacao,
                    co.modelo_contratacao,
                    co.empresa_relacionada,
                    c.contato_principal
                ORDER BY SUM(ca.horas) DESC;

            ";
            
            var parametros = new 
            {
                Mes = mes,
                Ano = ano,
                OrgId = orgId,
                CodigoInternoGerente = cpfGerente
            };
            
            var resultado = await connection.QueryAsync<dynamic>(query, parametros, commandTimeout: 120);
            return resultado.ToList();
        }

        public async Task<dynamic> RelatorioApontamentoSimplificado(int? mes, int? ano, int orgId, List<string>? diretorias, bool considerarApenasAtivos = false)
        {
            var connection = _dapperConnection.GetConnection();

            await connection.ExecuteAsync("SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");

            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            var query = $@"
                SELECT DISTINCT
                    CONCAT(LOWER(CONVERT(fn_get_dia_semana(tca.data), CHAR)), ' ', CONVERT(DATE_FORMAT(tca.data, '%d/%m'), CHAR)) AS Data,
                    tc.nome_completo AS Nome,
                    tco.cod_colaborador_externo AS Matrícula,
                    ta.descricao AS Tarefa,
                    CASE
                        WHEN tpo.projeto IS NULL OR tpo.projeto = '-' THEN tpo.cod_projeto
                        ELSE CONCAT(tpo.cod_projeto, ' - ', tpo.projeto)
                    END AS Projeto,
                    TIME_FORMAT(SEC_TO_TIME(tca.horas * 60), '%H:%i') AS Horas,
                    tca.observacao AS `Resumo das atividades`,
                    tc_aprovador.nome_completo AS Aprovador,
                    REPLACE(CONVERT(ROUND((tca.horas / 60), 2), CHAR), '.', ',') AS `Hora (Decimal)`,
                    tsag.descricao AS Status
                FROM
                    tb_colaborador_apontamento tca
                    JOIN tb_colaborador tc
                        ON tca.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    JOIN tb_colaborador_org tco
                        ON tca.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        AND tca.tb_org_id = tco.tb_org_id
                    INNER JOIN tb_modelo_contratacao_org tmco
                        ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao
                        AND tmco.tb_org_id = tco.tb_org_id
                    JOIN tb_projeto_org tpo
                        ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto
                        AND tca.tb_org_id = tpo.tb_org_id
                    JOIN tb_atividade ta
                        ON tca.tb_atividade_id = ta.id
                    JOIN tb_status_apontamento tsa
                        ON tca.tb_status_apontamento_id = tsa.id
                    JOIN tb_status_apontamento_grupo tsag
                        ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
                    LEFT JOIN tb_colaborador tc_aprovador
                        ON tca.codigo_interno_colaborador_justificativa = tc_aprovador.codigo_interno_colaborador
                        AND tsa.tb_cod_status_grupo = 2 -- aprovado
                WHERE
                    tca.tb_org_id = @OrgId
                    {(considerarApenasAtivos  ? "AND tco.ativo = 1" : "")}
                    AND (tsa.tb_cod_status_grupo = 1 OR tsa.tb_cod_status_grupo = 2)
                    AND (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)
                    AND MONTH(tca.data) = @Mes
                    AND YEAR(tca.data) = @Ano
                    {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                ORDER BY
                    tca.data,
                    tc.nome_completo;
            ";
            
            var result = await connection.QueryAsync<dynamic>(query, new
            {
                Mes = mes,
                Ano = ano,
                OrgId = orgId
            }, commandTimeout: 120);

            return result.ToList();
        }
        
        public async Task<IEnumerable<RelatorioApontamentoDTO>> ListarApontamentoRelatorioBIAsync(int orgId, string dataInicial)
        {
            var connection = _dapperConnection.GetConnection();

            await connection.ExecuteAsync("SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");

            var query = @"
                SELECT
                co.cod_colaborador_externo AS CodColaborador,
                    c.nome_completo AS Colaborador,
                    po.cod_cliente AS CodCliente,
                    tclo.nome_cliente AS Cliente,
                    po.cod_projeto AS CodProjeto,
                    po.projeto AS Projeto,
                    po.status AS StatusProjeto,
                    co.departamento AS Departamento,
                    atv.descricao AS Atividade,
                    sag.descricao AS StatusAprovacao,
                    tco_modificador.cod_colaborador_externo AS CodigoAprovador,
                    CASE
                    WHEN sa.tb_cod_status_grupo IN (2, 3, 4)
                        THEN c_modificador.nome_completo
                        ELSE ''
                    END AS Aprovador,
                    ca.justificativa AS Justificativa,
                    ROUND((ca.horas / 60), 4) AS Horas,
                    CONCAT(YEAR(ca.data), '-', (LPAD(WEEK(ca.data), 2, '0') + 1)) AS Semana,
                    DATE_FORMAT(ca.data, '%d/%m/%Y') AS Data,
                    ca.observacao AS Observacao,
                    co.modelo_contratacao AS ModeloContratacao,
                    co.empresa_relacionada AS EmpresaRelacionada,
                    c.contato_principal AS ContatoPrincipal
                FROM tb_colaborador_apontamento ca
                JOIN tb_colaborador c
                    ON ca.codigo_interno_colaborador = c.codigo_interno_colaborador
                JOIN tb_colaborador_org co
                    ON ca.codigo_interno_colaborador = co.codigo_interno_colaborador
                   AND ca.tb_org_id = co.tb_org_id
                INNER JOIN tb_modelo_contratacao_org tmco
                            ON tmco.codigo_modelo_contratacao = co.codigo_modelo_contratacao
                            AND tmco.tb_org_id = co.tb_org_id
                JOIN tb_projeto_org po
                    ON ca.tb_projeto_org_cod_projeto = po.cod_projeto
                   AND ca.tb_org_id = po.tb_org_id
                LEFT JOIN tb_cliente_org tclo
                    ON po.cod_cliente = tclo.codigo_cliente
                   AND po.tb_org_id = tclo.tb_org_id
                JOIN tb_atividade atv
                    ON ca.tb_atividade_id = atv.id
                JOIN tb_status_apontamento sa
                    ON ca.tb_status_apontamento_id = sa.id
                JOIN tb_status_apontamento_grupo sag
                    ON sag.cod_status_grupo = sa.tb_cod_status_grupo
                LEFT JOIN tb_colaborador c_modificador
                    ON ca.codigo_interno_colaborador_alteracao = c_modificador.codigo_interno_colaborador
                LEFT JOIN tb_colaborador_org tco_modificador
                    ON tco_modificador.codigo_interno_colaborador = c_modificador.codigo_interno_colaborador
                   AND tco_modificador.tb_org_id = ca.tb_org_id
                WHERE ca.tb_org_id = @OrgId
                  AND (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)
                  AND ca.data >= @DateFrom
                ORDER BY ca.data ASC;
                ";

                var parametros = new
                {
                    OrgId = orgId,
                    DateFrom = dataInicial
                };

                var result = await connection.QueryAsync<RelatorioApontamentoDTO>(query, parametros, commandTimeout: 120);

                var listarApontamentoRelatorioBiAsync = result as RelatorioApontamentoDTO[] ?? result.ToArray();
                var projetosIds = listarApontamentoRelatorioBiAsync.Select(x => x.CodProjeto).Distinct().ToList();

                var aprovadoresQuery = @"
                    SELECT
                        tpo.cod_projeto AS CodProjeto,
                        GROUP_CONCAT(tc.nome_completo SEPARATOR ', ') AS Aprovadores
                    FROM tb_projeto_org tpo
                    INNER JOIN tb_projeto_gerente tpg
                        ON tpg.cod_projeto = tpo.cod_projeto
                        AND tpg.tipo_gerente = 'GerenteProjeto'
                    INNER JOIN tb_colaborador_org tco
                        ON tco.cod_colaborador_externo = tpg.cod_colaborador_gerente
                        AND tco.tb_org_id = tpg.tb_org_id
                        AND tco.ativo = 1
                    INNER JOIN tb_colaborador tc
                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    WHERE tpo.cod_projeto IN @ProjetoIds
                      AND tpo.tb_org_id = @OrgId
                    GROUP BY
                        tpo.cod_projeto;
                ";
                var aprovadoresPorListaDeProjetosResult = await connection.QueryAsync<ProjetoAprovadoresRelatorioBIDTO>(
                aprovadoresQuery,
                new
                {
                    ProjetoIds = projetosIds,
                    OrgId = orgId
                },
                commandTimeout: 120);

            var aprovadoresDict = aprovadoresPorListaDeProjetosResult
                .ToDictionary(x => x.CodProjeto, x => x.Aprovadores);

            foreach (var item in listarApontamentoRelatorioBiAsync)
            {
                if (aprovadoresDict.TryGetValue(item.CodProjeto, out var aprovadores))
                {
                    item.Aprovadores = aprovadores;
                }
            }

            return listarApontamentoRelatorioBiAsync;
        }

        public async Task<List<RelatorioApontamentoDTO>> ListarApontamentoRecenteRelatorioBI(int orgId)
        {
            return (await ListarApontamentoRelatorioBIAsync(orgId, DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"))) .ToList();
        }

        public async Task<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>> ListarColaboradoresEApontamentosPorGestor(
            string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, int orgId,
            string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, List<string>? diretorias, bool considerarApenasAtivos = false)
        {
            var ret = new ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>();
            var connection = _dapperConnection.GetConnection();
            
            var inClauseUnidades = diretorias.BuildInClauseOrNull();
            
            try
            {

                var query = $@"
                    DROP TEMPORARY TABLE IF EXISTS temp_result_filtrada;
                    DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_filtrado;

                    -- =============================================================
                    -- 1️⃣ TEMPORARY TABLE: RESULTADO PRINCIPAL (colaboradores + apontamentos)
                    -- =============================================================

                    CREATE TEMPORARY TABLE temp_result_filtrada AS
                    SELECT
                        tc.nome_completo AS colaborador_nome,
                        tc.codigo_interno_colaborador,
                        tco.ativo,
                        tco.data_admissao,
                        tco.data_inativacao,
                        tcg.nome_completo AS nome_completo_gerente,
                        tch.cod_colaborador_superior AS codigo_gerente,
                        CASE
                            WHEN tsa.tb_cod_status_grupo IS NULL THEN 5
                            WHEN SUM(CASE WHEN tsa.tb_cod_status_grupo = 3 THEN 1 ELSE 0 END) > 0 THEN 3
                            WHEN SUM(CASE WHEN tsa.tb_cod_status_grupo = 2 THEN 1 ELSE 0 END) = COUNT(0) THEN 2
                            ELSE 1
                        END AS cod_status_apontamento_periodo,
                        tsag.descricao AS descricao_status_apontamento,
                        COALESCE(SUM(tca.horas), 0) AS soma_horas,
                        tco.tb_org_id,
                        COALESCE(tv.mes, @Mes) AS mes,
                        COALESCE(tv.ano, @Ano) AS ano,
                        CASE
                            WHEN tpo.projeto IS NULL OR tpo.projeto = '-' THEN tpo.cod_projeto
                            ELSE CONCAT(tpo.cod_projeto, ' - ', tpo.projeto)
                        END AS projeto,
                        tpo.cod_projeto,
                       CASE 
                            WHEN tsa.tb_cod_status_grupo IN (2, 3, 4)
                            THEN tca.codigo_interno_colaborador_alteracao
                            ELSE NULL
                        END AS codigo_interno_aprovador
                    FROM tb_colaborador tc
                    JOIN tb_colaborador_org tco
                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN tb_modelo_contratacao_org tmco
                        ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao
                        AND tmco.tb_org_id = tco.tb_org_id
                    LEFT JOIN tb_colaborador_hierarquia tch
                        ON tco.cod_colaborador_externo = tch.cod_colaborador_externo
                        AND tch.tb_org_id = tco.tb_org_id
                    LEFT JOIN tb_colaborador_org tcog
                        ON tcog.cod_colaborador_externo = tch.cod_colaborador_superior
                        AND tco.tb_org_id = tcog.tb_org_id
                    LEFT JOIN tb_colaborador tcg
                        ON tcg.codigo_interno_colaborador = tcog.codigo_interno_colaborador
                    LEFT JOIN tb_colaborador_apontamento tca
                        ON tc.codigo_interno_colaborador = tca.codigo_interno_colaborador
                        AND tco.tb_org_id = tca.tb_org_id
                        AND tca.tb_vigencia_id = (SELECT id FROM tb_vigencia WHERE mes = @Mes AND ano = @Ano)
                    LEFT JOIN tb_status_apontamento tsa
                        ON tca.tb_status_apontamento_id = tsa.id
                    LEFT JOIN tb_status_apontamento_grupo tsag
                        ON tsa.tb_cod_status_grupo = tsag.cod_status_grupo
                    LEFT JOIN tb_vigencia tv
                        ON tca.tb_vigencia_id = tv.id
                    LEFT JOIN tb_projeto_org tpo
                        ON tca.tb_projeto_org_cod_projeto = tpo.cod_projeto
                        AND tco.tb_org_id = tpo.tb_org_id
                    WHERE
                        tco.tb_org_id = @TbOrgId
                        {(considerarApenasAtivos ? "AND tco.ativo = 1" : "")}
                        AND (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)
                        AND (COALESCE(@CodGerente, '0') = '0' OR tch.cod_colaborador_superior = @CodGerente)
                        AND (
                            COALESCE(@CodColaboradorExternoAprovador, '0') = '0'
                            OR EXISTS (
                                SELECT 1
                                FROM tb_projeto_gerente pg
                                WHERE pg.cod_projeto = tpo.cod_projeto
                                  AND pg.cod_colaborador_gerente = @CodColaboradorExternoAprovador
                            )
                        )
                        AND (COALESCE(@Nome, '') = '' OR tc.nome_completo LIKE CONCAT('%', @Nome, '%'))
                        AND (COALESCE(@CodProjeto, '0') = '0' OR tpo.cod_projeto = @CodProjeto)
                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                        AND (
                            -- Colaborador ativo
			                (
			                    tco.ativo = 1
			                    AND (
			                        YEAR(tco.data_admissao) < @Ano
			                        OR (YEAR(tco.data_admissao) = @Ano AND MONTH(tco.data_admissao) <= @Mes)
			                    )
			                )
			                OR
                            -- Colaborador inativo
			                (
			                    tco.ativo = 0
			                    AND (
			                        YEAR(tco.data_admissao) <= @Ano
			                        OR (YEAR(tco.data_admissao) = @Ano AND MONTH(tco.data_admissao) >= @Mes)
			                    )
			                    AND (
			                        YEAR(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) > @Ano
			                        OR (
			                            YEAR(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) = @Ano
			                            AND MONTH(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) >= @Mes
			                        )
			                    )
			                )
			            )	
                    GROUP BY
                        tc.nome_completo,
                        tc.codigo_interno_colaborador,
                        tco.ativo,
                        tco.data_admissao,
                        tco.data_inativacao,
                        tcg.nome_completo,
                        tch.cod_colaborador_superior,
                        tco.tb_org_id,
                        tv.mes,
                        tv.ano,
                        tpo.cod_projeto,
                        tpo.projeto,
                        COALESCE(tca.codigo_interno_colaborador_alteracao, tca.codigo_interno_colaborador_criacao),
                        tsag.descricao
                    ORDER BY tc.nome_completo;
                    
                    DELETE FROM temp_result_filtrada
                    WHERE @CodStatusApontamento IS NOT NULL
                      AND @CodStatusApontamento <> 0
                      AND cod_status_apontamento_periodo <> @CodStatusApontamento;

                    -- =============================================================
                    -- 2️⃣ TEMPORARY TABLE: APROVADORES
                    -- =============================================================

                    CREATE TEMPORARY TABLE temp_aprovadores_filtrado AS
                    SELECT
                        DISTINCT tr.codigo_interno_colaborador,
                        tr.tb_org_id,
                        tr.mes,
                        tr.ano,
                        tr.cod_projeto,
                        tr.cod_status_apontamento_periodo,
                        tr.codigo_interno_aprovador AS codigo_interno_aprovador,
                        tca.nome_completo AS nome_aprovador
                    FROM temp_result_filtrada tr
                    JOIN tb_colaborador tca
                        ON tr.codigo_interno_aprovador = tca.codigo_interno_colaborador
                    WHERE
                        COALESCE(@CodColaboradorExternoAprovador, '0') = '0'
                        OR tr.codigo_interno_aprovador = (
                            SELECT codigo_interno_colaborador
                            FROM tb_colaborador_org
                            WHERE cod_colaborador_externo = @CodColaboradorExternoAprovador
                              AND tb_org_id = @TbOrgId
                        );

                    -- =============================================================
                    -- 3️⃣ RETORNOS: MÚLTIPLOS RESULTSETS
                    -- =============================================================

                    -- 1️⃣ Resultado principal: colaboradores e apontamentos
                    SELECT * FROM temp_result_filtrada
                    LIMIT @Limite OFFSET @Offset;

                    -- 2️⃣ Lista de aprovadores
                    SELECT * FROM temp_aprovadores_filtrado;

                    -- 3️⃣ Totalizador padrão (total + soma_horas)
                    SELECT
                        COUNT(DISTINCT codigo_interno_colaborador) AS total,
                        SUM(soma_horas) AS soma_horas
                    FROM temp_result_filtrada;

                    -- 4️⃣ Totalizador big numbers
                    SELECT
                        COUNT(DISTINCT codigo_interno_colaborador) AS quantidade_total_colaboradores,

                        SUM(CASE 
                            WHEN cod_status_apontamento_periodo IN (1,2,3) THEN soma_horas 
                            ELSE 0 
                        END) AS soma_horas_lancadas,

                        COUNT(DISTINCT CASE 
                            WHEN cod_status_apontamento_periodo = 5                          
                            THEN codigo_interno_colaborador 
                        END) AS quantidade_total_nao_apontado,

                        SUM(CASE 
                            WHEN cod_status_apontamento_periodo = 2 THEN soma_horas 
                            ELSE 0 
                        END) AS soma_horas_aprovadas,

                        SUM(CASE 
                            WHEN cod_status_apontamento_periodo = 1 THEN soma_horas 
                            ELSE 0 
                        END) AS soma_horas_pendentes,

                        SUM(CASE 
                            WHEN cod_status_apontamento_periodo = 3 THEN soma_horas 
                            ELSE 0 
                        END) AS soma_horas_reprovadas
                    FROM temp_result_filtrada;

                    -- =============================================================
                    -- LIMPEZA FINAL
                    -- =============================================================
                    DROP TEMPORARY TABLE IF EXISTS temp_result_filtrada;
                    DROP TEMPORARY TABLE IF EXISTS temp_aprovadores_filtrado;
                ";
                
                var parameters = new
                {
                    Limite = limite,
                    Offset = cursor,
                    Mes = mesVigencia,
                    Ano = anoVigencia,
                    TbOrgId = orgId,
                    Nome = nomeColaborador,
                    CodGerente = codigoGerente,
                    CodStatusApontamento = codigoStatus,
                    CodProjeto = codProjeto,
                    CodColaboradorExternoAprovador = codColaboradorExternoAprovador
                };

                var dadosResult = await connection.QueryMultipleAsync(
                    query,
                    parameters,
                    commandType: CommandType.Text
                );

                var colaboradorResults = dadosResult.Read<dynamic>().ToList(); // #temp_result_filtrada
                var aprovadorResults = dadosResult.Read<dynamic>().ToList();  // #temp_aprovadores_filtrado

                ret.Lista = colaboradorResults.Select(x =>
                {
                    var colaborador = new ColaboradorEApontamentoResult
                    {
                        ColaboradorNome = x.colaborador_nome,
                        ColaboradorCPF = x.codigo_interno_colaborador,
                        Ativo = x.ativo == 1,
                        DataInativacao = x.data_inativacao?.ToString("dd/MM/yyyy"),
                        DataAdmissao = x.data_admissao?.ToString("dd/MM/yyyy"),
                        NomeCompletoGerente = x.nome_completo_gerente,
                        CodigoGerente = x.codigo_gerente,
                        CodigoStatusApontamentoPeriodo = Convert.ToInt32(x.cod_status_apontamento_periodo),
                        DescricaoStatusApontamento = x.descricao_status_apontamento,
                        SomaHoras = x.soma_horas,
                        Observacao = x.observacao,
                        Projeto = x.projeto,
                        CodProjeto = x.cod_projeto
                    };

                    // Filtra aprovadores para correspondências específicas
                    colaborador.Aprovadores = aprovadorResults.Where(a =>
                                                                   a.codigo_interno_colaborador == x.codigo_interno_colaborador &&
                                                                   a.cod_status_apontamento_periodo == x.cod_status_apontamento_periodo &&
                                                                   a.tb_org_id == x.tb_org_id &&
                                                                   a.mes == x.mes &&
                                                                   a.ano == x.ano &&
                                                                   a.cod_projeto == x.cod_projeto)
                     .Select(a => new AprovadorResult()
                     {
                         NomeAprovador = ((string)a.nome_aprovador).ToUpperInvariant(),
                         CodigoInternoAprovador = ((string)a.codigo_interno_aprovador)
                     })
                     .ToList();

                    return colaborador;
                }).ToList();
                
                

                // Totalizador padrão
                ret.Totalizador = dadosResult.Read<dynamic>().Select(x => new TotalizadorApontamentosDTO // #temp_result_filtrada_count_1
                {
                    QuantidadeTotalColaboradores = (int)x.total,
                    SomaHoras = x.soma_horas != null ? (long)x.soma_horas : 0
                }).FirstOrDefault();

                // Totalizador BigNumbers
                var totalizadorBigNumbers = dadosResult.ReadFirstOrDefault<dynamic>(); // #temp_result_filtrada_count_2

                if (totalizadorBigNumbers != null)
                {
                    ret.TotalizadorBigNumbers = new TotalizadorApontamentosBigNumbersDTO
                    {
                        QuantidadeTotalColaboradores = ObjectExtension.ToIntOuZero(totalizadorBigNumbers.quantidade_total_colaboradores),
                        SomaHorasLancadas = ObjectExtension.ToDecimalOuZero(totalizadorBigNumbers.soma_horas_lancadas),
                        QuantidadeTotalNaoApontado = ObjectExtension.ToStringOuParametro(totalizadorBigNumbers.quantidade_total_nao_apontado, "0/0"),
                        SomaHorasAprovadas = ObjectExtension.ToDecimalOuZero(totalizadorBigNumbers.soma_horas_aprovadas),
                        SomaHorasPendentes = ObjectExtension.ToDecimalOuZero(totalizadorBigNumbers.soma_horas_pendentes),
                        SomaHorasReprovdas = ObjectExtension.ToDecimalOuZero(totalizadorBigNumbers.soma_horas_reprovadas)
                    };
                }
                
                var listaDeProjetosApontamentosSemAprovador = ret.Lista.Where(x => x.Aprovadores.Count == 0 && !x.CodProjeto.IsNullOrEmpty()).Select(x => x.CodProjeto).ToList();

                var aprovadoresBusca =
                    await BuscarAprovadoresPorListaDeProjetos(listaDeProjetosApontamentosSemAprovador, orgId);
                
                foreach (var apontamento in ret.Lista)
                {
                    if (apontamento.Aprovadores.Count > 0)
                    {
                        continue;
                    }
                    
                    var aprovadores = aprovadoresBusca.Where(x => x.CodigoProjeto == apontamento.CodProjeto).ToList();
                    apontamento.Aprovadores = aprovadores ?? [];
                }

                ret.DataColetaDeDados = await BuscarHorasExataBancoDeDados();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<AprovadorResult>> BuscarAprovadoresPorListaDeProjetos(List<string> projetos, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
	                tc.nome_completo AS NomeAprovador,
	                tc.codigo_interno_colaborador AS CodigoInternoAprovador,
	                tpge.cod_projeto AS CodigoProjeto
                from tb_projeto_gerente tpge 
                INNER JOIN tb_colaborador_org tco ON tco.cod_colaborador_externo = tpge.cod_colaborador_gerente  
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador 
                WHERE tpge.cod_projeto IN @Projetos
                AND tpge.tb_org_id = @OrgId
            ";

            var parametros = new
            {
                Projetos = projetos,
                OrgId = orgId
            };
            
            var result = await connection.QueryAsync<AprovadorResult>(query, parametros);
            return result.ToList();
        }

        public async Task<List<GestoresApontamentoResult>> ListarGestoresApontamento(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@tb_org_id", orgId);

                string sql = @"SELECT
                                        vgo.cod_colaborador_superior,
                                        vgo.nome_completo,
                                        vgo.cod_diretoria,
                                        vgo.diretoria,
                                        vgo.tb_org_id
                                  FROM
                                        vw_gestores_org vgo
                                  WHERE
                                        vgo.tb_org_id = @tb_org_id";

                var listaGerentes = await connection.QueryAsync<dynamic>(sql, parameters);

                return listaGerentes.Select(x => new GestoresApontamentoResult
                {
                    NomeCompleto = x.nome_completo,
                    CodigoGerente = x.cod_colaborador_superior.ToString(),
                    CodigoDiretoria = x.cod_diretoria,
                    Diretoria = x.diretoria,
                }).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<StatusApontamentoGrupoResult>> ListaStatusApontamentoGerenteProjeto(int orgId, string idioma)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"SELECT
                                        tsa.id,
                                        COALESCE(tt.traducao,tsa.descricao)  as descricao,
                                    	tsa.cod_status_apontamento,
                                    	tsa.tb_cod_status_grupo,
                                    	tsa.exibir_status_gerente_projeto
                                   FROM
                                    	tb_status_apontamento tsa
								   LEFT JOIN
										tb_traducoes tt ON tsa.cod_status_apontamento = tt.valor_chave
										AND tt.tabela_chave = 'tb_status_apontamento.cod_status_apontamento'
										AND tt.idioma = @Idioma";

                var parametros = new { OrgId = orgId, Idioma = idioma };  //orgid não usado por enquanto

                var listaStatus = await connection.QueryAsync<dynamic>(sql, parametros);

                return listaStatus.Select(x => new StatusApontamentoGrupoResult
                {
                    CodigoStatus = x.cod_status_apontamento,
                    StatusDescricao = x.descricao
                }).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ColaboradoresOrgDTO> GetColaboradorPorCpfEOrgId(string cpfColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @" SELECT
                                    	tco.tb_org_id,
                                    	tco.codigo_interno_colaborador,
                                    	tco.cod_diretoria,
                                    	tco.diretoria,
                                    	tco.departamento,
                                    	tco.cod_departamento,
                                    	tco.data_criacao,
                                    	tco.data_alteracao,
                                    	tco.cargo,
                                    	tco.codigo_cargo,
                                    	tco.cod_colaborador_externo,
                                    	tco.data_admissao,
                                    	tco.ativo,
                                        tco.data_inativacao
                                    FROM
                                     	tb_colaborador_org tco
                                    WHERE
                                     	tco.codigo_interno_colaborador = @Cpf and tco.tb_org_id = @OrgId";

                var parametros = new
                {
                    Cpf = cpfColaborador,
                    OrgId = orgId
                };

                var listaColaboradores = await connection.QueryAsync<dynamic>(sql, parametros);

                return listaColaboradores.Select(tco => new ColaboradoresOrgDTO
                {
                    Cpf = tco.codigo_interno_colaborador,
                    CodColaborador = tco.cod_colaborador_externo,
                    Ativo = Convert.ToBoolean(tco.ativo),
                    DataInativacao = tco.data_inativacao?.ToString()
                }).SingleOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ProjetoDTO> GetProjetoPorCodProjetoOrgId(string codProjeto, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @" SELECT
                                    	tpo.cod_projeto,
                                    	tpo.projeto,
                                    	tpo.cod_cliente,
                                    	tclo.nome_cliente as cliente,
                                    	tpo.permite_apont_sem_alocacao,
                                    	tpo.permite_apont_sem_alocacao_outro_colab,
                                    	tpo.data_fim
                                    FROM
                                     	tb_projeto_org tpo
	                                LEFT JOIN
		                                tb_cliente_org tclo ON tpo.cod_cliente = tclo.codigo_cliente AND tpo.tb_org_id = tclo.tb_org_id
                                    WHERE
                                     	tpo.cod_projeto = @CodProjeto and tpo.tb_org_id = @OrgId";

                var parametros = new
                {
                    CodProjeto = codProjeto,
                    OrgId = orgId
                };

                var listaProjeto = await connection.QueryAsync<dynamic>(sql, parametros);

                return listaProjeto.Select(tpo => new ProjetoDTO
                {
                    CodProjeto = tpo.cod_projeto,
                    NomeProjeto = tpo.projeto,
                    CodCliente = tpo.cod_cliente,
                    NomeCliente = tpo.cliente,
                    PermiteApontamentoSemAlocacao = tpo.permite_apont_sem_alocacao,
                    PermiteApontamentoSemAlocacaoParaOutroColaborador = tpo.permite_apont_sem_alocacao_outro_colab,
                    DataFim = tpo.data_fim != null ? DateTime.Parse(tpo.data_fim.ToString()) : null
                }).SingleOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ColaboradorRelatorioNaoApontadoDTO>> RelatorioColaboradoresQueNaoApontaram(int orgId, int mes, int ano)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                string sql = @"SELECT DISTINCT
                                        tco.cod_colaborador_externo,
                                        tc.nome_completo,
                                        tc.contato_principal,
                                        tu.email,
                                        tco.modelo_contratacao,
                                        tco.empresa_relacionada
                                    FROM
										tb_colaborador_org tco
                                    JOIN tb_colaborador tc
                                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                    JOIN tb_usuario tu
                                        ON tu.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                    LEFT JOIN tb_colaborador_apontamento tca
                                        ON tco.codigo_interno_colaborador = tca.codigo_interno_colaborador
                                        AND MONTH(tca.data) = @Mes
                                        AND YEAR(tca.data) = @Ano
                                   INNER JOIN tb_modelo_contratacao_org tmco
                                        ON tmco.codigo_modelo_contratacao = tco.codigo_modelo_contratacao
                                        AND tmco.tb_org_id = tco.tb_org_id
                                    WHERE
                                        (tmco.id IS NULL OR tmco.oculta_busca_timesheet = 0)
										AND tco.tb_org_id = @OrgId
                                        AND tca.codigo_interno_colaborador IS NULL
                                        AND (
                                             -- Colaborador ativo
		                                        (
		                                            tco.ativo = 1
		                                            AND (
		                                                YEAR(tco.data_admissao) < @Ano
		                                                OR (YEAR(tco.data_admissao) = @Ano AND MONTH(tco.data_admissao) <= @Mes)
		                                            )
		                                        )
		                                        OR
		                                        -- Colaborador inativo
		                                        (
		                                            tco.ativo = 0
		                                            AND (
		                                                YEAR(tco.data_admissao) <= @Ano
		                                                OR (YEAR(tco.data_admissao) = @Ano AND MONTH(tco.data_admissao) >= @Mes)
		                                            )
		                                            AND (
		                                                YEAR(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) > @Ano
		                                                OR (
		                                                    YEAR(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) = @Ano
		                                                    AND MONTH(DATE_SUB(tco.data_inativacao, INTERVAL 1 DAY)) >= @Mes
		                                                )
		                                            )
		                                        )
		                                    )
									ORDER BY
										tc.nome_completo;";

                var parametros = new
                {
                    Mes = mes,
                    OrgId = orgId,
                    Ano = ano
                };

                var listaProjeto = await connection.QueryAsync<dynamic>(sql, parametros);

                return listaProjeto.Select(x => new ColaboradorRelatorioNaoApontadoDTO
                {
                    CodigoColaborador = x.cod_colaborador_externo,
                    Email = x.email,
                    NomeColaborador = x.nome_completo,
                    ContatoPrincipal = x.contato_principal,
                    ModeloContratacao = x.modelo_contratacao,
                    EmpresaRelacionada = x.empresa_relacionada
                })
                .ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ExisteApontamentosAlteradosDuranteAExecucaoPorIds(List<string> ids,
            DateTime? dataColetaDeDados)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"
                        SELECT
                            tca.codigo_interno_colaborador
                        FROM
                            tb_colaborador_apontamento tca
                        WHERE
                            FIND_IN_SET(tca.id, @Ids) > 0 AND
                            tca.data_alteracao IS NOT NULL
                            AND tca.data_alteracao > @DataColeta";

                var parametros = new
                {
                    DataColeta = dataColetaDeDados,
                    Ids = string.Join(",", ids)
                };

                var result = await connection.QueryAsync(query, parametros);

                return result.ToList().Count > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DateTime> BuscarHorasExataBancoDeDados()
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"SELECT NOW();";

                var result = await connection.QueryFirstOrDefaultAsync<DateTime>(query);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> VerificaSeDataDeRegistroEhMaiorQueDataDoCadastro(string dataRegistro, string codigoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    CASE 
                        WHEN DATE(tco.data_admissao) is null THEN 1
                        WHEN DATE(@DataRegistro) >= DATE(tco.data_admissao) THEN 1
                        ELSE 0
                    END
                FROM tb_colaborador_org tco
                WHERE 
                    tco.codigo_interno_colaborador = @CodigoColaborador
                    AND tco.tb_org_id = @OrgId
                ORDER BY tco.data_criacao DESC
                LIMIT 1;
            ";

            var parametros = new
            {
                DataRegistro = DateTime.Parse(dataRegistro),
                CodigoColaborador = codigoColaborador,
                OrgId = orgId
            };

            var result = await connection.QuerySingleOrDefaultAsync<bool>(query, parametros);
            return result;
        }
    
        
        public async Task<bool> VerificaSeEhGestorHierarquicoDeUmAprovador(int orgId, string codigoInternoColaborador, string codigoInternoGerente)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                    SELECT 
                    CASE 
                        WHEN EXISTS (
                            SELECT 1
                            FROM vw_gestor_hierarquico_aprovadores vwgha 
                            WHERE 
                                vwgha.tb_org_id = @OrgId
                                AND vwgha.cod_interno_colaborador = @CodigoInternoColaborador
                                AND vwgha.cod_interno_gestor = @CodigoInternoGerente
                        ) THEN 1
                        ELSE 0
                    END AS Resultado
            ";

            var parametros = new
            {
                OrgId = orgId,
                CodigoInternoColaborador = codigoInternoColaborador,
                CodigoInternoGerente = codigoInternoGerente,
            };

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, parametros);
            return result == 1;
        }

        public async Task<string> BuscarCpfPorApontamentoId(string apontamentoId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                    SELECT 
                        tca.codigo_interno_colaborador
                    FROM tb_colaborador_apontamento tca
                    WHERE tca.id = @Id
            ";

            var parametros = new
            {
                Id = apontamentoId
            };

            var result = await connection.QuerySingleOrDefaultAsync<string>(query, parametros);
            return result;
        }
        
        public async Task<List<VigenciaSimplesDTO>> ListarVigenciasMesEQuantidade(int quantidadeMes, DateTime dataAtual)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    tv.ano AS Ano,
                    tv.mes AS Mes
                FROM tb_vigencia tv
                WHERE STR_TO_DATE(CONCAT(tv.ano, '-', LPAD(tv.mes, 2, '0'), '-01'), '%Y-%m-%d')
                      BETWEEN DATE_SUB(STR_TO_DATE(CONCAT(@Ano, '-', LPAD(@Mes, 2, '0'), '-01'), '%Y-%m-%d'), INTERVAL @QuantidadeMes MONTH)
                          AND STR_TO_DATE(CONCAT(@Ano, '-', LPAD(@Mes, 2, '0'), '-01'), '%Y-%m-%d')
                ORDER BY tv.ano, tv.mes;
            ";
            var parametros = new
            {
                QuantidadeMes = quantidadeMes,
                Mes = dataAtual.Month,
                Ano = dataAtual.Year
            };
            
            var result = await connection.QueryAsync<VigenciaSimplesDTO>(query, parametros);
            return result.ToList();
        }

        public async Task<bool> VerificaSeColaboradorPodeApontarPorModeloDeTrabalho(int orgId, string codigoInternoColaborador)
        {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                SELECT 
                    CASE 
                        WHEN EXISTS (
                            SELECT 1
                            FROM tb_colaborador_org tco
                            JOIN tb_parametro_configuracao tpc 
                                ON tpc.valor_parametro = tco.modelo_contratacao 
                                AND tpc.tb_org_id = tco.tb_org_id 
                                AND tpc.codigo_parametro = 'OCULTAR_CONSULTAS_TIMESHEET_MODELO_CONTRATACAO'
                            WHERE tco.codigo_interno_colaborador = @CodigoInternoColaborador
                              AND tco.tb_org_id = @OrgId
                        ) 
                        THEN 0  -- false: não pode apontar
                        ELSE 1  -- true: pode apontar
                    END AS PodeApontar;";

                var parametros = new
                {
                    OrgId = orgId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                };

                var result = await connection.QueryFirstAsync<bool>(query, parametros);
                return result;
        }
        
        public async Task<List<ApontamentoExcedenteDTO>> ListarApontamentosExcedentesPorOrgId(int orgId, int mes, int ano, int horasExcedentes)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
               SELECT
                    tc.nome_completo AS NomeColaborador,
                    SUM(tca.horas) AS HorasTotais,
                    SUM(CASE WHEN tsa.tb_cod_status_grupo = 2 THEN tca.horas ELSE 0 END) AS HorasAprovadas,
                    SUM(CASE WHEN tsa.tb_cod_status_grupo = 1 THEN tca.horas ELSE 0 END) AS HorasPendentes   
                FROM tb_colaborador_apontamento tca 
                JOIN tb_vigencia tv 
                    ON tv.id = tca.tb_vigencia_id 
                JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tca.codigo_interno_colaborador 
                JOIN tb_status_apontamento tsa 
                    ON tsa.id = tca.tb_status_apontamento_id 
                WHERE tca.tb_org_id = @OrgId
                  AND tsa.tb_cod_status_grupo NOT IN (4, 5, 3) -- DELETADO E NAO APONTADO
                  AND tv.mes = @Mes
                  AND tv.ano = @Ano
                GROUP BY tca.codigo_interno_colaborador, tc.nome_completo
                HAVING SUM(tca.horas) > @HorasExcedentes;";

            var parametros = new
            {
                OrgId = orgId,
                Mes = mes,
                Ano = ano,
                HorasExcedentes = horasExcedentes
            };

            var result = await connection.QueryAsync<ApontamentoExcedenteDTO>(query, parametros);
            return result.ToList();
        }
    }
}