using Colaboracao.Core.Interfaces;
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
    public class PerfilAlocacaoRepository : IPerfilAlocacaoRepository
    {
        private readonly IDBConnection _dbConnection;

        private const string SchemaRetornoLeftJoinSkills = @"
            COALESCE(
                tcco.descricao,
                tccs.descricao,
                tccm.descricao,
                tccn.descricao,
                tcci.descricao,
                tccd.descricao
            ) AS descricao, 
            tn.descricao as nivel, 
            tn.id as nivel_id, 
            a.skill_id,
            tip.descricao as tipo_skill
        ";
        private const string SchemaLeftJoinSkills = @"
            LEFT JOIN  tb_competencia tcco         ON tcco.id = a.skill_id AND a.tb_item_perfil_id = 1  AND tcco.ativo = 1
            LEFT JOIN  tb_softskill tccs           ON tccs.id = a.skill_id AND a.tb_item_perfil_id = 8  AND tccs.ativo = 1
            LEFT JOIN  tb_metodologia tccm         ON tccm.id = a.skill_id AND a.tb_item_perfil_id = 3  AND tccm.ativo = 1
            LEFT JOIN  tb_dominionegocio tccn      ON tccn.id = a.skill_id AND a.tb_item_perfil_id = 4  AND tccn.ativo = 1
            LEFT JOIN  tb_idioma tcci              ON tcci.id = a.skill_id AND a.tb_item_perfil_id = 9  AND tcci.ativo = 1
	        LEFT JOIN  tb_skill_desconhecida tccd  ON tccd.id = a.skill_id AND a.tb_item_perfil_id = 14 AND tccd.ativo = 1
            inner join tb_item_perfil tip on tip.id = a.tb_item_perfil_id
            left join tb_nivel tn on tn.id = a.tb_nivel_id
        ";

        public PerfilAlocacaoRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<PerfilAlocacaoDTO> ListarPerfilAlocacao(string codProjeto, int orgId, bool ocultarSkills = false)
        {
            var connection = _dbConnection.GetConnection();
            string sql;
            if (!String.IsNullOrEmpty(codProjeto))
                sql = @"select vp.id, vp.perfil from vw_perfis vp
                            where vp.tb_org_id = @OrgId and vp.codigo_cliente in (select tpo.cod_cliente from tb_projeto_org tpo where tpo.cod_projeto = @CodProjeto and tpo.tb_org_id = @OrgId)
                            AND vp.ativo = 1";
            else
                sql = @"select vp.id, vp.perfil from vw_perfis vp
                            where vp.tb_org_id = @OrgId
                            AND vp.ativo = 1";

            var rows = connection.Query<dynamic>(sql, new { OrgId = orgId, CodProjeto = codProjeto });
            
            var perfilIds = rows
                .Select(x => (string)x.id?.ToString())
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();
            
            var skillsProfiles = ocultarSkills ? [] :  GetSkillsByProfileIds(connection, perfilIds);
            
            return rows.Select(x => new PerfilAlocacaoDTO
            {
                Id = x.id,
                Perfil = x.perfil,
                Skills = ocultarSkills 
                    ? [] 
                    : skillsProfiles
                        .Where(y => y.PerfilId == x.id.ToString().Split('|')[1])
                        .ToList(),
            }).ToList();
        }
        
        private List<SkillNivelDTO> GetSkillsByProfileIds(MySqlConnection connection, List<string> perfilIds)
        {
            // Separa os tipos e ids
            var tipo1Ids = perfilIds
                .Where(x => x.StartsWith("1|"))
                .Select(x => x.Split("|")[1])
                .ToList();

            var tipo2Ids = perfilIds
                .Where(x => x.StartsWith("2|"))
                .Select(x => x.Split("|")[1])
                .ToList();

            var result = new List<SkillNivelDTO>();

            if (tipo1Ids.Any())
            {
                var sql1 = @$"select 
                                {SchemaRetornoLeftJoinSkills},
                                a.tb_perfil_id
                                from tb_perfil_skill a
                                    {SchemaLeftJoinSkills}
                            where a.tb_perfil_id in @Ids;";

                var skills1 = connection.Query<dynamic>(sql1, new { Ids = tipo1Ids })
                    .Select(x => new SkillNivelDTO
                    {
                        Descricao = x.descricao,
                        Id = x.skill_id,
                        TipoSkill = CompetenciaUtils.GetTipoSkillDescricao(x.tipo_skill),
                        Nivel = x.nivel != null ? new NivelDTO
                        {
                            Descricao = x.nivel,
                            Id = x.nivel_id
                        } : null,
                        PerfilId = x.tb_perfil_id.ToString()
                    })
                    .ToList();

                result.AddRange(skills1);
            }

            if (tipo2Ids.Any())
            {
                var sql2 = @$"select 
                            {SchemaRetornoLeftJoinSkills},
                            a.tb_gestor_externo_perfil_id
                            from tb_gestor_externo_perfil_skill a
                                {SchemaLeftJoinSkills}
                            where a.tb_gestor_externo_perfil_id in @Ids;";

                var skills2 = connection.Query<dynamic>(sql2, new { Ids = tipo2Ids })
                    .Select(x => new SkillNivelDTO
                    {
                        Descricao = x.descricao,
                        Id = x.skill_id,
                        TipoSkill = CompetenciaUtils.GetTipoSkillDescricao(x.tipo_skill),
                        Nivel = x.nivel != null ? new NivelDTO
                        {
                            Descricao = x.nivel,
                            Id = x.nivel_id
                        } : null,
                        PerfilId = x.tb_gestor_externo_perfil_id.ToString()
                    })
                    .ToList();

                result.AddRange(skills2);
            }

            return result;
        }

        public List<SkillNivelDTO> ListarSkillsPerfilAlocacao(string perfilId)
        {
            return GetSkills(_dbConnection.GetConnection(), perfilId);
        }

        public PerfilAlocacaoDTO BuscarPerfilAlocacao(long codAlocacao)
        {
            var connection = _dbConnection.GetConnection();

            var query = @$"
                SELECT
                    vp.id,
                    vp.perfil
                FROM
                    tb_perfil_alocacao tpa
                INNER JOIN
                    vw_perfis vp ON vp.tb_org_id = tpa.tb_org_id AND (vp.id = CONCAT('1|', tpa.tb_perfil_id) OR vp.id = CONCAT('2|', tpa.tb_gestor_externo_perfil_id))
                WHERE
                    tpa.tb_colaborador_periodo_alocacao_id = @ColaboradorAlocacaoId;
            ";

            var parametros = new
            {
                ColaboradorAlocacaoId = codAlocacao
            };

            var result = connection.QuerySingleOrDefault<PerfilAlocacaoDTO>(query, parametros);
            return result;
        }
        private List<SkillNivelDTO> GetSkills(MySqlConnection connection, string perfilId)
        {
            var tipo = perfilId.Split("|")[0];
            var id = perfilId.Split("|")[1];
            var sqlSkills = "";
            if (tipo == "1")
                sqlSkills = $@"select 
                                {SchemaRetornoLeftJoinSkills}
                                from tb_perfil_skill a
                                    {SchemaLeftJoinSkills}
                            where a.tb_perfil_id  = @PerfilId;";
            if (tipo == "2")
                sqlSkills = $@"select {SchemaRetornoLeftJoinSkills}
                                from tb_gestor_externo_perfil_skill a
                                    {SchemaLeftJoinSkills}
                            where a.tb_gestor_externo_perfil_id  = @PerfilId;";

            return connection.Query<dynamic>(sqlSkills, new { PerfilId = id }).Select(x =>
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

        public void VincularPerfilAlocacao(long idAlocacao, string codPerfilGestorExterno, string codPerfil, int orgId)
        {
            var connection = _dbConnection.GetConnection();
            var sql = @"INSERT INTO tb_perfil_alocacao
                        (id, tb_org_id, tb_colaborador_periodo_alocacao_id, tb_gestor_externo_perfil_id, tb_perfil_id)
                        VALUES(uuid(), @OrgId, @AlocacaoId, @CodPerfilGestorExterno, @CodPerfil);";
            connection.Execute(sql, new { AlocacaoId = idAlocacao, OrgId = orgId, CodPerfilGestorExterno = codPerfilGestorExterno, CodPerfil = codPerfil });
        }

        public string InserirNovoPerfilAlocacao(string perfil, string codProjeto, List<ItemSkillPerfilAlocacaoDTO> skills, string codInternoColaborador, int orgId)
        {
            var connection = _dbConnection.GetConnection();
            var idPerfil = Guid.NewGuid().ToString();

            var sqlQuery = "select id from tb_perfil where nome_perfil = @Perfil and codigo_projeto = @CodProjeto and tb_org_id = @OrgId";
            var ret = connection.Query<dynamic>(sqlQuery, new { OrgId = orgId, Perfil = perfil, CodProjeto = codProjeto });
            if (ret.Any())
                throw new Exception("Já existe um perfil com a mesma descrição para esse cliente");

            var sqlPerfil = @"INSERT INTO tb_perfil
                                (id, tb_org_id, nome_perfil, codigo_projeto, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao)
                            VALUES(@IdPerfil, @OrgId, @Perfil,
                                @CodProjeto,
                                @CodColaborador, @CodColaborador);";
            connection.Execute(sqlPerfil, new { IdPerfil = idPerfil, OrgId = orgId, Perfil = perfil, CodProjeto = codProjeto, CodColaborador = codInternoColaborador });

            foreach (var skill in skills)
            {
                var sqlSkill = @"INSERT INTO tb_perfil_skill
                                (tb_perfil_id, tb_item_perfil_id, skill_id, tb_nivel_id, codigo_interno_colaborador_criacao)
                                VALUES(@IdPerfil, (select tip.id from tb_item_perfil tip where tip.descricao = @ItemPerfil), @SkillId, @NivelId, @CodColaborador);";
                connection.Execute(sqlSkill, new { IdPerfil = idPerfil, ItemPerfil = CompetenciaUtils.GetTipoSkillDescricaoBD(skill.TipoSkill), SkillId = skill.IdSkill, NivelId = skill.NivelId, CodColaborador = codInternoColaborador });
            }

            return idPerfil;
        }

        public void InserirSkillsAlocacao(long idAlocacao, List<ItemSkillPerfilAlocacaoDTO> skills, string codInternoColaborador)
        {
            var connection = _dbConnection.GetConnection();

            foreach (var skill in skills)
            {
                var sqlSkill = @"INSERT INTO tb_colaborador_alocado_skill
                                    (tb_colaborador_periodo_alocacao_id, tb_item_perfil_id, skill_id, tb_nivel_id, data_criacao, codigo_interno_colaborador_criacao)
                                    VALUES(@IdAlocacao, (select tip.id from tb_item_perfil tip where tip.descricao = @ItemPerfil), @SkillId, @NivelId, CURRENT_TIMESTAMP, @CodColaborador);";
                connection.Execute(sqlSkill, new { IdAlocacao = idAlocacao, ItemPerfil = CompetenciaUtils.GetTipoSkillDescricaoBD(skill.TipoSkill), SkillId = skill.IdSkill, NivelId = skill.NivelId, CodColaborador = codInternoColaborador });
            }
        }

        public List<SkillNivelDTO> ListarSkillsAlocacao(long idAlocacao)
        {
            var connection = _dbConnection.GetConnection();
            var sqlSkills = $@"select 
                                {SchemaRetornoLeftJoinSkills}
                                from tb_colaborador_alocado_skill a
                                    {SchemaLeftJoinSkills}
                                where a.tb_colaborador_periodo_alocacao_id = @IdAlocacao;";

            return connection.Query<dynamic>(sqlSkills, new { IdAlocacao = idAlocacao }).Select(x =>
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

        public void RemoverSkillAlocacao(long idAlocacao, ItemSkillPerfilAlocacaoDTO skill)
        {
            var connection = _dbConnection.GetConnection();
            var sqlSkill = @"DELETE FROM tb_colaborador_alocado_skill where tb_colaborador_periodo_alocacao_id = @IdAlocacao and skill_id = @SkillId and tb_item_perfil_id = (select tip.id from tb_item_perfil tip where tip.descricao = @ItemPerfil);";
            connection.Execute(sqlSkill, new { IdAlocacao = idAlocacao, ItemPerfil = CompetenciaUtils.GetTipoSkillDescricaoBD(skill.TipoSkill), SkillId = skill.IdSkill });
        }

        public void AlteraVinculoPerfilAlocacao(long idAlocacao, string codPerfilGestorExterno, string codPerfil)
        {
            var connection = _dbConnection.GetConnection();
            var sql = @"UPDATE tb_perfil_alocacao set tb_gestor_externo_perfil_id = @CodPerfilGestorExterno, tb_perfil_id = @CodPerfil where tb_colaborador_periodo_alocacao_id = @AlocacaoId;";
            connection.Execute(sql, new { AlocacaoId = idAlocacao, CodPerfilGestorExterno = codPerfilGestorExterno, CodPerfil = codPerfil });
        }

        public string ObterCodClientePorPerfilId(string perfilId, int orgId)
        {
            var connection = _dbConnection.GetConnection();
            var sql = @"SELECT
                        tge.codigo_cliente FROM
                    tb_gestor_externo_perfil tgep
                    inner join tb_gestor_externo tge on tge.cod_gestor_externo  = tgep.cod_gestor_externo
                    WHERE tgep.id = @PerfilId and tge.tb_org_id = @OrgId";
            return connection.QueryFirstOrDefault<string>(sql, new { PerfilId = perfilId, OrgId = orgId });
        }
    }
}