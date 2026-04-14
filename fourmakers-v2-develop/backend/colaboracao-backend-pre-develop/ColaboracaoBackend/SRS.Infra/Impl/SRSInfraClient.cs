using Colaboracao.Helper;
using Colaboracao.Helper.Util.Competencia;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Historico;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Cargos;
using DataTransferObject.Domain.SRS.LocalDeTrabalho;
using DataTransferObject.Domain.SRS.Offboarding;
using DataTransferObject.Domain.SRS.Onboarding;
using DataTransferObject.Domain.SRS.Tecnica;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SRS.Infra.Impl
{
    public class SRSInfraClient : ISRSInfraClient
    {
        private MySqlConnection CreateMySqlConnectionFromEnv() => new(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SRS_STRING_CONNECTION"));

        public InsertCandidateDTO GetCandidate(string cpf)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@cpf", cpf);
                    var query = "SELECT `nomecompleto`, `phone_home`, `phone_cell`, `address`, `address_number`, `address_complement`, " +
                                "`district`, `city`, `state`, `zip`, `source`, `methodologies`, `email1`, `email2`, `emailFoursys`, `desired_pay`, " +
                                "`current_pay`, `is_active`, `cand_rg`, `cand_cpf`, `cand_estCivil`, `cand_Lkdin`, `cand_skype`, `instagram`, `facebook`, " +
                                "`twitter`, `cand_filhos`, `cand_fumante`, `disponibilidade`, `dataNascimento`," +
                                "`zona`, `uniresp`, `pcd`, `genero`, `orientacao_sexual`, " +
                                "`cand_other_level`, `cand_esp_level`, `cand_eng_level`, `cand_modalidade`, " +
                                "`cand_saude`, `cand_gruporisco`, `cand_familiares`, `cand_residentes`, " +
                                "`indicacao_4makers`, `etnia`, `pessoa_refugiada`, `tipo_cargo`, `site_id` FROM candidate WHERE cand_cpf = @cpf or cand_cpf_old = @cpf";
                    var candidate = db.QueryFirstOrDefault<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    return new InsertCandidateDTO
                    {
                        first_name = candidate.nomecompleto,
                        phone_home = candidate.phone_home,
                        phone_cell = candidate.phone_cell,
                        address = candidate.address,
                        address_number = candidate.address_number,
                        address_complement = candidate.address_complement,
                        district = candidate.district,
                        city = candidate.city,
                        state = candidate.state,
                        zip = candidate.zip,
                        source = candidate.source,
                        methodologies = candidate.methodologies,
                        email1 = candidate.email1,
                        email2 = candidate.email2,
                        emailFoursys = candidate.emailFoursys,
                        desired_pay = (decimal?)candidate.desired_pay,
                        current_pay = (decimal?)candidate.current_pay,
                        EhAtivo = (sbyte)candidate.is_active,
                        cand_rg = candidate.cand_rg,
                        Cand_cpf = candidate.cand_cpf,
                        cand_estCivil = candidate.cand_estCivil,
                        cand_Lkdin = candidate.cand_Lkdin,
                        cand_skype = candidate.cand_skype,
                        instagram = candidate.instagram,
                        facebook = candidate.facebook,
                        twitter = candidate.twitter,
                        cand_filhos = (int)candidate.cand_filhos,
                        EhFumante = candidate.cand_fumante,
                        disponibilidade = candidate.disponibilidade,
                        zona = candidate.zona,
                        uniresp = candidate.uniresp,
                        pcd = candidate.pcd ? (sbyte)1 : (sbyte)0,
                        genre = candidate.genre,
                        sexual_orientation = candidate.sexual_orientation,
                        cand_other_level = candidate.cand_other_level,
                        cand_esp_level = candidate.cand_esp_level,
                        cand_eng_level = candidate.cand_eng_level,
                        cand_modalidade = candidate.cand_modalidade,
                        cand_saude = candidate.cand_saude,
                        cand_gruporisco = candidate.cand_gruporisco,
                        cand_familiares = candidate.cand_familiares,
                        cand_residentes = candidate.cand_residentes,
                        indicacao_4makers = candidate.indicacao_4makers,
                        ethnicity = candidate.etnia,
                        refugee_person = candidate.pessoa_refugiada ? (sbyte)1 : (sbyte)0,
                        TipoCargo = candidate.tipo_cargo,
                        SiteId = (int)candidate.site_id,
                        dataNascimento = candidate.dataNascimento,
                        maquina_four = candidate.maquina_four,
                        maquina_cliente = candidate.maquina_cliente,
                        notes = candidate.notes,
                    };
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

        public InsertCandidateDTO GetCandidate(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    var query = "SELECT `candidate_id`, `nomecompleto`, `phone_home`, `phone_cell`, `address`, `address_number`, `address_complement`, " +
                                "`district`, `city`, `state`, `zip`, `source`, `methodologies`, `email1`, `email2`, `emailFoursys`, `desired_pay`, " +
                                "`current_pay`, `is_active`, `cand_rg`, `cand_cpf`, `cand_estCivil`, `cand_Lkdin`, `cand_skype`, `instagram`, `facebook`, " +
                                "`twitter`, `cand_filhos`, `cand_fumante`, `disponibilidade`, `dataNascimento`," +
                                "`zona`, `uniresp`, `pcd`, `genero`, `orientacao_sexual`, " +
                                "`cand_other_level`, `cand_esp_level`, `cand_eng_level`, `cand_modalidade`, " +
                                "`cand_saude`, `cand_gruporisco`, `cand_familiares`, `cand_residentes`, " +
                                "`indicacao_4makers`, `etnia`, `pessoa_refugiada`, `tipo_cargo`, `site_id` FROM candidate WHERE candidate_id = @candidate_id";
                    var candidate = db.QueryFirstOrDefault<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    return candidate != null ? new InsertCandidateDTO
                    {
                        candidate_id = candidate.candidate_id,
                        first_name = candidate.nomecompleto,
                        phone_home = candidate.phone_home,
                        phone_cell = candidate.phone_cell,
                        address = candidate.address,
                        address_number = candidate.address_number,
                        address_complement = candidate.address_complement,
                        district = candidate.district,
                        city = candidate.city,
                        state = candidate.state,
                        zip = candidate.zip,
                        source = candidate.source,
                        methodologies = candidate.methodologies,
                        email1 = candidate.email1,
                        email2 = candidate.email2,
                        emailFoursys = candidate.emailFoursys,
                        desired_pay = (decimal?)candidate.desired_pay,
                        current_pay = (decimal?)candidate.current_pay,
                        EhAtivo = (sbyte)candidate.is_active,
                        cand_rg = candidate.cand_rg,
                        Cand_cpf = candidate.cand_cpf,
                        cand_estCivil = candidate.cand_estCivil,
                        cand_Lkdin = candidate.cand_Lkdin,
                        cand_skype = candidate.cand_skype,
                        instagram = candidate.instagram,
                        facebook = candidate.facebook,
                        twitter = candidate.twitter,
                        cand_filhos = (int)candidate.cand_filhos,
                        EhFumante = candidate.cand_fumante,
                        disponibilidade = candidate.disponibilidade,
                        zona = candidate.zona,
                        uniresp = candidate.uniresp,
                        pcd = candidate.pcd ? (sbyte)1 : (sbyte)0,
                        genre = candidate.genre,
                        sexual_orientation = candidate.sexual_orientation,
                        cand_other_level = candidate.cand_other_level,
                        cand_esp_level = candidate.cand_esp_level,
                        cand_eng_level = candidate.cand_eng_level,
                        cand_modalidade = candidate.cand_modalidade,
                        cand_saude = candidate.cand_saude,
                        cand_gruporisco = candidate.cand_gruporisco,
                        cand_familiares = candidate.cand_familiares,
                        cand_residentes = candidate.cand_residentes,
                        indicacao_4makers = candidate.indicacao_4makers,
                        ethnicity = candidate.etnia,
                        refugee_person = candidate.pessoa_refugiada ? (sbyte)1 : (sbyte)0,
                        TipoCargo = candidate.tipo_cargo,
                        SiteId = (int)candidate.site_id,
                        dataNascimento = candidate.dataNascimento,
                        maquina_four = candidate.maquina_four,
                        maquina_cliente = candidate.maquina_cliente,
                        notes = candidate.notes,
                    } : null;
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

        public int? GetCandidateId(string linkedin)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@linkedin", linkedin);
                    var query = "SELECT `candidate_id` FROM candidate WHERE cand_Lkdin = @linkedin";
                    var candidate = db.QueryFirstOrDefault<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    return candidate != null ? candidate.candidate_id : null;
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

        public async Task<List<VagaDTO>> GetVagaInfra()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var vagasQuery = @"
                SELECT DISTINCT
                    jo.joborder_id AS IdVaga,
                    u.email AS Email,
                    jo.title AS Titulo,
                    jo.jo_stVaga AS Status,
                    jo.tipoVaga AS Tipo,
                    jo.openings AS NumeroDeVagas,
                    jo.rate_max AS TaxaMaximaPorHora,
                    jo.termometro AS NivelDeUrgencia,
                    jo.description AS Descricao,
                    jo.funcao AS Funcao,
                    jo.date_created AS DataCriacao,
                    jo.date_modified AS DataAtualizacao,
                    jo.start_date AS DataPublicacao,
                    ca.date_modified AS DataAceitacao,
                    jo.jo_locTrab AS TipoLocalizacao,
                    jo.state AS Estado,
                    jo.city AS Cidade
                FROM
                    joborder jo
                LEFT JOIN
                    user u ON jo.owner = u.user_id
                LEFT JOIN
                    controle_aprovacao ca ON jo.joborder_id = ca.joborder_id
                WHERE
                    ca.status_aprovacao = 'EM ANDAMENTO' OR ca.status_aprovacao IS NULL";

                    var vagas = await db.QueryAsync<VagaDTO>(vagasQuery);

                    var skillsQuery = @"
                SELECT
                    js.joborder_id AS VagaId,
                    js.category_id AS CategoriaId,
                    js.description_id AS DescricaoId,
                    js.nivel_id AS NivelId,
                    js.date_created AS DataCriacao,
                    js.date_modified AS DataAlteracao
                FROM
                    joborder_skills js ";

                    var skills = await db.QueryAsync<SkillsVagasSrsDTO>(skillsQuery);

                    var candidatosQuery = @"
                SELECT
                    cj.joborder_id AS VagaId,
                    cj.candidate_id AS CandidatoId,
                    c.first_name AS CandidatoNome,
                    c.email1 AS CandidatoEmail,
                    cjs.short_description AS Status
                FROM
                    candidate_joborder cj
                LEFT JOIN
                    candidate c ON cj.candidate_id = c.candidate_id
                LEFT JOIN
                    candidate_joborder_status cjs ON cj.status = cjs.candidate_joborder_status_id";

                    var candidatos = await db.QueryAsync<CandidatoVagaDTO>(candidatosQuery);

                    var skillsCandidatosQuery = @"
                SELECT
                    cs.candidate_id AS CandidatoId,
                    cs.category_id AS CategoriaId,
                    cs.description_id AS DescricaoId,
                    cs.nivel_id AS NivelId,
                    cs.date_created AS DataCriacao,
                    cs.date_modified AS DataAlteracao
                FROM
                    candidate_skills cs
                LEFT JOIN
                    candidate_joborder cj ON cs.candidate_id = cj.candidate_id";

                    var skillsCandidatos = await db.QueryAsync<SkillsCandidatosDTO>(skillsCandidatosQuery);

                    var vagasDic = vagas.ToDictionary(v => v.IdVaga, v => v);
                    foreach (var skill in skills)
                    {
                        if (vagasDic.TryGetValue(skill.VagaId, out var vaga))
                        {
                            vaga.Skills ??= new List<SkillsVagasSrsDTO>();
                            vaga.Skills.Add(skill);
                        }
                    }

                    foreach (var candidato in candidatos)
                    {
                        if (vagasDic.TryGetValue(candidato.VagaId, out var vaga))
                        {
                            vaga.Candidatos ??= new List<CandidatoVagaDTO>();
                            vaga.Candidatos.Add(candidato);
                            var candidatoSkills = skillsCandidatos.Where(sc => sc.CandidatoId == candidato.CandidatoId).ToList();
                            foreach (var candidatoSkill in candidatoSkills)
                            {
                                vaga.SkillsCandidatos ??= new List<SkillsCandidatosDTO>();
                                vaga.SkillsCandidatos.Add(candidatoSkill);
                            }
                        }
                    }

                    return vagasDic.Values.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao obter vagas da infraestrutura: {ex.Message}");
                }
            }
        }

        public void InsertCandidate(SRSInsertCandidateParam param, string cpf)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@first_name", param.first_name);
                    parametro.Add("@phone_home", param.phone_home);
                    parametro.Add("@phone_cell", param.phone_cell);
                    parametro.Add("@address", param.address);
                    parametro.Add("@address_number", param.address_number);
                    parametro.Add("@address_complement", param.address_complement);
                    parametro.Add("@district", param.district);
                    parametro.Add("@city", param.city);
                    parametro.Add("@state", param.state);
                    parametro.Add("@zip", param.zip);
                    parametro.Add("@source", param.source);
                    parametro.Add("@methodologies", param.methodologies);
                    parametro.Add("@email1", param.email1);
                    parametro.Add("@email2", param.email2);
                    parametro.Add("@emailFoursys", param.emailFoursys);
                    parametro.Add("@desired_pay", param.desired_pay);
                    parametro.Add("@current_pay", param.current_pay);
                    parametro.Add("@is_active", param.EhAtivo);
                    parametro.Add("@cand_rg", param.cand_rg);
                    parametro.Add("@cand_cpf_old", param.Cand_cpf);
                    parametro.Add("@cand_cpf", param.Cand_cpf);
                    parametro.Add("@cand_estCivil", param.cand_estCivil);
                    parametro.Add("@cand_Lkdin", param.cand_Lkdin);
                    parametro.Add("@cand_skype", param.cand_skype);
                    parametro.Add("@instagram", param.instagram);
                    parametro.Add("@facebook", param.facebook);
                    parametro.Add("@twitter", param.twitter);
                    parametro.Add("@cand_filhos", param.cand_filhos);
                    parametro.Add("@cand_fumante", param.EhFumante);
                    parametro.Add("@disponibilidade", param.disponibilidade);
                    parametro.Add("@dataNascimento", param.dataNascimento);
                    parametro.Add("@zona", param.zona);
                    parametro.Add("@uniresp", param.uniresp);
                    parametro.Add("@pcd", param.pcd);
                    parametro.Add("@genero", param.genre);
                    parametro.Add("@orientacao_sexual", param.sexual_orientation);
                    parametro.Add("@cand_other_level", param.cand_other_level);
                    parametro.Add("@cand_esp_level", param.cand_esp_level);
                    parametro.Add("@cand_eng_level", param.cand_eng_level);
                    parametro.Add("@cand_modalidade", param.cand_modalidade);
                    parametro.Add("@cand_saude", param.cand_saude);
                    parametro.Add("@cand_gruporisco", param.cand_gruporisco);
                    parametro.Add("@cand_familiares", param.cand_familiares);
                    parametro.Add("@cand_residentes", param.cand_residentes);
                    parametro.Add("@indicacao_4makers", param.indicacao_4makers);
                    parametro.Add("@etnia", param.ethnicity);
                    parametro.Add("@pessoa_refugiada", param.refugee_person);
                    parametro.Add("@tipo_cargo", param.TipoCargo);
                    parametro.Add("@site_id", param.site_id);
                    parametro.Add("@notes", param.notes);
                    parametro.Add("@maquina_four", param.maquina_four);
                    parametro.Add("@maquina_cliente", param.maquina_cliente);

                    string insert = "INSERT INTO candidate " +
                                    "(`nomecompleto`, `phone_home`, `phone_cell`, `address`, `address_number`, `address_complement`, " +
                                     "`district`, `city`, `state`, `zip`, `source`, `methodologies`, `email1`, `email2`, `emailFoursys`, " +
                                     "`desired_pay`, `current_pay`, `is_active`, `cand_rg`, `cand_cpf_old`, `cand_cpf`, `cand_estCivil`, " +
                                     "`cand_Lkdin`, `cand_skype`, `instagram`, `facebook`, `twitter`, `cand_filhos`, `cand_fumante`, " +
                                     "`disponibilidade`, `dataNascimento`, `zona`, `uniresp`, `pcd`, `genero`, `orientacao_sexual`, " +
                                     "`cand_other_level`, `cand_esp_level`, `cand_eng_level`, `cand_modalidade`, `cand_saude`, " +
                                     "`cand_gruporisco`, `cand_familiares`, `cand_residentes`, `indicacao_4makers`, `etnia`, " +
                                     "`pessoa_refugiada`, `tipo_cargo`, `site_id`) " +
                                     "VALUES " +
                                     "(@first_name, @phone_home, @phone_cell, @address, @address_number, @address_complement, " +
                                     "@district, @city, @state, @zip, @source, @methodologies, @email1, @email2, @emailFoursys, " +
                                     "@desired_pay, @current_pay, @EhAtivo, @cand_rg, @Cand_cpf, @Cand_cpf, @cand_estCivil, " +
                                     "@cand_Lkdin, @cand_skype, @instagram, @facebook, @twitter, @cand_filhos, @EhFumante, " +
                                     "@disponibilidade, @dataNascimento, @zona, @uniresp, @pcd, @genre, @sexual_orientation, " +
                                     "@cand_other_level, @cand_esp_level, @cand_eng_level, @cand_modalidade, @cand_saude, " +
                                     "@cand_gruporisco, @cand_familiares, @cand_residentes, @indicacao_4makers, @ethnicity, " +
                                     "@refugee_person, @TipoCargo, @SiteId);";

                    db.Execute(insert, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public bool ValidaSeJaExisteEmail(string email)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@email", email);
                    var emailExiste = db.Query<string>("Select email1, email2, emailFoursys from cats.candidate where email1 = @email or email2 = @email or emailFoursys = @email",
                        param: parametro, commandType: System.Data.CommandType.Text);

                    if (emailExiste != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EntrevistaTecnicaInsert(EntrevistaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var parametro = new DynamicParameters();

                        parametro.Add("@candidate_id", param.Candidate_id);
                        parametro.Add("@joborder_id", param.Joborder_id);
                        parametro.Add("@analista", param.Analista);
                        parametro.Add("@area", 2);
                        parametro.Add("@prcanalista", param.PrcAnalista);
                        parametro.Add("@dataEntrevista", param.DataEntrevista);
                        parametro.Add("@horaInicioEntrevista", param.HoraInicioEntrevista);
                        parametro.Add("@horaFinalEntrevista", param.HoraFinalEntrevista);
                        parametro.Add("@notesDPA", param.NotesDPA);

                        string queryInsertCandidate = "INSERT INTO `candidate_entrevista` (`candidate_id`, `joborder_id`, `analista`, `area`, `prcanalista`, `dataEntrevista`," +
                                                      " `horaInicioEntrevista`, `horaFinalEntrevista`, `notesDPA`) VALUES(@candidate_id, @joborder_id, @analista," +
                                                      "@area, @prcanalista, @dataEntrevista, @horaInicioEntrevista, @horaFinalEntrevista, @notesDPA)";

                        db.Execute(queryInsertCandidate, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        var entrevistaId = db.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

                        var candidateSkills = GetCandidateSkill(param.Candidate_id);

                        if (candidateSkills.Count() > 0)
                        {
                            foreach (var item in candidateSkills)
                            {
                                var parametroSkill = new DynamicParameters();

                                parametroSkill.Add("@joborder_id", param.Joborder_id);
                                parametroSkill.Add("@candidate_id", param.Candidate_id);
                                parametroSkill.Add("@category_id", item.Category_id);
                                parametroSkill.Add("@description_id", item.Description_id);
                                parametroSkill.Add("@nivel_id", item.Nivel_id);

                                string queryInsertSkillsTecnica = "INSERT INTO `candidate_tecnica_skills` (`tecnica_id`, `candidate_id`, `category_id`, `description_id`, `nivel_id`) " +
                                                "VALUES(@joborder_id, @candidate_id, @category_id, @description_id, @nivel_id)";

                                db.Execute(queryInsertSkillsTecnica, param: parametroSkill, transaction, commandType: System.Data.CommandType.Text);
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally { db.Close(); }
                }
            }
        }

        public void EntrevistaRHInsert(EntrevistaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var parametro = new DynamicParameters();

                        parametro.Add("@candidate_id", param.Candidate_id);
                        parametro.Add("@joborder_id", param.Joborder_id);
                        parametro.Add("@analista", param.Analista);
                        parametro.Add("@area", 1);
                        parametro.Add("@prcanalista", param.PrcAnalista);
                        parametro.Add("@dataEntrevista", param.DataEntrevista);
                        parametro.Add("@horaInicioEntrevista", param.HoraInicioEntrevista);
                        parametro.Add("@horaFinalEntrevista", param.HoraFinalEntrevista);
                        parametro.Add("@notesDPA", param.NotesDPA);

                        string queryInsertCandidate = "INSERT INTO `candidate_entrevista` (`candidate_id`, `joborder_id`, `analista`, `area`, `prcanalista`, `dataEntrevista`," +
                                                      " `horaInicioEntrevista`, `horaFinalEntrevista`, `notesDPA`) VALUES(@candidate_id, @joborder_id, @analista," +
                                                      "@area, @prcanalista, @dataEntrevista, @horaInicioEntrevista, @horaFinalEntrevista, @notesDPA)";

                        db.Execute(queryInsertCandidate, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        var entrevistaId = db.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

                        var candidateSkills = GetCandidateSkill(param.Candidate_id);

                        if (candidateSkills.Count > 0)
                        {
                            foreach (var item in candidateSkills)
                            {
                                var parametroSkill = new DynamicParameters();

                                parametroSkill.Add("@joborder_id", param.Joborder_id);
                                parametroSkill.Add("@candidate_id", param.Candidate_id);
                                parametroSkill.Add("@category_id", item.Category_id);
                                parametroSkill.Add("@description_id", item.Description_id);
                                parametroSkill.Add("@nivel_id", item.Nivel_id);

                                string queryInsertSkillsTecnica = "INSERT INTO `candidate_rh_skills` (`rh_id`, `candidate_id`, `category_id`, `description_id`, `nivel_id`) " +
                                                "VALUES(@joborder_id, @candidate_id, @category_id, @description_id, @nivel_id)";

                                db.Execute(queryInsertSkillsTecnica, param: parametroSkill, transaction, commandType: System.Data.CommandType.Text);
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally { db.Close(); }
                }
            }
        }

        public List<EntrevistaDTO> GetEntrevista(int candidateId)
        {
            var ret = new List<EntrevistaDTO>();
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    var query = "SELECT * FROM candidate_entrevista WHERE candidate_id = @candidate_id;";
                    var candidate = db.Query<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    if (candidate is null)
                    {
                        throw new Exception("Entrevista não encontrada");
                    }
                    var candidateSkills = GetCandidateSkill(candidateId);
                    if (candidateSkills.Count() > 0)
                    {
                        foreach (var item in candidate)
                        {
                            ret.Add(new EntrevistaDTO
                            {
                                id = item.id,
                                analista = item.analista,
                                Area = item.area,
                                candidate_id = item.candidate_id,
                                dataEntrevista = item.dataEntrevista,
                                horaFinalEntrevista = item.horaFinalEntrevista,
                                horaInicioEntrevista = item.horaInicioEntrevista,
                                joborder_id = item.joborder_id,
                                notesDPA = item.notesDPA,
                                tipo_ia = item.tipo_ia,
                                prcanalista = item.prcanalista,
                                Skills = candidateSkills,
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in candidate)
                        {
                            ret.Add(new EntrevistaDTO
                            {
                                id = item.id,
                                analista = item.analista,
                                Area = item.area,
                                candidate_id = item.candidate_id,
                                dataEntrevista = item.dataEntrevista,
                                horaFinalEntrevista = item.horaFinalEntrevista,
                                horaInicioEntrevista = item.horaInicioEntrevista,
                                joborder_id = item.joborder_id,
                                notesDPA = item.notesDPA,
                                tipo_ia = item.tipo_ia,
                                prcanalista = item.prcanalista
                            });
                        }
                    }
                    return ret;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<SoEntrevistaDTO> GetSoEntrevista(int candidateId)
        {
            var ret = new List<SoEntrevistaDTO>();
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    var query = "SELECT * FROM candidate_entrevista WHERE candidate_id = @candidate_id;";
                    var candidate = db.Query<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    if (candidate is null)
                    {
                        throw new Exception("Entrevista não encontrada");
                    }

                    foreach (var item in candidate)
                    {
                        ret.Add(new SoEntrevistaDTO
                        {
                            id = item.id,
                            analista = item.analista,
                            Area = item.area,
                            candidate_id = item.candidate_id,
                            dataEntrevista = item.dataEntrevista,
                            horaFinalEntrevista = item.horaFinalEntrevista,
                            horaInicioEntrevista = item.horaInicioEntrevista,
                            joborder_id = item.joborder_id,
                            notesDPA = item.notesDPA,
                            prcanalista = item.prcanalista
                        });
                    }
                    return ret;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<SoEntrevistaIdDTO> GetSoEntrevistaId(int entrevistaId, int candidateId)
        {
            var ret = new List<SoEntrevistaIdDTO>();
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@entrevistaId", entrevistaId);
                    var query = "SELECT * FROM candidate_entrevista WHERE id = @entrevistaId AND candidate_id = @candidate_id;";
                    var candidate = db.Query<dynamic>(query, param: parametro, commandType: System.Data.CommandType.Text);
                    if (candidate is null)
                    {
                        throw new Exception("Entrevista não encontrada");
                    }

                    foreach (var item in candidate)
                    {
                        ret.Add(new SoEntrevistaIdDTO
                        {
                            id = item.id,
                            analista = item.analista,
                            Area = item.area,
                            candidate_id = item.candidate_id,
                            dataEntrevista = item.dataEntrevista,
                            horaFinalEntrevista = item.horaFinalEntrevista,
                            horaInicioEntrevista = item.horaInicioEntrevista,
                            joborder_id = item.joborder_id,
                            notesDPA = item.notesDPA,
                            prcanalista = item.prcanalista,
                            tipo_ia = item.tipo_ia
                        });
                    }
                    return ret;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EntrevistaClienteGestorInsert(EntrevistaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();

                    parametro.Add("@candidate_id", param.Candidate_id);
                    parametro.Add("@joborder_id", param.Joborder_id);
                    parametro.Add("@analista", param.Analista);
                    parametro.Add("@area", 3);
                    parametro.Add("@prcanalista", param.PrcAnalista);
                    parametro.Add("@dataEntrevista", param.DataEntrevista);
                    parametro.Add("@horaInicioEntrevista", param.HoraInicioEntrevista);
                    parametro.Add("@horaFinalEntrevista", param.HoraFinalEntrevista);
                    parametro.Add("@notesDPA", param.NotesDPA);

                    string queryInsertCandidate = "INSERT INTO `candidate_entrevista` (`candidate_id`, `joborder_id`, `analista`, `area`, `prcanalista`, `dataEntrevista`," +
                                                    " `horaInicioEntrevista`, `horaFinalEntrevista`, `notesDPA`) VALUES(@candidate_id, @joborder_id, @analista," +
                                                    "@area, @prcanalista, @dataEntrevista, @horaInicioEntrevista, @horaFinalEntrevista, @notesDPA)";

                    db.Execute(queryInsertCandidate, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EditarEntrevista(EntrevistaDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@id", param.id);
                    parametro.Add("@candidate_id", param.candidate_id);
                    parametro.Add("@joborder_id", param.joborder_id);
                    parametro.Add("@analista", param.analista);
                    parametro.Add("@prcanalista", param.prcanalista);
                    parametro.Add("@dataEntrevista", param.dataEntrevista);
                    parametro.Add("@horaInicioEntrevista", param.horaInicioEntrevista);
                    parametro.Add("@horaFinalEntrevista", param.horaFinalEntrevista);
                    parametro.Add("@notesDPA", param.notesDPA);
                    parametro.Add("@tipo_ia", param.tipo_ia);

                    string queryEditaEntrevista = " UPDATE `candidate_entrevista` SET `candidate_id` = @candidate_id, `joborder_id` = @joborder_id, " +
                                                    "`analista` = @analista, `prcanalista` = @prcanalista, `dataEntrevista` = @dataEntrevista, " +
                                                    "`horaInicioEntrevista` = @horaInicioEntrevista, `horaFinalEntrevista` = @horaFinalEntrevista, " +
                                                    "`notesDPA` = @notesDPA, `tipo_ia` = @tipo_ia WHERE(`id` = @id);";

                    db.Execute(queryEditaEntrevista, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public int insertOffboarding(OffboardingDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    string queryOffboarding = "INSERT INTO `candidate_offboarding` (`candidate_id`, `desligado`, `dataDeslig`, `analistaDesl`, " +
                                                  "`semEntrevistaDeDesligamento`, `motivoDesligamento`, `outrosAnotacoesDesligamento`, `diretoriaDesl`, " +
                                                  "`gestorDesl`, `dataEntrevista`, `pergunta`, `resposta`, `linkFormIndividual`) " +
                                                  "VALUES(" + param.candidate_id + "," + param.desligado + ",'" + param.dataDeslig.ToString("yyyy-MM-dd") + "','" + param.analistaDesl +
                                                  "'," + param.semEntrevistaDeDesligamento + ",'" + param.motivoDesligamento + "','" + param.outrosAnotacoesDesligamento +
                                                  "','" + param.diretoriaDesl + "','" + param.gestorDesl + "','" + param.dataEntrevista.ToString("yyyy-MM-dd") + "','" + param.pergunta +
                                                  "','" + param.resposta + "','" + param.linkFormIndividual + "'); ";
                    db.Execute(queryOffboarding, param: parametro, commandType: System.Data.CommandType.Text);
                    var retId = db.Query<int>("Select MAX(candidate_offboarding_id) from candidate_offboarding where candidate_id = @CandidateId group by candidate_id", new { CandidateId = param.candidate_id });
                    return retId.First();
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EditarOffboarding(OffboardingDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    string queryOffboarding = "UPDATE `candidate_offboarding` SET `candidate_id` = '" + param.candidate_id + "', `desligado` = '" + param.desligado +
                                              "', `dataDeslig` = '" + param.dataDeslig.Date + "', `analistaDesl` = '" + param.analistaDesl +
                                              "', `semEntrevistaDeDesligamento` = '" + param.semEntrevistaDeDesligamento + "', `motivoDesligamento` = '" + param.motivoDesligamento +
                                              "', `outrosAnotacoesDesligamento` = '" + param.outrosAnotacoesDesligamento + "', `diretoriaDesl` = '" + param.diretoriaDesl +
                                              "', `gestorDesl` = '" + param.gestorDesl + "', `dataEntrevista` = '" + param.dataEntrevista.Date +
                                              "', `pergunta` = '" + param.pergunta + "', `resposta` = '" + param.resposta + "',`linkFormIndividual` = '" + param.linkFormIndividual +
                                              "' WHERE(`candidate_offboarding_id` = '" + param.id + "');";
                    db.Execute(queryOffboarding, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<OffboardingDTO> GetOffboarding(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<OffboardingDTO> ret = new List<OffboardingDTO>();
                    string queryOffboarding = "";
                    db.Open();
                    var parametro = new DynamicParameters();
                    if (candidateId > 0)
                    {
                        queryOffboarding = "SELECT * FROM `candidate_offboarding` where candidate_id = " + candidateId + ";";
                    }
                    else
                    {
                        queryOffboarding = "SELECT * FROM `candidate_offboarding`;";
                    }
                    var query = db.Query(queryOffboarding, param: parametro, commandType: System.Data.CommandType.Text);
                    foreach (var item in query)
                    {
                        ret.Add(new OffboardingDTO
                        {
                            id = item.candidate_offboarding_id,
                            analistaDesl = item.analistaDesl,
                            candidate_id = item.candidate_id,
                            dataDeslig = item.dataDeslig,
                            dataEntrevista = item.dataEntrevista,
                            desligado = item.desligado,
                            diretoriaDesl = item.diretoriaDesl,
                            gestorDesl = item.gestorDesl,
                            motivoDesligamento = item.motivoDesligamento,
                            outrosAnotacoesDesligamento = item.outrosAnotacoesDesligamento,
                            pergunta = item.pergunta,
                            resposta = item.resposta,
                            semEntrevistaDeDesligamento = item.semEntrevistaDeDesligamento,
                            linkFormIndividual = item.linkFormIndividual,
                        });
                    }
                    return ret;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertSkillCandidate(CandidateSkillParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", param.Candidate_id);
                    parametro.Add("@category_id", param.Category_id);
                    parametro.Add("@description_id", param.Description_id);
                    parametro.Add("@nivel_id", param.Nivel_id);
                    parametro.Add("@entrevista_id", param.EntrevistaId);
                    parametro.Add("@date_created", param.DateCreated);
                    parametro.Add("@date_modified", param.DateModified);
                    var querySkill = "INSERT INTO `candidate_skills` (`candidate_id`, `category_id`, `description_id`, `nivel_id`, `entrevista_id`, `date_created`, `date_modified`) " +
                                     "VALUES (@candidate_id, @category_id, @description_id, @nivel_id, @entrevista_id, @date_created, @date_modified)";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertCandidateCurriculum(int candidateId, string headline, string summary, string geolationname, DateTime dataCreated, DateTime dateModified)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@headline", headline);
                    parametro.Add("@summary", summary);
                    parametro.Add("@geolocationname", geolationname);
                    parametro.Add("@date_created", dataCreated);
                    parametro.Add("@date_modified", dateModified);
                    var querySkill = @"INSERT INTO candidate_curriculum
                        (candidate_id, headline, qualifications_abstract, geolocationname, date_created, date_modified)
                        VALUES(@candidate_id, @headline, @summary, @geolocationname, @date_created, @date_modified);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }
        public void UpdateCandidateCurriculum(int candidateId, string headline, string summary, string geolationname, DateTime dateModified)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@headline", headline);
                    parametro.Add("@summary", summary);
                    parametro.Add("@geolocationname", geolationname);
                    parametro.Add("@date_modified", dateModified);
                    var querySkill = @"UPDATE candidate_curriculum set headline = @headline, qualifications_abstract = @summary, geolocationname = @geolocationname, date_modified = @date_modified where candidate_id = @candidate_id";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertCandidateCurriculumCertification(int candidateId, string certificateName, string authority, DateTime? dateStarted, DateTime? dateEnded, DateTime dataCreated, DateTime dateModified)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@certificate_name", certificateName);
                    parametro.Add("@authority", authority);
                    parametro.Add("@date_started", dateStarted);
                    parametro.Add("@date_ended", dateEnded);
                    parametro.Add("@date_created", dataCreated);
                    parametro.Add("@date_modified", dateModified);
                    var querySkill = @"INSERT INTO candidate_curriculum_certification
                        (candidate_id, certificate_name, authority, date_started, date_ended, date_created, date_modified)
                        VALUES(@candidate_id, @certificate_name, @authority, @date_started, @date_ended, @date_created, @date_modified);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<string> GetCandidateCurriculumCertification(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<string> Companys = new List<string>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);

                    var querySkill = @"SELECT certificate_name FROM candidate_curriculum_certification
                        where candidate_id = @candidate_id;";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        Companys.Add(item.certificate_name);
                    }

                    return Companys;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertCandidateCurriculumCompany(int candidateId, string companyName, string description, string title, string locationName, DateTime? dateStarted, DateTime? dateEnded, bool empresaAtual, DateTime dataCreated, DateTime dateModified)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@company_name", companyName);
                    parametro.Add("@description", description);
                    parametro.Add("@role", title);
                    parametro.Add("@location_name", locationName);
                    parametro.Add("@date_started", dateStarted);
                    parametro.Add("@date_ended", dateEnded);
                    parametro.Add("@empresa_atual", empresaAtual == true ? 1 : 0);
                    parametro.Add("@date_created", dataCreated);
                    parametro.Add("@date_modified", dateModified);
                    var querySkill = @"INSERT INTO candidate_curriculum_company
                        (candidate_id, company_name, description, role, locationName, start_date, end_date, empresa_atual, date_created, date_modified)
                        VALUES(@candidate_id, @company_name, @description, @role, @location_name, @date_started, @date_ended, @empresa_atual, @date_created, @date_modified);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<string> GetCandidateCurriculumCompany(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<string> Companys = new List<string>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);

                    var querySkill = @"SELECT CONCAT(company_name, '|', role) as company_name FROM candidate_curriculum_company
                        where candidate_id = @candidate_id;";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        Companys.Add(item.company_name);
                    }

                    return Companys;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertCandidateCurriculumDegree(int candidateId, string degreeName, string fieldStudy, string schoolName, DateTime? dateStarted, DateTime? dateEnded, DateTime dataCreated, DateTime dateModified)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@degree_name", degreeName);
                    parametro.Add("@field_study", fieldStudy);
                    parametro.Add("@school_name", schoolName);
                    parametro.Add("@date_started", dateStarted);
                    parametro.Add("@date_ended", dateEnded);
                    parametro.Add("@date_created", dataCreated);
                    parametro.Add("@date_modified", dateModified);
                    var querySkill = @"INSERT INTO candidate_curriculum_degree
                        (candidate_id, degree_name, field_study, school_name, date_started, date_ended, date_created, date_modified)
                        VALUES(@candidate_id, @degree_name, @field_study, @school_name, @date_started, @date_ended, @date_created, @date_modified);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<string> GetCandidateCurriculumDegree(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<string> Companys = new List<string>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);

                    var querySkill = @"SELECT CONCAT(school_name, '|', COALESCE(degree_name, '')) as degree FROM candidate_curriculum_degree
                        where candidate_id = @candidate_id;";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        Companys.Add(item.degree);
                    }

                    return Companys;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertCandidateSkillEntrevista(EntrevistaSkillParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", param.Candidate_id);
                    parametro.Add("@entrevista_id", param.Entrevista_id);
                    parametro.Add("@category_id", param.Category_id);
                    parametro.Add("@description_id", param.Description_id);
                    parametro.Add("@nivel_id", param.Nivel_id);
                    var querySkill = "INSERT INTO `candidate_skills` (`candidate_id`, `entrevista_id`, `category_id`, `description_id`, `nivel_id`) " +
                                     "VALUES (@candidate_id, @entrevista_id, @category_id, @description_id, @nivel_id)";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EditarSkillCandidate(CandidateSkillDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@id", param.Id);
                    parametro.Add("@candidate_id", param.Candidate_id);
                    parametro.Add("@category_id", param.Category_id);
                    parametro.Add("@description_id", param.Description_id);
                    parametro.Add("@nivel_id", param.Nivel_id);
                    var querySkill = "UPDATE `candidate_skills` SET `candidate_id` = @candidate_id, `category_id` = @category_id " +
                                     "`description_id` = @description_id, `nivel_id` = @nivel_id WHERE (`id` = @id);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void RemoveSkillCandidate(int skillId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@id", skillId);
                    var querySkill = "DELETE FROM `candidate_skills` WHERE (`id` = @id);";

                    var query = db.Execute(querySkill, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<CandidateSkillDTO> GetCandidateSkill(int candidate_id)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<CandidateSkillDTO> candidateSkills = new List<CandidateSkillDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidate_id);
                    var querySkill = "SELECT * FROM candidate_skills WHERE candidate_id = @candidate_id;";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        candidateSkills.Add(new CandidateSkillDTO
                        {
                            Id = item.id != null ? item.id : 0,
                            Candidate_id = item.candidate_id != null ? item.candidate_id : 0,
                            Category_id = item.category_id != null ? item.category_id : 0,
                            Description_id = item.description_id != null ? item.description_id : 0,
                            Nivel_id = item.nivel_id != null ? item.nivel_id : 0,
                            Date_Created = item.date_created != null ? item.date_created : new Nullable<DateTime>(),
                            Date_Modified = item.date_modified != null ? item.date_modified : new Nullable<DateTime>(),
                            Entrevista_id = item.entrevista_id != null ? item.entrevista_id : 0,
                        });
                    }
                    return candidateSkills;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<CandidateSkillDTO> GetCandidateSkillEntrevista(int candidateId, int entrevistaId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<CandidateSkillDTO> candidateSkills = new List<CandidateSkillDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    parametro.Add("@entrevista_id", entrevistaId);
                    var querySkill = "SELECT * FROM candidate_skills WHERE candidate_id = @candidate_id AND entrevista_id = @entrevista_id;";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        candidateSkills.Add(new CandidateSkillDTO
                        {
                            Id = item.id,
                            Candidate_id = item.candidate_id,
                            Category_id = item.category_id,
                            Description_id = item.description_id,
                            Nivel_id = item.nivel_id
                        });
                    }
                    return candidateSkills;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void InsertOnboarding(OnboardingParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryOnboarding = "INSERT INTO `candidate_onboarding` (`candidate_id`, `date_start`, `project_date_start`, `project_name`, `onboarding_group_id`) " +
                                          "VALUES ('" + param.Candidate_id + "', '" + param.Date_start + "', '" + param.Project_date_start + "', '" + param.Project_name + "', '" + param.Oboarding_group_id + "');";

                    var query = db.Execute(queryOnboarding, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<OnboardingDTO> GetOnboarding(int candidate_id)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<OnboardingDTO> onboarding = new List<OnboardingDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    var querySkill = "SELECT * FROM candidate_onboarding WHERE candidate_id = " + candidate_id + ";";

                    var query = db.Query(querySkill, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        onboarding.Add(new OnboardingDTO
                        {
                            Candidate_onboarding_id = item.candidate_onboarding_id,
                            Candidate_id = item.candidate_id,
                            Date_start = item.date_start,
                            Project_date_start = item.project_date_start,
                            Project_name = item.project_name,
                            Oboarding_group_id = item.onboarding_group_id
                        });
                    }
                    return onboarding;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<HistoricoCandidateEntrevistaDTO> GetHistoricoCandidatoEntrevistas(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<HistoricoCandidateEntrevistaDTO> historico = new List<HistoricoCandidateEntrevistaDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", candidateId);
                    var query = "SELECT * FROM candidate_cliente_gestor_entrevista WHERE candidate_id = @candidate_id;";

                    var result = db.Query(query, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in result)
                    {
                        historico.Add(new HistoricoCandidateEntrevistaDTO
                        {
                            id = item.id,
                            candidate_id = item.candidate_id,
                            joborder_id = item.joborder_id,
                            analista = item.analista,
                            prcanalista = item.prcanalista,
                            dataEntrevista = item.dataEntrevista,
                            horaInicioEntrevista = item.horaInicioEntrevista,
                            horaFinalEntrevista = item.horaFinalEntrevista,
                            notesDPA = item.notesDPA
                        });
                    }
                    return historico;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void HistoricoCandidatoEntrevista(HistoricoCandidateEntrevistaDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", param.candidate_id);
                    parametro.Add("@joborder_id", param.joborder_id);
                    parametro.Add("@analista", param.analista);
                    parametro.Add("@prcanalista", param.prcanalista);
                    parametro.Add("@dataEntrevista", param.dataEntrevista);
                    parametro.Add("@horaInicioEntrevista", param.horaInicioEntrevista);
                    parametro.Add("@horaFinalEntrevista", param.horaFinalEntrevista);
                    parametro.Add("@notesDPA", param.notesDPA);

                    var stringHistorico = "INSERT INTO `candidate_cliente_gestor_entrevista` (`candidate_id`, `joborder_id`, `analista`, `prcanalista`, `dataEntrevista`, " +
                                        "`horaInicioEntrevista`, `horaFinalEntrevista`, `notesDPA`) " +
                                        "VALUES (@candidate_id, @joborder_id, @analista, @prcanalista, @dataEntrevista, " +
                                        "@horaInicioEntrevista, @horaFinalEntrevista, @notesDPA);";

                    var queryCriaVaga = db.Execute(stringHistorico, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public HistoricoCandidateEntrevistaDTO GetHistoricoCandidatoEntrevistaEspecifica(int id)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    HistoricoCandidateEntrevistaDTO historico = new HistoricoCandidateEntrevistaDTO();
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@id", id);
                    var query = "SELECT * FROM candidate_cliente_gestor_entrevista WHERE id = @id;";

                    var result = db.Query(query, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in result)
                    {
                        historico = new HistoricoCandidateEntrevistaDTO
                        {
                            id = item.id,
                            candidate_id = item.candidate_id,
                            joborder_id = item.joborder_id,
                            analista = item.analista,
                            prcanalista = item.prcanalista,
                            dataEntrevista = item.dataEntrevista,
                            horaInicioEntrevista = item.horaInicioEntrevista,
                            horaFinalEntrevista = item.horaFinalEntrevista,
                            notesDPA = item.notesDPA
                        };
                    }
                    return historico;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void EditarOnboarding(OnboardingDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryOnboarding = "UPDATE `cats`.`candidate_onboarding` SET `candidate_id` = '" + param.Candidate_id + "', `date_start` = '" + param.Date_start.Date +
                                          "', `project_date_start` = '" + param.Project_date_start.Date + "', `project_name` = '" + param.Project_name +
                                          "', `onboarding_group_id` = '" + param.Oboarding_group_id + "' WHERE (`candidate_onboarding_id` = '" + param.Candidate_onboarding_id + "');";

                    var query = db.Execute(queryOnboarding, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public void RemoveOnboarding(int onboardingId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryOnboarding = "DELETE FROM `candidate_onboarding` WHERE (`candidate_onboarding_id` = '" + onboardingId + "');";

                    var query = db.Execute(queryOnboarding, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<CargoSimplesDTO> GetCargoSimples()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<CargoSimplesDTO> cargos = new List<CargoSimplesDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryCargo = "SELECT idcandidate_cargo_id, papel FROM cargo;";

                    var query = db.Query(queryCargo, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        cargos.Add(new CargoSimplesDTO
                        {
                            idcandidate_cargo_id = item.idcandidate_cargo_id,
                            papel = item.papel
                        });
                    }
                    return cargos;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public CargoDTO GetCargo(int cargoId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    CargoDTO cargo = new CargoDTO();
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryCargo = "SELECT `cargo_kenoby_id`, `papel`, `responsabilidade`, `conhecimento_tecnico`, `habilidades_comportamentais`," +
                                     "`conhecimentos_metodologia`, `formacao_academica`, `certificacoes`, `experiencia`, `equipamentos`" +
                                     "FROM `cargo` where `idcandidate_cargo_id` = " + cargoId + ";";

                    var query = db.QueryFirstOrDefault<dynamic>(queryCargo, param: parametro, commandType: System.Data.CommandType.Text);

                    cargo = new CargoDTO
                    {
                        cargo_kenoby_id = query.cargo_kenoby_id,
                        papel = query.papel,
                        certificacoes = query.certificacoes,
                        conhecimentos_metodologia = query.conhecimentos_metodologia,
                        conhecimento_tecnico = query.conhecimento_tecnico,
                        equipamentos = query.equipamentos,
                        experiencia = query.experiencia,
                        formacao_academica = query.formacao_academica,
                        habilidades_comportamentais = query.habilidades_comportamentais,
                        responsabilidade = query.responsabilidade,
                    };

                    return cargo;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public List<LocalDeTrabalhoDTO> GetLocalDeTrabalho()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    List<LocalDeTrabalhoDTO> localDeTrabalho = new List<LocalDeTrabalhoDTO>();
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryLocal = "SELECT * FROM tipo_local_trabalho;";

                    var query = db.Query(queryLocal, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in query)
                    {
                        localDeTrabalho.Add(new LocalDeTrabalhoDTO
                        {
                            cidade = item.cidade,
                            descricao = item.descricao,
                            desc_completa = item.desc_completa,
                            id = item.id,
                            uf = item.uf,
                        });
                    }
                    return localDeTrabalho;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public int UnidadeFourmakersToSRS(int idUnidade)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    var queryLocal = "SELECT unidade_id FROM unidades where id_fourmakers = " + idUnidade + ";";
                    var query = db.QueryFirstOrDefault(queryLocal, param: parametro, commandType: System.Data.CommandType.Text);

                    return query.unidade_id;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public async Task<CriarVagasSRSParam> CriarOuAtualizarVagaFourmakersSRS(CriarVagasSRSParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        int? companyId = db.QueryFirstOrDefault<int?>("select company_id from company where company_crm_id = @CodigoCRM", param: new { CodigoCRM = param.Company.CodigoCRM }, transaction, commandType: System.Data.CommandType.Text);

                        if (companyId == null)
                        {
                            companyId = db.ExecuteScalar<int>("insert into company (company_crm_id, name, site_id) values (@CodigoCRM, @Nome, @SiteId); select last_insert_id();", param: new { CodigoCRM = param.Company.CodigoCRM, Nome = param.Company.Nome, SiteId = 1 }, transaction, commandType: System.Data.CommandType.Text);
                        }

                        var parametro = new DynamicParameters();

                        parametro.Add("@idVaga", param.IdVaga);
                        parametro.Add("@owner", param.Solicitante);
                        parametro.Add("@cargo", param.Cargo);
                        parametro.Add("@title", param.Titulo);
                        parametro.Add("@tipoVaga", param.Tipo);
                        parametro.Add("@valor_limite", param.TaxaMaximaPorHora);
                        parametro.Add("@city", param.Cidade.IsEmpty() ? "São Paulo" : param.Cidade);
                        parametro.Add("@openings", param.NumeroDeVagas);
                        parametro.Add("@state", param.Estado.IsEmpty() ? "SP" : param.Estado);
                        parametro.Add("@jo_locTrab", param.TipoLocalizacao);
                        parametro.Add("@uniresp", param.Unidade);
                        parametro.Add("@entered_by", param.IdUsuario);
                        parametro.Add("@email_uniresp", param.Aprovador);
                        parametro.Add("@termometro", param.Termometro);
                        parametro.Add("@hardskill_id", param.StackPrincipal.IsEmpty() ? "0" : param.StackPrincipal );
                        parametro.Add("@equipamento_id", param.ConfiguracaoMaquina.IsEmpty() ? "0" : param.ConfiguracaoMaquina );
                        parametro.Add("@duracao_contrato", param.DuracaoContrato);
                        parametro.Add("@duracao_contrato_determinado", param.DuracaoContratoDeterminado);
                        parametro.Add("@company_id", companyId);
                        parametro.Add("@contact_id", 0);
                        parametro.Add("@notes", param.Notes + "\n" + param.GestorFoursys + "\nCriado pelo Fourmakers");
                        parametro.Add("@data_prev_inicio", param.DataPrevistaInicio);
                        parametro.Add("@profissional_interno_id", 0);
                        parametro.Add("@substitucao_colaborador", "N");
                        parametro.Add("@salary", 0);
                        parametro.Add("@company_department_id", 0);
                        parametro.Add("@jo_areaCliente", "");
                        parametro.Add("@senioridade", "");
                        parametro.Add("@rate_max", "");
                        parametro.Add("@descricaoCargo", param.DescricaoCargo);
                        parametro.Add("@descricaoTecnica", param.DescricaoTecnica);
                        parametro.Add("@acrescimoPorcentagem", param.Acrescimo ?? 0);
                        parametro.Add("@cargaHoraria", param.CargaHoraria);
                        parametro.Add("@frequencia", param.Frequencia);
                        parametro.Add("@maquina_cliente", param.MaquinaCliente.IsEmpty() ? "0" : param.MaquinaCliente );
                        parametro.Add("@maquina_four", param.MaquinaFour.IsEmpty() ? "0" : param.MaquinaFour);

                        if (param.CrmInfo != null)
                        {
                            parametro.Add("@codigo_crm", param.CrmInfo.PropostaOportunidadeCCRM);
                            parametro.Add("@responsavel_cliente", param.CrmInfo.NomeContato);
                            parametro.Add("@email_responsavel_cliente", param.CrmInfo.EmailContato);
                            parametro.Add("@fone_responsavel_cliente", param.CrmInfo.TelefoneContato);
                            parametro.Add("@cpEmails_responsavel_cliente", param.CrmInfo.CopiaEmailContato);
                        }

                        if (param.IdVaga.HasValue && param.IdVaga > 0)
                        {
                            // Atualização da vaga existente
                            string updateJobOrder = @"
                                                        UPDATE `joborder`
                                                        SET
                                                            `owner` = @owner,
                                                            `cargo_id` = @cargo,
                                                            `title` = @title,
                                                            `type` = @tipovaga,
                                                            `valor_limite` = @valor_limite,
                                                            `city` = @city,
                                                            `state` = @state,
                                                            `jo_locTrab` = @jo_locTrab,
                                                            `uniresp` = @uniresp,
                                                            `date_prev_inicio` = @data_prev_inicio,
                                                            `profissional_interno_id` = @profissional_interno_id,
                                                            `substituicao_colaborador` = @substitucao_colaborador,
                                                            `salary` = @salary,
                                                            `company_department_id` = @company_department_id,
                                                            `jo_areaCliente` = @jo_areaCliente,
                                                            `senioridade` = @senioridade,
                                                            `rate_max` = @rate_max,
                                                            `company_id` = @company_id,
                                                            `contact_id` = @contact_id,
                                                            `notes` = @notes,
                                                            `entered_by` = @entered_by,
                                                            `email_uniresp` = @email_uniresp,
                                                            `termometro` = @termometro,
                                                            `hardskill_id` = @hardskill_id,
                                                            `equipamento_id` = @equipamento_id,
                                                            `duracao_contrato` = @duracao_contrato,
                                                            `duracao_contrato_determinado` = @duracao_contrato_determinado,
                                                            `cargo` = @descricaoCargo,
                                                            `detalhe_tecnico_cliente` = @descricaoTecnica,
                                                            `acrescimoPorcentagem` = @acrescimoPorcentagem,
                                                            `openings` = @openings,
                                                            `cargaHoraria` = @cargaHoraria,
                                                            `frequencia` = @frequencia,
                                                            `codigo_crm` = @codigo_crm,
                                                            `responsavel_cliente` = @responsavel_cliente,
                                                            `email_responsavel_cliente` = @email_responsavel_cliente,
                                                            `fone_responsavel_cliente` = @fone_responsavel_cliente,
                                                            `cpEmails_responsavel_cliente` = @cpEmails_responsavel_cliente,
                                                            `maquina_cliente` = @maquina_cliente,
                                                            `maquina_four` = @maquina_four,
                                                            `date_modified` = NOW()
                                                        WHERE
                                                            `joborder_id` = @idVaga;";

                            db.Execute(updateJobOrder, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                            db.Execute("DELETE FROM `joborder_skills` WHERE `joborder_id` = @idVaga;", param: parametro, transaction, commandType: System.Data.CommandType.Text);
                            db.Execute("DELETE FROM `joborder_tipo_contratacao` WHERE `joborder_id` = @idVaga;", param: parametro, transaction, commandType: System.Data.CommandType.Text);
                        }
                        else
                        {
                            var stringCriaVaga = @"INSERT INTO `joborder` (
                                            `owner`,
                                            `cargo_id`,
                                            `title`,
                                            `type`,
                                            `valor_limite`,
                                            `city`,
                                            `state`,
                                            `jo_stVaga`,
                                            `jo_locTrab`,
                                            `uniresp`,
                                            `complex_joborder`,
                                            `date_prev_inicio`,
                                            `profissional_interno_id`,
                                            `substituicao_colaborador`,
                                            `salary`,
                                            `company_department_id`,
                                            `jo_areaCliente`,
                                            `senioridade`,
                                            `rate_max`,
                                            `company_id`,
                                            `contact_id`,
                                            `notes`,
                                            `entered_by`,
                                            `email_uniresp`,
                                            `termometro`,
                                            `hardskill_id`,
                                            `equipamento_id`,
                                            `duracao_contrato`,
                                            `duracao_contrato_determinado`,
                                            `cargo`,
                                            `detalhe_tecnico_cliente`,
                                            `acrescimoPorcentagem`,
                                            `openings`,
                                            `cargaHoraria`,
                                            `frequencia`,
                                            `codigo_crm`,
                                            `responsavel_cliente`,
                                            `email_responsavel_cliente`,
                                            `fone_responsavel_cliente`,
                                            `cpEmails_responsavel_cliente`,
                                            `status`,
                                            `site_id`,
                                            `maquina_cliente`,
                                            `maquina_four`,
                                            `jo_dtabvaga`,
                                            `date_created`,
                                            `date_modified`
                                        ) VALUES (
                                            @owner,
                                            @cargo,
                                            @title,
                                            @tipovaga,
                                            @valor_limite,
                                            @city,
                                            @state,
                                            'EM APROVAÇÃO',
                                            @jo_locTrab,
                                            @uniresp,
                                            '1',
                                            @data_prev_inicio,
                                            @profissional_interno_id,
                                            @substitucao_colaborador,
                                            @salary,
                                            @company_department_id,
                                            @jo_areaCliente,
                                            @senioridade,
                                            @rate_max,
                                            @company_id,
                                            @contact_id,
                                            @notes,
                                            @entered_by,
                                            @email_uniresp,
                                            @termometro,
                                            @hardskill_id,
                                            @equipamento_id,
                                            @duracao_contrato,
                                            @duracao_contrato_determinado,
                                            @descricaoCargo,
                                            @descricaoTecnica,
                                            @acrescimoPorcentagem,
                                            @openings,
                                            @cargaHoraria,
                                            @frequencia,
                                            @codigo_crm,
                                            @responsavel_cliente,
                                            @email_responsavel_cliente,
                                            @fone_responsavel_cliente,
                                            @cpEmails_responsavel_cliente,
                                            'Active',
                                            1,
                                            @maquina_cliente,
                                            @maquina_four,
                                            NOW(),
                                            NOW(),
                                            NOW()
                                        );";

                            var queryCriaVaga = db.Execute(stringCriaVaga, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                            var buscaIdVaga = await db.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();", transaction);

                            param.IdVaga = buscaIdVaga;

                            parametro.Add("@idVaga", param.IdVaga);
                            var queryAtualizaClientJobId = db.Execute("UPDATE joborder SET client_job_id = @idVaga WHERE joborder_id = @idVaga;", param: parametro, transaction, commandType: System.Data.CommandType.Text);

                            string stringControleAprovacao = "INSERT INTO controle_aprovacao (`joborder_id`, `diretoria_id`, `status_aprovacao`, `flag`) " +
                                                            "VALUES(@idVaga, @uniresp, 'EM APROVAÇÃO', '1');";

                            var queryControleAprovacao = db.Execute(stringControleAprovacao, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                            string stringControleLeadTime = "INSERT INTO `controle_aprovacao_leadtime` (`joborder_id`) " +
                                                           "VALUES(@idVaga);";

                            var queryControleLeadTime = db.Execute(stringControleLeadTime, param: parametro, transaction, commandType: System.Data.CommandType.Text);
                        }

                        foreach (var item in param.Skills)
                        {
                            var parametroSkill = new DynamicParameters();
                            parametroSkill.Add("@idVaga", param.IdVaga);
                            parametroSkill.Add("@description_id", item.DescricaoId);
                            parametroSkill.Add("@nivel_id", item.NivelId);
                            parametroSkill.Add("@category_id", item.CategoriaId ?? CompetenciaUtils.ConverterItemPerfilIdParaTipoCompetenciaSRSId((int)item.CategoriaIdFourmakers));

                            string insertJobOrderSkill = "INSERT INTO `joborder_skills` (`joborder_id`, `category_id`, `description_id`, `nivel_id`) " +
                                                         "VALUES (@idVaga, @category_id, @description_id, @nivel_id);";

                            db.Execute(insertJobOrderSkill, param: parametroSkill, transaction, commandType: System.Data.CommandType.Text);
                        }

                        var indice = 0;
                        foreach (var item in param.TipoContratacao)
                        {
                            indice++;
                            var parametroTipoContratacao = new DynamicParameters();
                            parametroTipoContratacao.Add("@idVaga", param.IdVaga);
                            parametroTipoContratacao.Add("@tipoContratacao", item);
                            parametroTipoContratacao.Add("@tipo", indice);

                            string insertJobOrderTipoContratacao = @"INSERT INTO `joborder_tipo_contratacao` (`joborder_id`, `tipo_contratacao_id`, `tipo`)
                                                                   VALUES (@idVaga, @tipoContratacao, @tipo);";

                            db.Execute(insertJobOrderTipoContratacao, param: parametroTipoContratacao, transaction, commandType: System.Data.CommandType.Text);
                        }

                        transaction.Commit();

                        var jobOrderDates = await db.QueryFirstOrDefaultAsync<dynamic>(@"
                                                                                        SELECT
                                                                                            date_created AS DataCriacao,
                                                                                            date_modified AS DataAtualizacao
                                                                                        FROM
                                                                                            joborder
                                                                                        WHERE
                                                                                            joborder_id = @idVaga;",
                        param: parametro,
                        commandType: System.Data.CommandType.Text);

                        if (jobOrderDates != null)
                        {
                            param.DataCriacao = jobOrderDates.DataCriacao;
                            param.DataAtualizacao = jobOrderDates.DataAtualizacao;
                        }

                        return param;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Ocorreu um erro ao criar ou atualizar a vaga.", ex);
                    }
                    finally
                    {
                        db.Close();
                    }
                }
            }
        }

        public async Task<CriarVagasSRSParam> ObterVagaPorId(int vagaId)
        {
            string sql = @$"
                            SELECT
                                j.joborder_id AS IdVaga,
                                j.owner AS IdUsuario,
                                j.uniresp AS Unidade,
                                j.title AS Titulo,
                                j.type AS Tipo,
                                j.openings AS NumeroDeVagas,
                                j.valor_limite AS TaxaMaximaPorHora,
                                j.cargo_id AS Cargo,
                                j.cargo AS DescricaoCargo,
                                IFNULL(j.detalhe_tecnico_cliente, '') AS DescricaoTecnica,
                                IFNULL(j.acrescimoPorcentagem, 0) AS Acrescimo,
                                j.profissional_interno_id AS GestorFoursys,
                                j.jo_dtabvaga AS DataCriacao,
                                j.email_uniresp as Aprovador,
                                j.date_prev_inicio AS DataPrevistaInicio,
                                j.date_modified AS DataAtualizacao,
                                j.jo_locTrab AS TipoLocalizacao,
                                j.owner AS Solicitante,
                                j.termometro AS Termometro,
                                j.hardskill_id AS StackPrincipal,
                                j.equipamento_id AS ConfiguracaoMaquina,
                                j.duracao_contrato AS DuracaoContrato,
                                CAST(IFNULL(j.duracao_contrato_determinado, NULL) AS DECIMAL(10, 2)) AS DuracaoContratoDeterminado,
                                j.cargaHoraria AS CargaHoraria,
                                j.frequencia AS Frequencia,
                                j.state AS Estado,
                                j.city AS Cidade,
                                j.maquina_cliente as MaquinaCliente,
                                j.maquina_four as MaquinaFour,
                                j.notes as Notes,
                                c.company_crm_id AS CodigoCRM,
                                c.name AS Nome,
                                j.codigo_crm as PropostaOportunidadeCCRM,
                                j.responsavel_cliente as NomeContato,
                                j.email_responsavel_cliente as EmailContato,
                                j.fone_responsavel_cliente as TelefoneContato,
                                j.cpEmails_responsavel_cliente as CopiaEmailContato
                            FROM
                                cats.joborder j
                            LEFT JOIN
                                cats.company c ON j.company_id = c.company_id
                            WHERE
                                j.joborder_id = @IdVaga;";

            try
            {
                using (var _connectionSRS = new MySqlConnection(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("SRS_STRING_CONNECTION")))
                {
                    await _connectionSRS.OpenAsync();

                    var vagasResult = await _connectionSRS.QueryAsync<CriarVagasSRSParam, CompanySRSParam, CrmInfoSRSParam, CriarVagasSRSParam>(
                        sql,
                        (vaga, company, crmInfo) =>
                        {
                            vaga.Company = company;
                            vaga.CrmInfo = crmInfo;
                            return vaga;
                        },
                        new { IdVaga = vagaId },
                        splitOn: "CodigoCRM, PropostaOportunidadeCCRM"
                    );

                    var vagaResult = vagasResult.FirstOrDefault();

                    var skills = await ObterSkillsPorVaga(vagaResult.IdVaga.ToIntOuZero(), _connectionSRS);
                    var tiposContratacoes = await ObterTiposContratacaoPorVaga(vagaResult.IdVaga.ToIntOuZero(), _connectionSRS);
                    vagaResult.TipoContratacao = tiposContratacoes.ToList();

                    if (vagaResult != null)
                    {
                        vagaResult.Skills = skills.Select(skill => new SkillsVagasSrsDTO
                        {
                            CategoriaIdFourmakers = CompetenciaUtils.ConverterTipoCompetenciaSRSIdParaItemPerfilId(skill.CategoriaId.ToIntOuZero()),
                            CategoriaId = skill.CategoriaId,
                            DescricaoId = skill.DescricaoId,
                            NivelId = skill.NivelId
                        }).ToList();
                    }

                    return vagaResult;
                }
            }
            catch (Exception ex)
            {
                // Tratar o erro conforme necessário
                throw;
            }
        }

        private async Task<IEnumerable<SkillsVagasSrsDTO>> ObterSkillsPorVaga(int vagaId, MySqlConnection connection)
        {
            string sqlSkills = @"
                                        SELECT
                                            js.category_id AS CategoriaId,
                                            js.description_id AS DescricaoId,
                                            js.nivel_id AS NivelId
                                        FROM
                                            cats.joborder_skills js
                                        WHERE
                                            js.joborder_id = @IdVaga;";

            return await connection.QueryAsync<SkillsVagasSrsDTO>(sqlSkills, new { IdVaga = vagaId });
        }

        private async Task<IEnumerable<string>> ObterTiposContratacaoPorVaga(int vagaId, MySqlConnection connection)
        {
            const string query = @"
                                    SELECT
                                       tc.tipo_contratacao_id
                                    FROM
                                        cats.tipo_contratacao tc
                                    INNER JOIN
                                         cats.joborder_tipo_contratacao jotc ON tc.tipo_contratacao_id = jotc.tipo_contratacao_id
                                    WHERE
                                        jotc.joborder_id = @IdVaga;
                                ";

            return await connection.QueryAsync<string>(query, new { IdVaga = vagaId });
        }

        public int? BuscaUserId(string email)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                var parametro = new DynamicParameters();
                parametro.Add("@email", email);
                var queryUser = "SELECT user_id from user where email = @email;";

                var query = db.QueryFirstOrDefault(queryUser, param: parametro, commandType: System.Data.CommandType.Text);

                return query?.user_id;
            }
        }

        public async Task<CriarVagasParam> EditarVagasFourmakersSRS(CriarVagasParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string dataAcceptance = param.DataAceitacao.ToString("dd-MM-yyyy-hh-mm-ss");
                        string dataPublish = param.DataPublicacao.ToString("dd-MM-yyyy-hh-mm-ss");

                        var parametro = new DynamicParameters();

                        parametro.Add("@joborder_id", param.IdVaga);
                        parametro.Add("@owner", param.IdUsuario);
                        parametro.Add("@cargo", param.Funcao);
                        parametro.Add("@title", param.Titulo);
                        parametro.Add("@description", param.Descricao);
                        parametro.Add("@tipoVaga", param.Tipo);
                        parametro.Add("@valor_limite", param.TaxaMaximaPorHora);
                        parametro.Add("@city", param.Cidade);
                        parametro.Add("@state", param.Estado);
                        parametro.Add("@jo_stVaga", param.Status);
                        parametro.Add("@jo_locTrab", param.TipoLocalizacao);
                        parametro.Add("@uniresp", param.Unidade);
                        parametro.Add("@dataModificacao", DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"));
                        parametro.Add("@maquina_cliente", param.MaquinaCliente);
                        parametro.Add("@maquina_four", param.MaquinaFour);

                        string EditarVaga = "UPDATE `joborder` " +
                                            "SET `owner` = @owner, `cargo` = @cargo, `title` = @title,`valor_limite` = @valor_limite, `city` = @city, " +
                                            "`state` = @state, `jo_stVaga` = @jo_stVaga, `jo_locTrab` = @jo_locTrab, " +
                                            "`uniresp` = @uniresp, `complex_joborder` = '1' " +
                                            "WHERE(`joborder_id` = '@joborder_id');";

                        var queryEditarVaga = db.Execute(EditarVaga, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        string stringControleAprovacao = "UPDATE `controle_aprovacao` " +
                                                         "SET `joborder_id` = @idVaga, `diretoria_id` = @uniresp, `status_aprovacao` = @jo_stVaga ";

                        var queryControleAprovacao = db.Execute(stringControleAprovacao, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        string stringControleLeadTime = "UPDATE `controle_aprovacao_leadtime` SET `date_modified` = @dataModificacao";

                        var queryControleLeadTime = db.Execute(stringControleLeadTime, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        transaction.Commit();
                        return param;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception("Ocorreu um erro ao Editar a vaga.", ex);
                    }
                    finally { db.Close(); }
                }
            }
        }

        public async Task<SkillVagaParam> EditarSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();

                    parametro.Add("@joborder_id", param.Joborder_id);
                    parametro.Add("@category_id", param.TypeSkills);
                    parametro.Add("@description_id", param.SkillId);
                    parametro.Add("@nivel_id", param.SkillNivelId);
                    parametro.Add("@id", param.Id);

                    string EditarSkill = "UPDATE `joborder_skills` " +
                                        "SET `category_id` = @category_id, `description_id` = @description_id, `nivel_id` = @nivel_id " +
                                        "WHERE(`id` = @id);";

                    var queryEditarSkill = db.Execute(EditarSkill, param: parametro, commandType: System.Data.CommandType.Text);

                    return param;
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao Editar a Skill.", ex);
                }
                finally { db.Close(); }
            }
        }

        public async Task<SkillVagaParam> InserirSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();

                    parametro.Add("@joborder_id", param.Joborder_id);
                    parametro.Add("@category_id", param.TypeSkills);
                    parametro.Add("@description_id", param.SkillId);
                    parametro.Add("@nivel_id", param.SkillNivelId);

                    string CriaSkill = "INSERT INTO `joborder_skills` (`joborder_id`, `category_id`, `description_id`, `nivel_id`) " +
                                            "VALUES (@joborder_id, @category_id, @description_id, @nivel_id);";

                    var skills = db.Execute(CriaSkill, param: parametro, commandType: System.Data.CommandType.Text);

                    return param;
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao criar a Skill.", ex);
                }
                finally { db.Close(); }
            }
        }

        public async Task<SkillVagaParam> RemoverSkillVagaFourmakersSRS(int skillId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();

                    parametro.Add("@id", skillId);

                    string stringBuscaSkill = "SELECT * FROM `joborder_skills` WHERE(`id` = @id);";

                    var buscaSkill = db.QueryFirst(stringBuscaSkill, param: parametro, commandType: System.Data.CommandType.Text);

                    string RemoveSkill = "DELETE FROM `joborder_skills` WHERE(`id` = @id);";

                    db.Execute(RemoveSkill, param: parametro, commandType: System.Data.CommandType.Text);

                    return new SkillVagaParam
                    {
                        SkillId = buscaSkill.description_id,
                        Id = buscaSkill.id,
                        Joborder_id = buscaSkill.joborder_id,
                        SkillNivelId = buscaSkill.nivel_id,
                        TypeSkills = buscaSkill.category_id,
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao remover a Skill.", ex);
                }
                finally { db.Close(); }
            }
        }

        public async Task<List<SkillVagaParam>> BuscaSkillsVagaFourmakersSRS(int vagaId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    var ret = new List<SkillVagaParam>();

                    parametro.Add("@joborder_id", vagaId);

                    string buscaSkillsVaga = "SELECT * FROM `joborder_skills` WHERE(`joborder_id` = @joborder_id);";

                    var skills = db.Query(buscaSkillsVaga, param: parametro, commandType: System.Data.CommandType.Text);

                    foreach (var item in skills)
                    {
                        ret.Add(new SkillVagaParam
                        {
                            Id = item.id,
                            Joborder_id = item.joborder_id,
                            SkillId = item.description_id,
                            SkillNivelId = item.nivel_id,
                            TypeSkills = item.category_id
                        });
                    }

                    return ret;
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro na consulta, verifique o Id da vaga.", ex);
                }
                finally { db.Close(); }
            }
        }

        public async Task<AlterarStatusCandidaturaDTO> AlterarStatusCandidatura(AlterarStatusCandidaturaParam status, string nomeAnalista)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var parametro = new DynamicParameters();

                        parametro.Add("@joborder_id", status.Joborder_id);
                        parametro.Add("@candidate_id", status.Candidate_id);
                        parametro.Add("@status_id", status.Status_id);
                        parametro.Add("@nomeAnalista", nomeAnalista);
                        parametro.Add("@date", DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));

                        string alterarStatusSQL = "UPDATE `candidate_joborder` SET `status` = @status_id " +
                                                  "WHERE(`joborder_id` = @joborder_id and `candidate_id` = @candidate_id);";

                        db.Execute(alterarStatusSQL, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        string alterarStatusHistorySQL = "UPDATE `cats`.`candidate_joborder_status_history` " +
                                                         "SET `status_from` = '1', `status_to` = @status_id, `site_id` = '1', `nomeAnalista` = @nomeAnalista , `date` = @date" +
                                                         "WHERE(`joborder_id` = @joborder_id and `candidate_id` = @candidate_id);";

                        db.Execute(alterarStatusHistorySQL, param: parametro, transaction, commandType: System.Data.CommandType.Text);

                        transaction.Commit();
                        return new AlterarStatusCandidaturaDTO
                        {
                            Candidate_id = status.Candidate_id,
                            Joborder_id = status.Joborder_id,
                            nomeAnalista = nomeAnalista,
                            Status_id = status.Status_id
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception("Ocorreu um erro ao Editar o Status do candidato.", ex);
                    }
                    finally { db.Close(); }
                }
            }
        }

        public string BuscaNomeAnalista(string email)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@email", email);
                    var queryUser = "SELECT first_name, last_name from user where email = @email;";

                    var query = db.QueryFirstOrDefault(queryUser, param: parametro, commandType: System.Data.CommandType.Text);

                    string userName = query.first_name + " " + query.last_name;

                    return userName;
                }
                catch
                {
                    throw new Exception("Usuário na cadastrado no SRS ou inválido.");
                }
                finally { db.Close(); }
            }
        }

        public void InsertHistoricoCandidato(HistoricoDTO param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                try
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@data_item_type", param.data_item_type);
                    parametro.Add("@data_item_id", param.data_item_id);
                    parametro.Add("@the_field", param.the_field);
                    parametro.Add("@previous_value", param.previous_value);
                    parametro.Add("@new_value", param.new_value);
                    parametro.Add("@description", param.description);
                    parametro.Add("@set_date", param.set_date);
                    parametro.Add("@entered_by", param.entered_by);
                    parametro.Add("@site_id", param.site_id);
                    parametro.Add("@flag", param.flag);

                    var stringHistorico = "INSERT INTO `cats`.`history` (`data_item_type`, `data_item_id`, `the_field`, `previous_value`, `new_value`, " +
                                        "`description`, `set_date`, `entered_by`, `site_id`, `Flag`) " +
                                        "VALUES (@data_item_type, @data_item_id, @the_field, @previous_value, @new_value, " +
                                        "@description, @set_date, @entered_by, @site_id, @flag);";

                    var queryCriaHIstorico = db.Execute(stringHistorico, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public async Task<int> InsertCandidateEntrevistaAsync(EntrevistaParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();

                try
                {
                    var parametro = new DynamicParameters();

                    parametro.Add("@candidate_id", param.Candidate_id);
                    parametro.Add("@joborder_id", param.Joborder_id);
                    parametro.Add("@analista", param.Analista);
                    parametro.Add("@area", param.Area);
                    parametro.Add("@prcanalista", param.PrcAnalista);
                    parametro.Add("@dataEntrevista", param.DataEntrevista);
                    parametro.Add("@horaInicioEntrevista", param.HoraInicioEntrevista);
                    parametro.Add("@horaFinalEntrevista", param.HoraFinalEntrevista);
                    parametro.Add("@notesDPA", param.NotesDPA);
                    parametro.Add("@tipo_ia", param.tipo_ia);

                    string queryInsertCandidate = "INSERT INTO `candidate_entrevista` (`candidate_id`, `joborder_id`, `analista`, `area`, `prcanalista`, `dataEntrevista`," +
                                                  " `horaInicioEntrevista`, `horaFinalEntrevista`, `notesDPA`, `tipo_ia`) VALUES(@candidate_id, @joborder_id, @analista," +
                                                  "@area, @prcanalista, @dataEntrevista, @horaInicioEntrevista, @horaFinalEntrevista, @notesDPA, @tipo_ia)";

                    await db.ExecuteAsync(queryInsertCandidate, param: parametro, commandType: System.Data.CommandType.Text);

                    var entrevistaId = await db.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

                    return entrevistaId;
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }

            }
        }

        public async Task<T> GenericPowerAutomateRequest<T>(HttpMethod method, string uri, FormUrlEncodedContent content, string token = null)
        {
            using var client = new HttpClient();

            var requestResult = new HttpRequestMessage(method, uri)
            {
                Content = content
            };

            requestResult.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
                requestResult.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.Send(requestResult);

            if (response.IsSuccessStatusCode)
            {
                var contentAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(contentAsString);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GenericPowerAutomateRequest: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<T> GenericPowerAutomateRequestWithBody<T>(HttpMethod method, string uri, object bodyRequest, string token = null)
        {
            using var client = new HttpClient();

            var jsonBody = JsonConvert.SerializeObject(bodyRequest);
            var contentBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var requestResult = new HttpRequestMessage(method, uri)
            {
                Content = contentBody
            };

            requestResult.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestResult.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.Send(requestResult);

            if (response.IsSuccessStatusCode)
            {
                var contentAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(contentAsString);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GenericPowerAutomateRequestWithBody: {response.StatusCode}, {errorContent}");
            }
        }

        public int SendToLogSqsQueueLog(SRSInsertCandidateParamSQS param)
        {
            Console.WriteLine("Iniciando inserção no sqsQueueLog...");
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    Console.WriteLine("Conexão com o banco de dados aberta.");
                    var parametro = new DynamicParameters();
                    parametro.Add("@candidate_id", param.CandidateId);
                    parametro.Add("@nomeCompleto", param.FirstName);
                    parametro.Add("@dataNascimento", param.DataNascimento);
                    parametro.Add("@emailContato", param.Email1);
                    parametro.Add("@emailCorporativo", param.EmailFoursys);
                    parametro.Add("@flagColaborador", param.IsActive);
                    parametro.Add("@cpf", param.CandCpf);
                    parametro.Add("@cargo", param.TipoCargo);
                    parametro.Add("@hardskill", JsonConvert.SerializeObject(param.Hardskill)); 
                    parametro.Add("@softskill", JsonConvert.SerializeObject(param.Softskill)); 
                    parametro.Add("@metodologia", JsonConvert.SerializeObject(param.Methodologia)); 
                    parametro.Add("@dominio", JsonConvert.SerializeObject(param.Dominio)); 
                    parametro.Add("@idioma", JsonConvert.SerializeObject(param.Idioma)); 
                    parametro.Add("@pendingToSend", param.PendingToSend);
                    parametro.Add("@observacao", param.Observacao);
                    parametro.Add("@operation", param.Operation);
                    parametro.Add("@dataOrigem", param.DataOrigem);
                    parametro.Add("@cidade", param.City);
                    parametro.Add("@estado", param.State);
                   
                    string insert = @"
                                     INSERT INTO `cats`.`sqsQueueLog` (
                                         `candidate_id`, `nomeCompleto`, `dataNascimento`, `emailContato`, `emailCorporativo`, 
                                         `flagColaborador`, `cpf`, `cargo`, `hardskill`, `softskill`, `metodologia`, `dominio`, `idioma`, 
                                         `pendingToSend`, `observacao`, `operation`, `dataOrigem`, `cidade`, `estado`
                                     ) VALUES (
                                         @candidate_id, @nomeCompleto, @dataNascimento, @emailContato, @emailCorporativo, 
                                         @flagColaborador, @cpf, @cargo, @hardskill, @softskill, @metodologia, @dominio, @idioma, 
                                         @pendingToSend, @observacao, @operation, @dataOrigem, @cidade, @estado
                                     );
                                     SELECT LAST_INSERT_ID() AS idLog;";

                    var idLog = db.ExecuteScalar<int>(insert, param: parametro, commandType: System.Data.CommandType.Text);

                    return idLog;
                }
                catch (Exception ex)
                {                    
                    throw;
                }
                finally
                {
                    db.Close();                    
                }
            }
        }             
               
        public void UpdateLogStatus(int idLog, string status)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var query = @"
                                    UPDATE `cats`.`sqsQueueLog`
                                    SET
                                        `pendingToSend` = @status
                                    WHERE
                                        idLog = @idLog";

                    db.Execute(query, new { idLog, status });
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public int VerificaLogExistenteCandidateID(int candidateId)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    var query = @"
                                    SELECT 
                                        idLog
                                    FROM
                                        `cats`.`sqsQueueLog` 
                                    WHERE 
                                        candidate_id = @candidateId
                                    ORDER BY
                                        dataCreated DESC
                                    LIMIT 1";

                    var idLog = db.ExecuteScalar<int>(query, new { candidateId });

                    return idLog;

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }
        
        public void InsertCandidateLinkedin(SRSInsertCandidateParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var parametro = new DynamicParameters();
                    parametro.Add("@siteId", param.site_id);
                    parametro.Add("@last_name", param.last_name);
                    parametro.Add("@first_name", param.first_name);
                    parametro.Add("@address", param.address);
                    parametro.Add("@can_relocate", param.can_relocate);
                    parametro.Add("@entered_by", param.entered_by);
                    parametro.Add("@owner", param.owner);
                    parametro.Add("@date_created", param.date_created);
                    parametro.Add("@date_modified", param.date_modified);
                    parametro.Add("@import_id", param.import_id);
                    parametro.Add("@is_hot", param.is_hot);
                    parametro.Add("@best_time_to_call", param.best_time_to_call);
                    parametro.Add("@cand_rg", param.cand_rg);
                    parametro.Add("@cand_cpf", param.cand_cpf);
                    parametro.Add("@cand_estCivil", param.cand_estCivil);
                    parametro.Add("@cand_Lkdin", param.cand_Lkdin);
                    parametro.Add("@cand_skype", param.cand_skype);
                    parametro.Add("@cand_filhos", param.cand_filhos);
                    parametro.Add("@cand_fumante", param.cand_fumante);
                    parametro.Add("@source", param.source);
                    parametro.Add("@email1", param.email1);
                    parametro.Add("@is_active", param.is_active);

                    string insert = "INSERT INTO `cats`.`candidate` " +
                                    "(`site_id`, `last_name`, `first_name`, `address`, `can_relocate`, `entered_by`, `owner`, " +
                                    "`date_created`, `date_modified`, `import_id`, `is_hot`, `best_time_to_call`, " +
                                    "`cand_rg`, `cand_cpf`, `cand_estCivil`, `cand_Lkdin`, `cand_skype`, `cand_filhos`, " +
                                    "`cand_fumante`, `source`, `email1`, `is_active`) " +
                                    "VALUES " +
                                    "(@siteId, @last_name, @first_name, @address, @can_relocate, @entered_by, @owner, " +
                                    "@date_created, @date_modified, @import_id, @is_hot, @best_time_to_call, " +
                                    "@cand_rg, @cand_cpf, @cand_estCivil, @cand_Lkdin, @cand_skype, @cand_filhos, " +
                                    "@cand_fumante, @source, @email1, @is_active);";

                    db.Execute(insert, param: parametro, commandType: System.Data.CommandType.Text);
                }
                catch
                {
                    throw;
                }
                finally { db.Close(); }
            }
        }

        public int GetCandidateIdByVanityName(string vanityName)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = "SELECT `candidate_id` FROM `cats`.`candidate` WHERE cand_Lkdin like '%linkedin.com/in/" + vanityName + "' or cand_Lkdin like '%linkedin.com/in/" + vanityName + "/'";
                    var candidate = db.QueryFirstOrDefault<dynamic>(query, commandType: System.Data.CommandType.Text);
                    var candidateId = candidate?.candidate_id ?? 0;

                    return candidateId;
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

        public List<CandidateRelatorioBI> GetRelatorioCandidatos()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                try
                {
                    db.Open();
                    var query = @"SELECT
                                    c.candidate_id AS id,
                                    c.first_name AS nomeCandidato,
                                    et.dataEntrevista AS dataEntrevistaTecnica,
                                    et.prcanalista AS parecerTecnico,
                                    et.notesDPA AS notasEntrevistaTecnica,
                                    er.dataEntrevista AS dataEntrevistaRh,
                                    er.prcanalista AS parecerRh,
                                    er.notesDPA AS notasEntrevistaRh,
                                    eg.dataEntrevista AS dataEntrevistaGestor,
                                    eg.prcanalista AS parecerGestor,
                                    eg.notesDPA AS notasEntrevistaGestor,
                                    CONCAT(u.first_name, ' ', u.last_name) AS nomeRecrutador,
                                    c.desired_pay AS desiredPay,
                                    c.cand_modalidade AS candModalidade,
                                    c.key_skills AS skillLegado,
                                    GROUP_CONCAT(DISTINCT CONCAT('(', cs.category_id, ',', cs.description_id, ',', cs.nivel_id, ')') SEPARATOR ';') AS skills
                                FROM
                                    candidate c
                                    LEFT JOIN `user` u ON u.user_id = c.owner
                                    LEFT JOIN candidate_skills cs ON cs.candidate_id = c.candidate_id
                                    LEFT JOIN candidate_entrevista et ON et.candidate_id = c.candidate_id AND et.area = 2 -- Entrevista Técnica
                                    LEFT JOIN candidate_entrevista er ON er.candidate_id = c.candidate_id AND er.area = 1 -- Entrevista RH
                                    left join candidate_cliente_gestor_entrevista eg on eg.candidate_id = c.candidate_id  -- Entrevista Cliente gestor
                                WHERE
                                    c.date_created >= '2025-01-01'
                                GROUP BY
                                    c.candidate_id -- Apenas o candidate_id é necessário no GROUP BY
                                ORDER BY
                                    c.candidate_id DESC,
                                    c.first_name; -- Ordenação secundária para consistência";

                    return db.Query<CandidateRelatorioBI>(query).ToList();
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

        public async Task<bool> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var queryJobOrderSkillSelect = @"SELECT
                                                            js.id AS Id,
                                                            js.joborder_id AS JobOrderId,
                                                            js.category_id AS CategoryId,
                                                            js.description_id AS DescriptionId,
                                                            js.nivel_id AS NivelId,
                                                            js.date_created AS DateCreated,
                                                            js.date_modified AS DateModified
                                                        FROM
                                                            joborder_skills js
                                                        WHERE
                                                            js.category_id = @PerfilItemIdAntigo
                                                            AND js.description_id = @IdSkillAntigo;";

                        var parametrosSelect = new
                        {
                            IdSkillAntigo = param.IdHabilidadeAntiga,
                            PerfilItemIdAntigo = param.TipoAntigo,
                        };

                        var resultadoJobOrderSkillSelectQuery = await db.QueryAsync<JobOrderSkillDTO>(queryJobOrderSkillSelect, parametrosSelect, transaction);

                        foreach (var item in resultadoJobOrderSkillSelectQuery)
                        {
                            var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteSRS((CategoriaSkillEnum)param.TipoNovo, item.NivelId);

                            var queryUpdate = @"UPDATE
                                                    joborder_skills
                                                SET
                                                    category_id = @PerfilItemIdNovo,
                                                    description_id = @IdSkillNovo,
                                                    nivel_id = @NivelIdEquivalente
                                                WHERE
                                                    category_id = @PerfilItemIdAntigo
                                                    AND description_id = @IdSkillAntigo
                                                    AND nivel_id = @NivelId;";

                            var parametrosUpdate = new
                            {
                                IdSkillAntigo = item.DescriptionId,
                                PerfilItemIdAntigo = item.CategoryId,
                                NivelId = item.NivelId,
                                PerfilItemIdNovo = param.TipoNovo,
                                IdSkillNovo = param.IdHabilidadeNova,
                                NivelIdEquivalente = nivelEquivalente
                            };

                            await db.ExecuteAsync(queryUpdate, parametrosUpdate, transaction, commandType: System.Data.CommandType.Text);
                        }

                        var queryCandidateSkillSelect = @"SELECT
                                                cs.id AS Id,
                                                cs.candidate_id AS Candidate_id,
                                                cs.entrevista_id AS Entrevista_id,
                                                cs.category_id AS Category_id,
                                                cs.description_id AS Description_id,
                                                cs.nivel_id AS Nivel_id,
                                                cs.date_created AS Date_Created,
                                                cs.date_modified AS Date_Modified
                                            FROM
                                                candidate_skills cs
                                            WHERE
                                                cs.category_id = @PerfilItemIdAntigo
                                                AND cs.description_id = @IdSkillAntigo;";

                        var parametrosCandidateSelect = new
                        {
                            IdSkillAntigo = param.IdHabilidadeAntiga,
                            PerfilItemIdAntigo = param.TipoAntigo,
                        };

                        var resultadoCandidateSkillSelectQuery = await db.QueryAsync<CandidateSkillDTO>(queryCandidateSkillSelect, parametrosCandidateSelect, transaction);

                        foreach (var item in resultadoCandidateSkillSelectQuery)
                        {
                            var nivelEquivalente = CategoriaHabilidadeUtil.GetNivelEquivalenteSRS((CategoriaSkillEnum)param.TipoNovo, item.Nivel_id);

                            var queryUpdate = @"UPDATE
                                                    candidate_skills
                                                SET
                                                    category_id = @PerfilItemIdNovo,
                                                    description_id = @IdSkillNovo,
                                                    nivel_id = @NivelIdEquivalente
                                                WHERE
                                                    category_id = @PerfilItemIdAntigo
                                                    AND description_id = @IdSkillAntigo
                                                    AND nivel_id = @NivelId;";

                            var parametrosUpdate = new
                            {
                                IdSkillAntigo = item.Description_id,
                                PerfilItemIdAntigo = item.Category_id,
                                NivelId = item.Nivel_id,
                                PerfilItemIdNovo = param.TipoNovo,
                                IdSkillNovo = param.IdHabilidadeNova,
                                NivelIdEquivalente = nivelEquivalente
                            };

                            await db.ExecuteAsync(queryUpdate, parametrosUpdate, transaction, commandType: System.Data.CommandType.Text);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        db.Close();
                    }
                }
            }
        }

        public async Task<Usuario> ValidarUsuarioRepository(int? userId, string userName)
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var query = @"SELECT
                                        user_id,
                                        user_kenoby_id,
                                        site_id,
                                        user_name,
                                        email,
                                        password,
                                        access_level,
                                        approve_template_admission,
                                        can_change_password,
                                        is_test_user,
                                        last_name,
                                        first_name,
                                        is_demo,
                                        categories,
                                        session_cookie,
                                        pipeline_entries_per_page,
                                        column_preferences,
                                        force_logout,
                                        title,
                                        phone_work,
                                        phone_cell,
                                        phone_other,
                                        address,
                                        notes,
                                        company,
                                        city,
                                        state,
                                        zip_code,
                                        country,
                                        can_see_eeo_info,
                                        usu_categoria,
                                        perfilC,
                                        typec,
                                        perfilRS,
                                        perfilAV,
                                        visualiza_vagas_unidades,
                                        uniresp,
                                        flg_trabalhar_vaga,
                                        flg_sugestoes_alteracao,
                                        template_admission_onboarding
                                       FROM user
                                     WHERE
                                         user_id = @userId
                                         OR user_name = @userName
                                    ";

                        var param = new
                        {
                            userId,
                            userName,
                        };

                        var user = await db.QueryAsync<Usuario>(query, param, transaction);
                        if (user.Count() > 0)
                            return user.FirstOrDefault();
                        else
                            return null;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        db.Close();
                    }
                }
            }
        }

        public async Task<List<JobOrderDTO>> BuscarVagasDoUltimoAno()
        {
            using (var db = CreateMySqlConnectionFromEnv())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {

                        var oneYearAgo = DateTime.UtcNow.AddYears(-1);

                        // Query para buscar os joborders e skills
                        string sqlJobOrders = @$"
                        SELECT
                            j.joborder_id as JobOrderId,
                            j.title as Title,
                            j.openings as Openings,
                            j.senioridade as Senioridade,
                            j.jo_dtabVaga as JoDtabVaga,
                            j.jo_stVaga as JoStVaga,
                            j.description as Description,
                            j.cargo as Cargo,
                            j.date_created as DateCreated,
                            j.date_modified as DateModified,
                            j.jo_locTrab as joLocTrab,
                            j.state as state,
                            j.textForLinkedin as textForLinkedin,
                            j.tipoVaga as tipoVaga,
                            j.confidential_job as confidential_job,
                            j.frequencia as Frequencia,
                            j.termometro as termometro,
                            j.email_uniresp as EmailNuResp,
                            ca.date_modified as DataAprovacao
                        FROM
                            joborder j
                        LEFT JOIN
                            controle_aprovacao ca on ca.joborder_id = j.joborder_id
                        WHERE
                            j.date_modified >= @OneYearAgo
                        ORDER BY
                            j.jo_dtabVaga DESC;

                        SELECT
                            s.description_id,
                            s.category_id,
                            s.joborder_id,
                            s.nivel_id
                        FROM
                            joborder_skills s
                        WHERE
                            s.joborder_id IN (
                                SELECT joborder_id
                                FROM joborder
                                WHERE date_modified >= @OneYearAgo
                            );
                    ";

                        using var multi = await db.QueryMultipleAsync(sqlJobOrders, new { OneYearAgo = oneYearAgo });

                        var jobOrders = (await multi.ReadAsync<JobOrderDTO>()).ToList();
                        var skills = (await multi.ReadAsync<SkillsVagasSRSDTO>()).ToList();

                        // Relacionar skills com seus respectivos joborders
                        foreach (var jobOrder in jobOrders)
                            jobOrder.Skills = skills.Where(s => s.JobOrderId == jobOrder.JobOrderId).ToList();

                        return jobOrders;
                    }
                    catch
                    {
                        transaction.Rollback();
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
}