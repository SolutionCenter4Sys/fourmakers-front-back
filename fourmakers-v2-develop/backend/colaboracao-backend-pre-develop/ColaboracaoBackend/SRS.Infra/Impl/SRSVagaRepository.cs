using Colaboracao.Helper;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using MySql.Data.MySqlClient;
using SRS.Infra.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DataTransferObject.Domain.VagasSRS;

namespace SRS.Infra.Impl
{
    public class SRSVagaRepository : ISRSVagaRepository
    {
        private MySqlConnection CreateMySqlConnectionFromEnv() => new(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SRS_STRING_CONNECTION"));

        public List<DropDownItemDTO> ListarAprovadores()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    email as Id,
                                    email as Descricao
                                FROM
                                    tipo_unidcontrata
                                WHERE
                                    descricao = 'UN II - RAFAEL'";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarCargaHoraria()
        {
            return new List<DropDownItemDTO>()
            {
                new DropDownItemDTO() { Id = "Integral", Descricao = "Integral" },
                new DropDownItemDTO() { Id = "Reduzida", Descricao = "Reduzida" },
            };
        }

        public List<DropDownItemDTO> ListarUnidadesSRS()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    u.descricao as Id,
                                    u.descricao as Descricao
                                FROM
                                    unidades u
                                ORDER BY
                                    u.descricao";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<CargoDropdownItemDTO> ListarCargos()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    c.idcandidate_cargo_id as Id,
                                    c.papel as Descricao,
                                    CONCAT('<p><strong>Papel:</strong>  ', c.papel, '</p>',
                                    '<p><strong>Responsabilidades:</strong>  ', COALESCE(c.responsabilidade, ''),
                                    '</p>', '<p><strong>Habilidades Técnicas Desejáveis:</strong>  ', COALESCE(c.conhecimento_tecnico, ''),
                                    '</p>', '<p><strong>Habilidades Comportamentais:</strong>  ', COALESCE(c.habilidades_comportamentais, ''),
                                    '</p>', '<p><strong>Experiência:</strong>  ', COALESCE(c.experiencia, ''),
                                    '</p>', '<p><strong>Formação:</strong>  ', COALESCE(c.formacao_academica, ''),
                                    '</p>' ) as DescricaoCargo
                                FROM
                                    cargo c
                                ORDER BY
                                    papel";

                    return db.Query<CargoDropdownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @$"
                                    SELECT
                                        e.hardware AS Descricao,
                                        ceh.equipamento_id AS Id
                                    FROM
                                        company_equipamento_hardskill ceh
                                    INNER JOIN
                                        equipamentos e ON e.equipamento_id = ceh.equipamento_id
                                    INNER JOIN
                                        company c ON c.company_id = ceh.company_id
                                    WHERE
                                        ceh.hardskill_id = @HardskillId
                                        AND ceh.STATUS = 1
                                        AND (
                                            c.company_crm_id = @IdCrm
                                            OR c.company_id = 1
                                        );
                                ";

                    return db.Query<DropDownItemDTO>(query, new { HardskillId = hardskillId, IdCrm = idContaCrm }).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarDuracaoContrato()
        {
            return new List<DropDownItemDTO>()
            {
                new DropDownItemDTO() { Id = "D", Descricao = "Determinado" },
                new DropDownItemDTO() { Id = "I", Descricao = "Indeterminado" },
            };
        }

        public List<DropDownItemDTO> ListarLocalTrabalho()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"select
                                    descricao as Id,
                                    descricao as Descricao
                                from
                                    tipo_local_trabalho
                                order by
                                    id";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarSolicitantes()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    user_id as Id,
                                    CONCAT(first_name, ' ', last_name) as Descricao
                                FROM
                                    user
                                WHERE
                                    usu_categoria in ('Gestor', 'Gestor de Relacionamento')
                                ORDER BY
                                    first_name";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarStackPrincipal()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    hardskill_id as Id,
                                    nome as Descricao
                                FROM
                                    hardskills
                                WHERE
                                    status = 1";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarTermometroVagas()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"select
                                    descricao as Id,
                                    descricao as Descricao
                                from
                                    tipo_medidor
                                order by
                                    id;";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarTipoContratacao()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    tipo_contratacao_id as Id,
                                    nome as Descricao
                                FROM
                                    tipo_contratacao
                                WHERE
                                    tipo_contratacao_id IN (2, 4, 5, 6, 7)
                                ORDER BY
                                    nome";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<DropDownItemDTO> ListarTipoVaga()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    descricao as Id,
                                    descricao_tela as Descricao
                                FROM
                                    tipo_vaga
                                ORDER BY
                                    id";

                    return db.Query<DropDownItemDTO>(query).ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }
        
        public List<JobOrderSemanticaDTO> ListarVagasParaSemanticaComSkill(int cursor, int limite)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"
                        SELECT
                            j.joborder_id AS JoborderId,
                            j.title AS Title,
                            j.description AS Description,
                            j.cargo_id AS CargoId,
                            j.cargo AS Cargo,
                            h.nome AS StackPrincipal,
                            j.skills AS Skills
                        FROM
                            joborder j
                        LEFT JOIN
                            hardskills h ON h.hardskill_id = j.hardskill_id
                        ORDER BY
                            j.joborder_id DESC
                        LIMIT @Limite OFFSET @Cursor;
                    ";

                    var vagas = db.Query<JobOrderSemanticaDTO>(query, new { Cursor = cursor, Limite = limite }).ToList();

                    const string sqlSkills = @"
                            SELECT
                                js.id AS Id,
                                js.joborder_id AS JoborderId,
                                js.category_id AS CategoryId,
                                js.description_id AS DescriptionId,
                                js.nivel_id AS NivelId
                            FROM
                                joborder_skills js
                            WHERE
                                js.joborder_id IN @JoborderIds;
                    ";
                    
                    
                    var ids = vagas.Select(x => x.JoborderId).ToList();
                    var results = db.Query<JobOrderSkillDTO>(
                        sqlSkills, 
                        new { JoborderIds = ids }
                    ).ToList();

                    var skillsPorJob = results
                        .GroupBy(s => s.JobOrderId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var vaga in vagas)
                    {
                        if (skillsPorJob.TryGetValue(vaga.JoborderId, out var skills))
                        {
                            vaga.JobOrderSkills = skills;
                        }
                        else
                        {
                            vaga.JobOrderSkills = new List<JobOrderSkillDTO>();
                        }
                    }
                    
                    return vagas;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }
    }
}