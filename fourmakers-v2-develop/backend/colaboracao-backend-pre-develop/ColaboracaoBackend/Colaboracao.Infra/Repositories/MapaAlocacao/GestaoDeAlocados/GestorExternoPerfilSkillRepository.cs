using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados
{
    public class GestorExternoPerfilSkillRepository : IGestorExternoPerfilSkillRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestorExternoPerfilSkillRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<GestorExternoPerfilSkillResult>> ListarGestorExternoPerfilSkillsPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId)
        {
            var connection = _dapperConnection.GetConnection();

            // Determina a tabela a ser usada com base no tb_item_perfil_id
            var select = @"
                            SELECT
                                tgeps.tb_gestor_externo_perfil_id AS GestorExternoPerfilId,
                                tgeps.tb_item_perfil_id AS ItemPerfilId,
                                tip.descricao AS ItemPerfilDescricao,
                                tgeps.skill_id AS SkillId,
                                CASE
                                    WHEN tgeps.tb_item_perfil_id = @Competencia THEN tc.descricao
                                    WHEN tgeps.tb_item_perfil_id = @SoftSkill THEN tss.descricao
                                    WHEN tgeps.tb_item_perfil_id = @Metodologia THEN tm.descricao
                                    WHEN tgeps.tb_item_perfil_id = @DominioNegocio THEN tdn.descricao
                                    WHEN tgeps.tb_item_perfil_id = @Idioma THEN ti.descricao
                                    ELSE NULL
                                END AS SkillDescricao,
                                tgeps.tb_nivel_id AS NivelId,
                                tn.descricao AS NivelDescricao,
                                tgeps.data_criacao AS DataCriacao,
                                tgeps.relevante as Relevante,
                                tgeps.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorAlteracao
                            FROM
                                tb_gestor_externo_perfil_skill tgeps
                            LEFT JOIN
                                tb_item_perfil tip ON tgeps.tb_item_perfil_id = tip.id AND tip.ativo = 1
                            LEFT JOIN
                                tb_competencia tc ON tgeps.skill_id = tc.id AND tgeps.tb_item_perfil_id = @Competencia AND tc.ativo = 1
                            LEFT JOIN
                                tb_softskill tss ON tgeps.skill_id = tss.id AND tgeps.tb_item_perfil_id = @SoftSkill AND tss.ativo = 1
                            LEFT JOIN
                                tb_metodologia tm ON tgeps.skill_id = tm.id AND tgeps.tb_item_perfil_id = @Metodologia AND tm.ativo = 1
                            LEFT JOIN
                                tb_dominionegocio tdn ON tgeps.skill_id = tdn.id AND tgeps.tb_item_perfil_id = @DominioNegocio AND tdn.ativo = 1
                            LEFT JOIN
                                tb_idioma ti ON tgeps.skill_id = ti.id AND tgeps.tb_item_perfil_id = @Idioma AND ti.ativo = 1
                            LEFT JOIN
                                tb_nivel tn ON tgeps.tb_nivel_id = tn.id AND tn.ativo = 1
                            WHERE
                                tgeps.tb_gestor_externo_perfil_id = @GestorExternoPerfilId;";

            // Define os parâmetros para o enum
            var parameters = new
            {
                GestorExternoPerfilId = gestorExternoPerfilId,
                Competencia = (long)ItemPerfilEnum.COMPETENCIA,
                SoftSkill = (long)ItemPerfilEnum.SOFTSKILL,
                Metodologia = (long)ItemPerfilEnum.METODOLOGIA,
                DominioNegocio = (long)ItemPerfilEnum.DOMINIONEGOCIO,
                Idioma = (long)ItemPerfilEnum.IDIOMA
            };

            // Executa a consulta SQL
            var perfilSkills = await connection.QueryAsync<dynamic>(select, parameters);

            // Mapeia os resultados para o DTO
            return perfilSkills
                .Where(perfil => perfil.SkillDescricao == null || !perfil.SkillDescricao.Contains("_INATIVO"))
                .Select(perfil => new GestorExternoPerfilSkillResult
            {
                DataCriacao = perfil.DataCriacao,
                ItemPerfil = new ItemPerfilResult
                {
                    Id = perfil.ItemPerfilId,
                    Descricao = perfil.ItemPerfilDescricao
                },
                Skill = perfil.SkillId != null ? new SkillResult
                {
                    Id = perfil.SkillId,
                    Descricao = perfil.SkillDescricao
                } : null,
                Nivel = perfil.NivelId != null ? new NivelResult
                {
                    Id = perfil.NivelId,
                    Descricao = perfil.NivelDescricao
                } : null,
                Relevante = perfil.Relevante
            });
        }

        public async Task<IEnumerable<GestorExternoPerfilSkillResult>> InserirGestorExternoPerfilSkillAsync(GestorExternoPerfilSkillInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            INSERT INTO tb_gestor_externo_perfil_skill
                                (tb_gestor_externo_perfil_id, tb_item_perfil_id, skill_id, tb_nivel_id, relevante, codigo_interno_colaborador_criacao)
                            VALUES
                                (@GestorExternoPerfilId, @ItemPerfilId, @SkillId, @NivelId, @Relevante, @CodigoInternoColaboradorAlteracao)";

            var parameters = new
            {
                input.GestorExternoPerfilId,
                ItemPerfilId = input.ItemPerfil.Id,
                SkillId = input.Skill.Id,
                NivelId = input.Nivel.Id,
                input.Relevante,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ListarGestorExternoPerfilSkillsPorGestorExternoPerfilIdAsync(input.GestorExternoPerfilId);
            }

            return null;
        }

        public async Task<bool> DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"  DELETE FROM
                                tb_gestor_externo_perfil_skill
                            WHERE
                                tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                        ";

            int rowsAffected = await connection.ExecuteAsync(query, new
            {
                GestorExternoPerfilId = gestorExternoPerfilId
            });
            return rowsAffected >= 0;
        }

        public async Task<bool> ValidarRelacaoNivelItemPerfilAsync(long itemPerfilId, long nivelId)
        {
            var sql = @"
                        SELECT
                            COUNT(1)
                        FROM
                            tb_nivel
                        WHERE
                            id = @NivelId
                            AND tb_item_perfil_id = @ItemPerfilId";

            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteScalarAsync<bool>(sql, new { NivelId = nivelId, ItemPerfilId = itemPerfilId });
        }

        public async Task<bool> ValidarSkillParaItemPerfilAsync(long skillId, long itemPerfilId)
        {
            var sql = @"
                        SELECT
                            CASE
                                WHEN @ItemPerfilId = @Competencia THEN
                                    EXISTS (SELECT 1 FROM tb_competencia WHERE id = @SkillId)
                                WHEN @ItemPerfilId = @SoftSkill THEN
                                    EXISTS (SELECT 1 FROM tb_softskill WHERE id = @SkillId)
                                WHEN @ItemPerfilId = @Metodologia THEN
                                    EXISTS (SELECT 1 FROM tb_metodologia WHERE id = @SkillId)
                                WHEN @ItemPerfilId = @DominioNegocio THEN
                                    EXISTS (SELECT 1 FROM tb_dominionegocio WHERE id = @SkillId)
                                WHEN @ItemPerfilId = @Idioma THEN
                                    EXISTS (SELECT 1 FROM tb_idioma WHERE id = @SkillId)
                                ELSE
                                    FALSE
                            END";

            var parametros = new
            {
                SkillId = skillId,
                ItemPerfilId = itemPerfilId,
                Competencia = (long)ItemPerfilEnum.COMPETENCIA,
                SoftSkill = (long)ItemPerfilEnum.SOFTSKILL,
                Metodologia = (long)ItemPerfilEnum.METODOLOGIA,
                DominioNegocio = (long)ItemPerfilEnum.DOMINIONEGOCIO,
                Idioma = (long)ItemPerfilEnum.IDIOMA
            };

            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteScalarAsync<bool>(sql, parametros);
        }

        public async Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var tabela = ObterNomeTabelaCompetencia(competencia);

            string queryUsuario = "SELECT id FROM tb_usuario WHERE codigo_interno_colaborador = @Cpf limit 1;";
            var usuarioId = await _connectionDapper.QuerySingleOrDefaultAsync<int>(queryUsuario, new { Cpf = cpf });

            if (usuarioId.IsNull())
            {
                throw new Exception("Usuário não encontrado.");
            }

            int id = 0;
            descricao = descricao.ToUpper();

            // Verifica se a competência já existe na tabela
            string queryVerificacao = $@"SELECT id FROM {tabela} WHERE descricao = @Descricao LIMIT 1;";
            var competenciaExistente = await _connectionDapper.QuerySingleOrDefaultAsync<int?>(queryVerificacao, new { Descricao = descricao });

            if (competenciaExistente.HasValue)
            {
                
                id = competenciaExistente.Value;
            }
            else
            {
                // Se não existe, insere uma nova competência
                string query = $@"INSERT INTO {tabela} (descricao, ativo, confirmada, usuario_criacao_id) VALUES (@Descricao, 1, 1, @UsuarioId);
                            SELECT LAST_INSERT_ID();";

                id = await _connectionDapper.ExecuteScalarAsync<int>(query, new { Descricao = descricao, UsuarioId = usuarioId });
            }

            return new AdicionarCompetenciaDTO()
            {
                Ativo = true,
                Pendente = false,
                DescricaoCompetencia = descricao,
                IdCompetencia = id
            };
        }

        private string ObterNomeTabelaCompetencia(TipoCompetenciaSRSEnum competenciaEnum)
        {
            return competenciaEnum switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "tb_competencia",
                TipoCompetenciaSRSEnum.SoftSkill => "tb_softskill",
                TipoCompetenciaSRSEnum.Metodologia => "tb_metodologia",
                TipoCompetenciaSRSEnum.Dominio => "tb_dominionegocio",
                TipoCompetenciaSRSEnum.Idioma => "tb_idioma",
                TipoCompetenciaSRSEnum.Desconhecida => "tb_skill_desconhecida",
                _ => throw new Exception("Tipo de competência inválido.")
            };
        }
    }
}