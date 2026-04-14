using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using Core.DomainModel.Competencia;
using Dapper;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.GestaoDeCompetencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Competencia
{
    public class GestaoDeCompetenciaRepository : IGestaoDeCompetenciaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestaoDeCompetenciaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<SkillItemDTO> VerificaHabilidadeExistentePorIdETipo(int id, int tipoId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT
                            vws.id AS Id,
                            vws.descricao as Descricao,
                            vws.tipo_id as TipoId
                        FROM
                            vw_skills vws
                        WHERE
                            vws.id = @Id
                            AND vws.tipo_id = @TipoId;";
            var parametros = new
            {
                Id = id,
                TipoId = tipoId
            };
            var result = await connection.QueryFirstOrDefaultAsync<SkillItemDTO>(query, parametros);

            return result;
        }

        public async Task<SkillItemDTO> VerificaHabilidadeExistentePorDescricaoETipo(string descricao, int tipoId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT
                            vws.id AS Id,
                            vws.descricao as Descricao,
                            vws.tipo_id as TipoId
                        FROM
                            vw_skills vws
                        WHERE
                            vws.descricao = @Descricao
                            AND vws.tipo_id = @TipoId;";
            var parametros = new
            {
                Descricao = descricao,
                TipoId = tipoId
            };
            var result = await connection.QueryFirstOrDefaultAsync<SkillItemDTO>(query, parametros);

            return result;
        }

        public async Task<List<ColaboradorHabilidadeDTO>> ListarColaboradoresPorSkillId(long id, TipoCompetenciaSRSEnum tipoCategoria)
        {
            var connection = _dapperConnection.GetConnection();

            if (!CompetenciasMapa().ContainsKey(tipoCategoria))
                throw new ArgumentException("Tipo de competência inválido.", nameof(tipoCategoria));

            var (relacao, tabela, coluna) = CompetenciasMapa()[tipoCategoria];

            var query = $@"
                            SELECT
                                tbc.codigo_interno_colaborador AS id,
                                tci.id as skill_id,
                                n.id AS nivel_id,
                                n.descricao AS nivel,
                                r.descricao as descricao
                            FROM
                                tb_colaborador tbc
                            JOIN {tabela} tci
                                ON tbc.codigo_interno_colaborador = tci.codigo_interno_colaborador
                            LEFT JOIN tb_nivel n
                                ON tci.tb_nivel_id = n.id
                            LEFT JOIN {relacao} r
                                ON tci.{coluna} = r.id
                            WHERE
                                tci.{coluna} = @Id";

            var parametros = new { Id = id };

            var result = await connection.QueryAsync<dynamic>(query, parametros);

            var colaboradores = result.
            GroupBy(x => new
            {
                x.id,
                x.skill_id,
                x.nivel_id,
                x.nivel,
                x.descricao,
            }).Select(group => new ColaboradorHabilidadeDTO
            {
                Id = group.Key.id,
                Skill = new()
                {
                    Descricao = group.Key.descricao,
                    TipoSkill = tipoCategoria.ToString(),
                    Nivel = group.Key.nivel_id != null ? new()
                    {
                        Id = (long)group.Key.nivel_id,
                        Descricao = group.Key.nivel
                    }
                    : null
                }
            }).ToList();

            return colaboradores;
        }

        public async Task AdicionaNovaCompetenciaAoColaborador(AdicionarCompetenciaDTO competencia, long? nivelId, string cpf, TipoCompetenciaSRSEnum tipoCompetenciaEnum)
        {
            var connection = _dapperConnection.GetConnection();

            var (_, tabela, coluna) = CompetenciasMapa()[tipoCompetenciaEnum];

            var query = $@"INSERT INTO {tabela}
                                (codigo_interno_colaborador, {coluna}, tb_nivel_id)
                            VALUES
                                (@Cpf, @SkillId, @NivelId);";

            var parametros = new
            {
                Cpf = cpf,
                SkillId = competencia.IdCompetencia,
                NivelId = nivelId
            };

            await connection.ExecuteAsync(query, parametros);
        }

        public async Task InativarHabilidadeDoColaborador(long habilidadeId, string CpfColaborador, TipoCompetenciaSRSEnum tipoCompetenciaEnum)
        {
            var connection = _dapperConnection.GetConnection();

            var (_, tabela, coluna) = CompetenciasMapa()[tipoCompetenciaEnum];

            var query = $@"UPDATE {tabela}
                                SET ativo = 0
                            WHERE
                                codigo_interno_colaborador = @Cpf
                                AND {coluna} = @SkillId;";

            var parametros = new
            {
                Cpf = CpfColaborador,
                SkillId = habilidadeId
            };

            await connection.ExecuteAsync(query, parametros);
        }

        public async Task AtualizarGestorExternoPerfilSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var queryAntigos = @"
                SELECT
                    CAST(tgeps.tb_gestor_externo_perfil_id AS CHAR(36)) AS GestorExternoPerfilId,
                    tgeps.tb_item_perfil_id AS ItemPerfilId,
                    tgeps.skill_id AS SkillId,
                    tgeps.tb_nivel_id AS NivelId,
                    tgeps.data_criacao AS DataCriacao,
                    tgeps.relevante AS Relevante
                FROM tb_gestor_externo_perfil_skill tgeps
                WHERE
                    tgeps.tb_item_perfil_id = @PerfilItemIdAntigo
                    AND tgeps.skill_id = @IdSkillAntigo;";

            var antigos = await connection.QueryAsync<GestorExternoPerfilSkillDTO>(queryAntigos, new
            {
                PerfilItemIdAntigo = perfilItemIdAntigo,
                IdSkillAntigo = idSkillAntigo
            });

            foreach (var antigo in antigos)
            {
                if (antigo.ItemPerfilId == perfilItemIdNovo && antigo.SkillId == idSkillNovo)
                {
                    continue;
                }
                // Verifica se já existe um novo com o mesmo Gestor, novo PerfilItem e nova Skill
                var queryExisteNovo = @"
                    SELECT COUNT(1)
                    FROM tb_gestor_externo_perfil_skill
                    WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                      AND tb_item_perfil_id = @PerfilItemIdNovo
                      AND skill_id = @IdSkillNovo;";

                var existeNovo = await connection.ExecuteScalarAsync<bool>(queryExisteNovo, new
                {
                    GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                    PerfilItemIdNovo = perfilItemIdNovo,
                    IdSkillNovo = idSkillNovo
                });

                if (existeNovo)
                {
                    // Já existe novo => deletar o antigo
                    var queryDelete = @"
                        DELETE FROM tb_gestor_externo_perfil_skill
                        WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                          AND tb_item_perfil_id = @PerfilItemIdAntigo
                          AND skill_id = @IdSkillAntigo;";

                    await connection.ExecuteAsync(queryDelete, new
                    {
                        GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                        PerfilItemIdAntigo = antigo.ItemPerfilId,
                        IdSkillAntigo = antigo.SkillId
                    });
                }
                else
                {
                    // Não existe => fazer update
                    var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, antigo.NivelId);

                    var queryUpdate = @"
                        UPDATE tb_gestor_externo_perfil_skill
                        SET tb_item_perfil_id = @PerfilItemIdNovo,
                            skill_id = @IdSkillNovo,
                            tb_nivel_id = @NivelIdEquivalente
                        WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                          AND tb_item_perfil_id = @PerfilItemIdAntigo
                          AND skill_id = @IdSkillAntigo
                          AND tb_nivel_id = @NivelId;";

                    await connection.ExecuteAsync(queryUpdate, new
                    {
                        GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente,
                        PerfilItemIdAntigo = antigo.ItemPerfilId,
                        IdSkillAntigo = antigo.SkillId,
                        NivelId = antigo.NivelId
                    });
                }
            }
        }

        public async Task AtualizarAlocadoSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var querySelect = @"
                SELECT
                    tb_colaborador_periodo_alocacao_id AS Id,
                    tb_item_perfil_id AS ItemPerfilId,
                    skill_id AS SkillId,
                    tb_nivel_id AS NivelId,
                    data_criacao AS DataCriacao
                FROM tb_colaborador_alocado_skill
                WHERE tb_item_perfil_id = @PerfilItemIdAntigo
                    AND skill_id = @IdSkillAntigo;";

            var resultadoQuery = await connection.QueryAsync<GestorExternoPerfilSkillDTO>(querySelect, new
            {
                PerfilItemIdAntigo = perfilItemIdAntigo,
                IdSkillAntigo = idSkillAntigo
            });

            foreach (var item in resultadoQuery)
            {
                if (item.ItemPerfilId == perfilItemIdNovo && item.SkillId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.NivelId);

                var queryExisteNovo = @"
                    SELECT Count(1) as ret
                    FROM tb_colaborador_alocado_skill
                    WHERE tb_colaborador_periodo_alocacao_id = @Id
                      AND tb_item_perfil_id = @PerfilItemIdNovo
                      AND skill_id = @IdSkillNovo;";

                var existeNovo = await connection.QueryFirstOrDefaultAsync<int?>(queryExisteNovo, new
                {
                    Id = item.Id,
                    PerfilItemIdNovo = perfilItemIdNovo,
                    IdSkillNovo = idSkillNovo,

                });
                Console.WriteLine("TEste: " + existeNovo);
                if (existeNovo != null)
                {
                    var queryDelete = @"
                        DELETE FROM tb_colaborador_alocado_skill
                        WHERE tb_colaborador_periodo_alocacao_id = @Id
                          AND tb_item_perfil_id = @ItemPerfilId
                          AND skill_id = @SkillId;";

                    await connection.ExecuteAsync(queryDelete, new
                    {
                        Id = item.Id,
                        ItemPerfilId = item.ItemPerfilId,
                        SkillId = item.SkillId,
                    });
                }
                else
                {
                    var queryUpdate = @"
                        UPDATE tb_colaborador_alocado_skill
                        SET tb_item_perfil_id = @PerfilItemIdNovo,
                            skill_id = @IdSkillNovo,
                            tb_nivel_id = @NivelIdEquivalente
                        WHERE tb_colaborador_periodo_alocacao_id = @Id
                          AND tb_item_perfil_id = @ItemPerfilId
                          AND skill_id = @SkillId;";

                    await connection.ExecuteAsync(queryUpdate, new
                    {
                        Id = item.Id,
                        ItemPerfilId = item.ItemPerfilId,
                        SkillId = item.SkillId,
                        NivelId = item.NivelId,
                        DataCriacao = item.DataCriacao,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente
                    });
                }
            }
        }


        public async Task AtualizarPerfilSkills(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<dynamic>(
                @"SELECT
                    tb_perfil_id AS GestorExternoPerfilId,
                    tb_item_perfil_id AS ItemPerfilId,
                    skill_id AS SkillId,
                    tb_nivel_id AS NivelId,
                    data_criacao AS DataCriacao
                  FROM tb_perfil_skill
                  WHERE tb_item_perfil_id = @PerfilItemIdAntigo
                    AND skill_id = @IdSkillAntigo;",
                new { PerfilItemIdAntigo = perfilItemIdAntigo, IdSkillAntigo = idSkillAntigo });

            foreach (var item in resultadoQuery)
            {
                if (item.ItemPerfilId == perfilItemIdNovo && item.SkillId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.NivelId);

                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_perfil_skill
                      WHERE tb_perfil_id = @GestorExternoPerfilId
                        AND tb_item_perfil_id = @PerfilItemIdNovo
                        AND skill_id = @IdSkillNovo;",
                    new
                    {
                        GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_perfil_skill
                          WHERE tb_perfil_id = @GestorExternoPerfilId
                            AND tb_item_perfil_id = @PerfilItemIdAntigo
                            AND skill_id = @IdSkillAntigo;",
                        new
                        {
                            GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                            PerfilItemIdAntigo = item.ItemPerfilId,
                            IdSkillAntigo = item.SkillId,
                        });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_perfil_skill
                      SET tb_item_perfil_id = @PerfilItemIdNovo,
                          skill_id = @IdSkillNovo,
                          tb_nivel_id = @NivelIdEquivalente
                      WHERE tb_perfil_id = @GestorExternoPerfilId
                        AND tb_item_perfil_id = @PerfilItemIdAntigo
                        AND skill_id = @IdSkillAntigo;",
                    new
                    {
                        GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                        PerfilItemIdAntigo = item.ItemPerfilId,
                        IdSkillAntigo = item.SkillId,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente
                    });
                }
            }
        }

        public async Task AtualizarSkillsVagaSRS(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<SkillsVagasSrsDTO>(
                @"SELECT 
                    id, 
                    vaga_id, 
                    categoria_id,
                    descricao_id, 
                    nivel_id, 
                    data_criacao, 
                    data_alteracao
                  FROM tb_skill_vaga_srs
                  WHERE categoria_id = @PerfilItemIdAntigo 
                    AND descricao_id = @IdSkillAntigo;",
                new
                {
                    PerfilItemIdAntigo = perfilItemIdAntigo,
                    IdSkillAntigo = idSkillAntigo
                });

            foreach (var item in resultadoQuery)
            {
                if (item.CategoriaId == perfilItemIdNovo && item.DescricaoId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.NivelId);

                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1) 
                      FROM tb_skill_vaga_srs 
                      WHERE vaga_id = @VagaId
                        AND categoria_id = @PerfilItemIdNovo
                        AND descricao_id = @IdSkillNovo
                        AND nivel_id = @NivelIdEquivalente;",
                    new
                    {
                        VagaId = item.VagaId,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_vaga_srs 
                          WHERE id = @Id;",
                        new { Id = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                        @"UPDATE tb_skill_vaga_srs
                          SET categoria_id = @PerfilItemIdNovo,
                              descricao_id = @IdSkillNovo,
                              nivel_id = @NivelIdEquivalente
                          WHERE id = @Id;",
                        new
                        {
                            Id = item.Id,
                            PerfilItemIdNovo = perfilItemIdNovo,
                            IdSkillNovo = idSkillNovo,
                            NivelIdEquivalente = nivelEquivalente
                        });
                }
            }
        }
        
        public async Task AtualizarSkillsVaga(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<SkillsVagasDTO>(
                @"SELECT id, 
                         tipo_skill_id AS TypeSkills, 
                         skill_id AS SkillId, 
                         skill_nivel_id AS SkillNivelId
                  FROM tb_skill_vaga
                  WHERE tipo_skill_id = @PerfilItemIdAntigo 
                    AND skill_id = @IdSkillAntigo;",
                new
                {
                    PerfilItemIdAntigo = perfilItemIdAntigo,
                    IdSkillAntigo = idSkillAntigo
                });

            foreach (var item in resultadoQuery)
            {
                if (item.TypeSkills == perfilItemIdNovo && item.SkillId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.SkillNivelId);

                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_skill_vaga
                      WHERE tipo_skill_id = @PerfilItemIdNovo
                        AND skill_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        Id = item.Id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_vaga WHERE id = @Id;",
                        new { Id = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_skill_vaga
                      SET tipo_skill_id = @PerfilItemIdNovo,
                          skill_id = @IdSkillNovo,
                          skill_nivel_id = @NivelIdEquivalente
                      WHERE id = @Id;",
                    new
                    {
                        Id = item.Id,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente
                    });
                }
            }
        }
        
        public async Task AtualizarSkillsVagaFourmakers(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<dynamic>(
                @"SELECT id, 
                         tb_item_perfil_id AS TypeSkills, 
                         skill_id AS SkillId, 
                         skill_nivel_id AS SkillNivelId
                  FROM tb_vaga_skill
                  WHERE tb_item_perfil_id = @PerfilItemIdAntigo 
                    AND skill_id = @IdSkillAntigo;",
                new
                {
                    PerfilItemIdAntigo = perfilItemIdAntigo,
                    IdSkillAntigo = idSkillAntigo
                });

            foreach (var item in resultadoQuery)
            {
                if (item.TypeSkills == perfilItemIdNovo && item.SkillId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.SkillNivelId);

                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_vaga_skill
                      WHERE tb_item_perfil_id = @PerfilItemIdNovo
                        AND skill_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        Id = item.Id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_vaga_skill WHERE id = @Id;",
                        new { Id = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_vaga_skill
                      SET tb_item_perfil_id = @PerfilItemIdNovo,
                          skill_id = @IdSkillNovo,
                          skill_nivel_id = @NivelIdEquivalente
                      WHERE id = @Id;",
                    new
                    {
                        Id = item.Id,
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        NivelIdEquivalente = nivelEquivalente
                    });
                }
            }
        }

        public async Task AtualizarSkillsVagaCandidato(int perfilItemIdAntigo, long idSkillAntigo, int perfilItemIdNovo, long idSkillNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<SkillsCandidatosDTO>(
                @"SELECT id, 
                         candidato_id, 
                         categoria_id, 
                         descricao_id, 
                         nivel_id, 
                         data_criacao, 
                         data_alteracao
                  FROM tb_skill_candidato_srs
                  WHERE categoria_id = @PerfilItemIdAntigo 
                    AND descricao_id = @IdSkillAntigo;",
                new { PerfilItemIdAntigo = perfilItemIdAntigo, IdSkillAntigo = idSkillAntigo });

            foreach (var item in resultadoQuery)
            {
                if (item.CategoriaId == perfilItemIdNovo && item.DescricaoId == idSkillNovo)
                {
                    continue;
                }
                var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteAlocado(perfilItemIdNovo, item.NivelId);

                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_skill_candidato_srs
                      WHERE categoria_id = @PerfilItemIdNovo
                        AND descricao_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemIdNovo = perfilItemIdNovo,
                        IdSkillNovo = idSkillNovo,
                        Id = item.Id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_candidato_srs WHERE id = @IdAtual;",
                        new { IdAtual = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                        @"UPDATE tb_skill_candidato_srs
                          SET categoria_id = @PerfilItemIdNovo,
                              descricao_id = @IdSkillNovo,
                              nivel_id = @NivelIdEquivalente
                          WHERE id = @IdAtual;",
                        new
                        {
                            IdAtual = item.Id,
                            PerfilItemIdNovo = perfilItemIdNovo,
                            IdSkillNovo = idSkillNovo,
                            NivelIdEquivalente = nivelEquivalente
                        });
                }
            }
        }

        public async Task<int> AlterarIdHabilidadePorTipoGestorExternoPerfilSkills(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var queryAntigos = @"
                SELECT
                    CAST(tgeps.tb_gestor_externo_perfil_id AS CHAR(36)) AS GestorExternoPerfilId,
                    tgeps.tb_item_perfil_id AS ItemPerfilId,
                    tgeps.skill_id AS SkillId,
                    tgeps.tb_nivel_id AS NivelId,
                    tgeps.data_criacao AS DataCriacao,
                    tgeps.relevante AS Relevante
                FROM tb_gestor_externo_perfil_skill tgeps
                WHERE
                    tgeps.tb_item_perfil_id = @PerfilItemId
                    AND tgeps.skill_id = @IdSkillAntigo;";
            
            var antigos = await connection.QueryAsync<GestorExternoPerfilSkillDTO>(queryAntigos, new
            {
                PerfilItemId = perfilItemId,
                IdSkillAntigo = skillIdAntiga
            });

            var rowsUpdated = 0;

            foreach (var antigo in antigos)
            {
                var queryExisteNovo = @"
                    SELECT COUNT(1)
                    FROM tb_gestor_externo_perfil_skill
                    WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                      AND tb_item_perfil_id = @PerfilItemId
                      AND skill_id = @IdSkillNovo;";

                var existeNovo = await connection.ExecuteScalarAsync<bool>(queryExisteNovo, new
                {
                    GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                    PerfilItemId = perfilItemId,
                    IdSkillNovo = skillId
                });

                if (existeNovo)
                {
                    // Já existe novo => deletar o antigo
                    var queryDelete = @"
                        DELETE FROM tb_gestor_externo_perfil_skill
                        WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                          AND tb_item_perfil_id = @PerfilItemIdAntigo
                          AND skill_id = @IdSkillAntigo;";

                    await connection.ExecuteAsync(queryDelete, new
                    {
                        GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                        PerfilItemIdAntigo = antigo.ItemPerfilId,
                        IdSkillAntigo = antigo.SkillId
                    });
                }
                else
                {
                    var queryUpdate = @"
                        UPDATE tb_gestor_externo_perfil_skill
                        SET 
                            skill_id = @IdSkillNovo              
                        WHERE tb_gestor_externo_perfil_id = @GestorExternoPerfilId
                          AND tb_item_perfil_id = @PerfilItemId
                          AND skill_id = @IdSkillAntigo;";

                    await connection.ExecuteAsync(queryUpdate, new
                    {
                        GestorExternoPerfilId = antigo.GestorExternoPerfilId,
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId,
                        IdSkillAntigo = antigo.SkillId,
                    });
                    rowsUpdated++;
                }

            }
            return rowsUpdated;
        }
        
        public async Task<int> AlterarIdHabilidadePorTipoAlocadoSkills(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var querySelect = @"
                SELECT
                    tb_colaborador_periodo_alocacao_id AS Id,
                    tb_item_perfil_id AS ItemPerfilId,
                    skill_id AS SkillId,
                    tb_nivel_id AS NivelId,
                    data_criacao AS DataCriacao
                FROM tb_colaborador_alocado_skill
                WHERE tb_item_perfil_id = @PerfilItemId
                    AND skill_id = @IdSkillAntigo;";
            
            var resultadoQuery = await connection.QueryAsync<GestorExternoPerfilSkillDTO>(querySelect, new
            {
                PerfilItemId = perfilItemId,
                IdSkillAntigo = skillIdAntiga
            });

            var rowsUpdated = 0;

            foreach (var item in resultadoQuery)
            {
                var queryExisteNovo = @"
                    SELECT Count(1) as ret
                    FROM tb_colaborador_alocado_skill
                    WHERE tb_colaborador_periodo_alocacao_id = @Id
                      AND tb_item_perfil_id = @PerfilItemId
                      AND skill_id = @IdSkill;";
                
                var existeNovo = await connection.QueryFirstOrDefaultAsync<bool>(queryExisteNovo, new
                {
                    Id = item.Id,
                    PerfilItemId = perfilItemId,
                    IdSkill = skillId,
                });
                
                if (existeNovo)
                {
                    var queryDelete = @"
                        DELETE FROM tb_colaborador_alocado_skill
                        WHERE tb_colaborador_periodo_alocacao_id = @Id
                          AND tb_item_perfil_id = @ItemPerfilId
                          AND skill_id = @SkillId;";

                    await connection.ExecuteAsync(queryDelete, new
                    {
                        Id = item.Id,
                        ItemPerfilId = item.ItemPerfilId,
                        SkillId = item.SkillId,
                    });
                }
                else
                {
                    var queryUpdate = @"
                        UPDATE tb_colaborador_alocado_skill
                        SET skill_id = @IdSkillNovo
                        WHERE tb_colaborador_periodo_alocacao_id = @Id
                          AND tb_item_perfil_id = @ItemPerfilId
                          AND skill_id = @SkillId;";

                    await connection.ExecuteAsync(queryUpdate, new
                    {
                        Id = item.Id,
                        ItemPerfilId = item.ItemPerfilId,
                        SkillId = item.SkillId,
                        IdSkillNovo = skillId,
                    });

                    rowsUpdated++;
                }
            }
            
            return rowsUpdated;
        }
        public async Task<int> AlterarIdHabilidadePorTipoPerfilSkills(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var selectQuery = @"
                SELECT
                    tb_perfil_id AS GestorExternoPerfilId,
                    tb_item_perfil_id AS ItemPerfilId,
                    skill_id AS SkillId,
                    tb_nivel_id AS NivelId,
                    data_criacao AS DataCriacao
                  FROM tb_perfil_skill
                  WHERE tb_item_perfil_id = @PerfilItemId
                    AND skill_id = @IdSkillAntigo;";
            
            var resultadoQuery = await connection.QueryAsync<dynamic>(selectQuery, new { PerfilItemId = perfilItemId, IdSkillAntigo = skillIdAntiga });
            
            var rowsUpdated = 0;
            
            foreach (var item in resultadoQuery)
            {
                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_perfil_skill
                      WHERE tb_perfil_id = @GestorExternoPerfilId
                        AND tb_item_perfil_id = @PerfilItemId
                        AND skill_id = @IdSkillNovo;",
                    new
                    {
                        GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId
                    });
                
                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_perfil_skill
                          WHERE tb_perfil_id = @GestorExternoPerfilId
                            AND tb_item_perfil_id = @PerfilItemId
                            AND skill_id = @IdSkillAntigo;",
                        new
                        {
                            GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                            PerfilItemId = item.ItemPerfilId,
                            IdSkillAntigo = item.SkillId,
                        });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_perfil_skill
                          SET 
                              skill_id = @IdSkillNovo
                          WHERE tb_perfil_id = @GestorExternoPerfilId
                            AND tb_item_perfil_id = @PerfilItemId
                            AND skill_id = @IdSkillAntigo;",
                    new
                    {
                        GestorExternoPerfilId = item.GestorExternoPerfilId.ToString(),
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId,
                        IdSkillAntigo = skillIdAntiga
                    });
                    
                    rowsUpdated++;
                }
            }
            return rowsUpdated;
        }

        public async Task<int> AlterarIdHabilidadePorTipoSkillsVaga(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<SkillsVagasDTO>(
            @"SELECT id, 
                     tipo_skill_id AS TypeSkills, 
                     skill_id AS SkillId, 
                     skill_nivel_id AS SkillNivelId
                  FROM tb_skill_vaga
                  WHERE tipo_skill_id = @PerfilItemId 
                    AND skill_id = @IdSkillAntigo;",
            new
            {
                PerfilItemId = perfilItemId,
                IdSkillAntigo = skillIdAntiga
            });
            
            var rowsUpdated = 0;
            
            foreach (var item in resultadoQuery)
            {
                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_skill_vaga
                      WHERE tipo_skill_id = @PerfilItemId
                        AND skill_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId,
                        Id = item.Id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_vaga WHERE id = @Id;",
                        new { Id = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_skill_vaga
                          SET 
                              skill_id = @IdSkillNovo
                          WHERE id = @Id;",
                    new
                    {
                        Id = item.Id,
                        IdSkillNovo = skillId
                    });
                    
                    rowsUpdated++;
                }

            }

            return rowsUpdated;
        }
        
         public async Task<int> AlterarIdHabilidadePorTipoSkillsVagaFourmakers(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<dynamic>(
            @"SELECT id, 
                     tb_item_perfil_id AS TypeSkills, 
                     skill_id AS SkillId, 
                     skill_nivel_id AS SkillNivelId
                  FROM tb_vaga_skill
                  WHERE tb_item_perfil_id = @PerfilItemId 
                    AND skill_id = @IdSkillAntigo;",
            new
            {
                PerfilItemId = perfilItemId,
                IdSkillAntigo = skillIdAntiga
            });
            
            var rowsUpdated = 0;
            
            foreach (var item in resultadoQuery)
            {
                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_vaga_skill
                      WHERE tb_item_perfil_id = @PerfilItemId
                        AND skill_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId,
                        Id = item.id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_vaga_skill WHERE id = @Id;",
                        new { Id = item.id });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_vaga_skill
                          SET 
                              skill_id = @IdSkillNovo
                          WHERE id = @Id;",
                    new
                    {
                        Id = item.id,
                        IdSkillNovo = skillId
                    });
                    
                    rowsUpdated++;
                }

            }

            return rowsUpdated;
        }
         
        public async Task<int> AlterarIdHabilidadePorTipoSkillsVagasSRS(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();
    
            var resultadoQuery = await connection.QueryAsync<SkillsVagasSrsDTO>(
                @"SELECT 
                    id, 
                    vaga_id, 
                    categoria_id,
                    descricao_id, 
                    nivel_id, 
                    data_criacao, 
                    data_alteracao
                  FROM tb_skill_vaga_srs
                  WHERE categoria_id = @PerfilItemId
                    AND descricao_id = @IdSkillAntigo;",
                new
                {
                    PerfilItemId = perfilItemId,
                    IdSkillAntigo = skillIdAntiga
                });
            
            var updatedRows = 0;
            foreach (var item in resultadoQuery)
            {
                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1) 
                      FROM tb_skill_vaga_srs 
                      WHERE vaga_id = @VagaId
                        AND categoria_id = @PerfilItemId
                        AND descricao_id = @IdSkillNovo",
                    new
                    {
                        VagaId = item.VagaId,
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_vaga_srs 
                          WHERE id = @Id;",
                        new { Id = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                    @"UPDATE tb_skill_vaga_srs
                      SET 
                          descricao_id = @IdSkillNovo
                      WHERE id = @Id;",
                    new
                    {
                        Id = item.Id,
                        IdSkillNovo = skillId,
                    });

                    updatedRows++;
                }
            }
            return updatedRows;
        }

        public async Task<int> AlterarIdHabilidadePorTipoSkillsVagasCandidato(int skillIdAntiga, int skillId, int perfilItemId)
        {
            var connection = _dapperConnection.GetConnection();

            var resultadoQuery = await connection.QueryAsync<SkillsCandidatosDTO>(
                @"SELECT id, 
                         candidato_id, 
                         categoria_id, 
                         descricao_id, 
                         nivel_id, 
                         data_criacao, 
                         data_alteracao
                  FROM tb_skill_candidato_srs
                  WHERE categoria_id = @PerfilItemId 
                    AND descricao_id = @IdSkillAntigo;",
                new { PerfilItemId = perfilItemId, IdSkillAntigo = skillIdAntiga });
            
            var updatedRows = 0;

            foreach (var item in resultadoQuery)
            {
                var existeNovo = await connection.ExecuteScalarAsync<bool>(
                    @"SELECT COUNT(1)
                      FROM tb_skill_candidato_srs
                      WHERE categoria_id = @PerfilItemId
                        AND descricao_id = @IdSkillNovo
                        AND id = @Id;",
                    new
                    {
                        PerfilItemId = perfilItemId,
                        IdSkillNovo = skillId,
                        Id = item.Id
                    });

                if (existeNovo)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM tb_skill_candidato_srs WHERE id = @IdAtual;",
                        new { IdAtual = item.Id });
                }
                else
                {
                    await connection.ExecuteAsync(
                        @"UPDATE tb_skill_candidato_srs
                          SET 
                              descricao_id = @IdSkillNovo
                          WHERE id = @IdAtual;",
                        new
                        {
                            IdAtual = item.Id,
                            IdSkillNovo = skillId,
                        });

                    updatedRows++;
                }
            }
            return updatedRows;
        }
        
        public async Task<List<SkillParaUnificarDTO>> ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum tipo)

        {
            var tabela = CompetenciasMapaPerfilitem(tipo);

            var connection = _dapperConnection.GetConnection();
            var query = @$"
                SELECT DISTINCT
                    verificacao.skill_id AS SkillIdOld,
                    tc.id AS SkillIdNew
                FROM {tabela} tc
                INNER JOIN (
                    SELECT 
                        tgeps.id , 
                        tgeps.skill_id,
                        tgeps.tb_item_perfil_id,
                        tc.descricao AS skill_desc, 
                        thc.descricao_competencia AS historico_skill_desc, 
                        thc.observacao,
                        TRIM(
                            SUBSTRING_INDEX(
                                REGEXP_SUBSTR(thc.observacao, 'com[[:space:]]*[^[:cntrl:]]+$'),
                                'com', -1
                            )
                        ) AS skill_unificada
                    FROM tb_vaga_skill tgeps 
                    INNER JOIN {tabela} tc 
                        ON tc.id = tgeps.skill_id 
                    LEFT JOIN tb_historico_competencia thc 
                        ON REPLACE(tc.descricao, '_X', '') = thc.descricao_competencia
                    WHERE tc.ativo = 0
                      AND tgeps.tb_item_perfil_id = @Tipo
                      AND thc.situacao = 'Unificada'
                ) AS verificacao 
                    ON verificacao.skill_unificada = tc.descricao;        
            ";
    
            var result = await connection.QueryAsync<SkillParaUnificarDTO>(query, new { Tipo = tipo});
            return result.ToList();
        }

        private Dictionary<TipoCompetenciaSRSEnum, (string Relacao, string Tabela, string Coluna)> CompetenciasMapa()
        {
            return new Dictionary<TipoCompetenciaSRSEnum, (string Relacao, string Tabela, string Coluna)>
            {
                { TipoCompetenciaSRSEnum.Idioma, ("tb_idioma", "tb_colaborador_idioma", "idioma_id") },
                { TipoCompetenciaSRSEnum.HardSkill, ("tb_competencia", "tb_colaborador_competencia", "competencia_id") },
                { TipoCompetenciaSRSEnum.SoftSkill, ("tb_softskill", "tb_colaborador_softskill", "softskill_id") },
                { TipoCompetenciaSRSEnum.Metodologia, ("tb_metodologia", "tb_colaborador_metodologia", "metodologia_id") },
                { TipoCompetenciaSRSEnum.Dominio, ("tb_dominionegocio", "tb_colaborador_dominionegocio", "dominionegocio_id") },
                { TipoCompetenciaSRSEnum.Desconhecida, ("tb_skill_desconhecida", "tb_colaborador_skill_desconhecida", "skill_desconhecida_id") }
            };
        }
        
        private string CompetenciasMapaPerfilitem(ItemPerfilEnum tipo)
        {
            return tipo switch
            {
                ItemPerfilEnum.SOFTSKILL => "tb_softskill",
                ItemPerfilEnum.COMPETENCIA => "tb_competencia",
                ItemPerfilEnum.DOMINIONEGOCIO => "tb_dominionegocio",
                ItemPerfilEnum.METODOLOGIA => "tb_metodologia",
                ItemPerfilEnum.IDIOMA => "tb_idioma",
                _ => throw new Exception("Tipo item perfil invalido")// caso padrão (default)
            };
        }
    }
}