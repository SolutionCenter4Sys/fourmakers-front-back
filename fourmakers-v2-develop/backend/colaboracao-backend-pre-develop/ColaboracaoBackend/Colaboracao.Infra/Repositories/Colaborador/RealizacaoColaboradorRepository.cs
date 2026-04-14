using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util.Competencia;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra
{
    public class RealizacaoColaboradorRepository : IRealizacaoColaboradorRepository
    {
        private readonly IDBConnection _dbConnection;
        public RealizacaoColaboradorRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public List<RealizacaoColaboradorDTO> ListaRealizacoesColaborador(string codInternoColaborador, int orgId)
        {
            var connection = _dbConnection.GetConnection();

            var sql = @"SELECT
                            tco.nome_cliente as cliente,
                            tpo.projeto, tcpa.data_inicio, tcpa.data_fim, tcpa.quantidade_horas, tcpa.inclui_fimdesemana, tcpa.id,
                            (CASE
                                WHEN tpa.id is null then '-'
                                WHEN tpa.tb_gestor_externo_perfil_id is not null THEN (select tgep.nome_perfil from tb_gestor_externo_perfil tgep where tgep.id = tpa.tb_gestor_externo_perfil_id)
                                WHEN tpa.tb_perfil_id is not null is not null THEN (select tp.nome_perfil from tb_perfil tp where tp.id = tpa.tb_perfil_id)
                            END) as perfil
                        FROM
                            tb_colaborador_periodo_alocacao tcpa
                        INNER JOIN
                            tb_projeto_org tpo on tpo.cod_projeto = tcpa.codigo_projeto  and tpo.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN
                            tb_cliente_org tco on tco.codigo_cliente = tpo.cod_cliente and tco.tb_org_id = tcpa.tb_org_id
                        LEFT JOIN
                            tb_perfil_alocacao tpa on tpa.tb_colaborador_periodo_alocacao_id  = tcpa.id
                        WHERE
                            tcpa.codigo_interno_colaborador  = @CodInternoColaborador and tcpa.tb_org_id = @OrgId and tcpa.retroalimenta_cv = 1 and tcpa.ativo = 1;";

            var rows = connection.Query<dynamic>(sql, new { CodInternoColaborador = codInternoColaborador, OrgId = orgId });

            return rows.Select(x => new RealizacaoColaboradorDTO
            {
                Atual = DateTime.Now < x.data_fim,
                Cliente = x.cliente,
                DataFim = x.data_fim,
                DataInicio = x.data_inicio,
                Horas = x.data_inicio > DateTime.Now ? 0 : MapaUtil.GetHorasMesPeriodo(x.data_inicio, x.data_fim > DateTime.UtcNow ? DateTime.UtcNow : x.data_fim, double.Parse(x.quantidade_horas.ToString()), x.inclui_fimdesemana == (sbyte)1, new List<DateTime>().ToArray()),
                Perfil = x.perfil,
                Projeto = x.projeto,
                Skills = GetSkills(connection, x.id)
            }).ToList();
        }

        private List<SkillNivelDTO> GetSkills(MySqlConnection connection, long alocacaoId)
        {
            var sqlSkills = @"  SELECT
                                    vs.descricao,
                                    tn.descricao as nivel,
                                    tn.id as nivel_id,
                                    tcas.skill_id,
                                    tip.descricao as tipo_skill
                                FROM
                                    tb_colaborador_alocado_skill tcas
                                INNER JOIN
                                    vw_skills vs on vs.tipo_id = tcas.tb_item_perfil_id and vs.id = tcas.skill_id
                                INNER JOIN
                                    tb_item_perfil tip on tip.id = tcas.tb_item_perfil_id
                                LEFT JOIN
                                    tb_nivel tn on tn.id = tcas.tb_nivel_id
                                WHERE
                                    tcas.tb_colaborador_periodo_alocacao_id  = @AlocacaoId;";

            return connection.Query<dynamic>(sqlSkills, new { AlocacaoId = alocacaoId }).Select(x =>
                new SkillNivelDTO
                {
                    Descricao = x.descricao,
                    Id = x.skill_id,
                    TipoSkill = CompetenciaUtils.GetTipoSkillDescricao(x.tipo_skill),
                    Nivel = x.nivel != null ? new NivelDTO
                    {
                        Descricao = x.nivel,
                        Id = x.nivel_id
                    } : null
                }
            ).ToList();
        }
    }
}