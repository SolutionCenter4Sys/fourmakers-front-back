using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class SRSRepository : ISRSRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;
        private readonly int ORG_FOURSYS = 2;
        private readonly int ORG_FOURMAKERS = 1;
        private readonly int ORG_FMU = 7;
        private const string SOFTSKILL = "SOFTSKILL";
        private const string HARDSKILL = "HARDSKILL";
        private const double MEIO_PESO = 2.0;
        private const int TODOS_NIVEIS = 0;
        public SRSRepository(ColaboradorContext colaboradorContext, IDBConnection dappeCconnection)
        {
            this._colaboradorContext = colaboradorContext;
            _dapperConnection = dappeCconnection;
        }

        private DateTime GetDate(DateTime? date)
        {
            return date ?? DateTime.MinValue;
        }

        private string FormatStatusVagaSrs(string statusSrs)
        {
            string statusFourmaker = null;
            switch (statusSrs)
            {
                case "STAND BY":
                    statusFourmaker = "CANCELADA";
                    break;

                case "FECHADA OC":
                    statusFourmaker = "CANCELADA";
                    break;

                case "FECHADA FOURSYS":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "FECHADA RI":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "EM APROVAÇÃO":
                    statusFourmaker = "EM AVALIAÇÃO";
                    break;

                case "INDICADO CONTRATADO":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "DECURSO DE PRAZO":
                    statusFourmaker = "CANCELADA";
                    break;

                case "AGUARDANDO AJUSTES":
                    statusFourmaker = "EM AVALIAÇÃO";
                    break;

                default:
                    break;
            }

            return statusFourmaker != null ? statusFourmaker : statusSrs;
        }

        public List<SRSDTO> BuscarVagas(string busca, int cursor, int limite, FiltroStatusPublicacaoEnum? filtroStatusPublicacao, int orgId)
        {
            try
            {
                var ret = new List<SRSDTO>();

                var query = _colaboradorContext.tb_vagas_srs
                    .Where(x => x.status_vaga == "EM ANDAMENTO");
                if (orgId == ORG_FOURMAKERS || orgId == ORG_FMU)
                {
                    query = query.Where(x => x.termometro == "BANCO DE TALENTOS" || x.confidential_job == 1);
                }
                else
                {
                    query = query.Where(x => x.confidential_job == 1);
                }

                if (!string.IsNullOrEmpty(busca))
                {
                    query = query.Where(x => EF.Functions.Like(x.titulo, "%" + busca + "%") || EF.Functions.Like(x.id_vaga.ToString(), "%" + busca + "%"));
                }

                var result = query
                    .OrderByDescending(x => x.data_abertura)
                    .Skip(cursor)
                    .Take(limite)
                    .ToList();

                foreach (var row in result)
                {
                    var sRS = new SRSDTO
                    {
                        Id_vaga = row.id_vaga,
                        Titulo = row.titulo,
                        Vagas_abertas = row.vagas_abertas ?? 0,
                        Nivel = row.nivel,
                        Data_abertura = GetDate(row.data_abertura),
                        Status_vaga = row.status_vaga,
                        TextoLinkedin = row.textForLinkedin,
                        Cargo = row.cargo,
                        Data_criacao = GetDate(row.data_criacao),
                        Data_alteracao = GetDate(row.data_alteracao),
                        Modalidade = row.loc_trabalho != "HOME OFFICE" && row.loc_trabalho != "HÍBRIDO" && row.loc_trabalho != "HIBRIDO" ? "PRESENCIAL" : row.loc_trabalho,
                        Estado = row.state,
                        TipoVaga = (TipoVagaEnum?)row.tipoVaga,
                        Ativo = row.ativo,
                        Descricao = row.descricao,
                        Frequencia = row.frequencia,
                        OrgId = null
                    };

                    ret.Add(sRS);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DetalheDTO> BuscarVagaDetalhada(long id_vaga)
        {
            try
            {
                var ret = new List<DetalheDTO>();

                if (id_vaga != 0)
                {
                    var result = _colaboradorContext.tb_vagas_srs.Where(x => x.id_vaga == id_vaga).ToList();

                    foreach (var row in result)
                    {
                        string localidade = "Presencial"; // Valor padrão

                        // Verifique se a localidade é diferente de "Home office" e "Híbrido"
                        if (row.loc_trabalho != "HOME OFFICE" && row.loc_trabalho != "HIBRIDO" && row.loc_trabalho != "HÍBRIDO")
                        {
                            localidade = "PRESENCIAL";
                        }
                        else
                        {
                            localidade = row.loc_trabalho; // Mantenha o valor original
                        }

                        var detalheDTO = new DetalheDTO
                        {
                            Id_Vaga = row.id_vaga,
                            Titulo = row.titulo,
                            TextoLinkedin = row.textForLinkedin,
                            Descricao = row.descricao,
                            Modalidade = localidade,
                            Estado = row.state,
                            TipoVaga = (TipoVagaEnum?)row.tipoVaga,
                            Data_abertura = row.data_abertura,
                            Frequencia = row.frequencia
                        };
                        ret.Add(detalheDTO);
                    }
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SkillsVagasDTO> BuscarSkills(List<long> id)
        {
            try
            {
                var ret = new List<SkillsVagasDTO>();

                var result = _colaboradorContext.tb_skill_vaga
                    .Where(x => id.Contains(x.tb_vagas_srs_id) && x.ativo == 1)
                    .ToList();

                var competencias = _colaboradorContext.tb_competencia.Where(x => x.ativo == 1).ToDictionary(x => x.id, x => x.descricao);
                var softskills = _colaboradorContext.tb_softskill.Where(x => x.ativo == 1).ToDictionary(x => x.id, x => x.descricao);
                var idiomas = _colaboradorContext.tb_idioma.Where(x => x.ativo == 1).ToDictionary(x => x.id, x => x.descricao);
                var metodologias = _colaboradorContext.tb_metodologia.Where(x => x.ativo == 1).ToDictionary(x => x.id, x => x.descricao);
                var dominios = _colaboradorContext.tb_dominionegocio.Where(x => x.ativo == 1).ToDictionary(x => x.id, x => x.descricao);

                var niveis = _colaboradorContext.tb_nivel.ToDictionary(x => x.id, x => x.descricao);

                foreach (var row in result)
                {
                    var description = "";
                    var descriptionType = "";
                    var descriptionNivel = "";
                    var descriptionNivelType = "";

                    switch (row.tipo_skill_id)
                    {
                        case 1:
                            description = competencias.GetValueOrDefault(row.skill_id);
                            descriptionType = "HardSkill";
                            break;

                        case 2:
                            description = softskills.GetValueOrDefault(row.skill_id);
                            descriptionType = "Softskill";
                            break;

                        case 3:
                            description = idiomas.GetValueOrDefault(row.skill_id);
                            descriptionType = "Idioma";
                            break;

                        case 4:
                            description = metodologias.GetValueOrDefault(row.skill_id);
                            descriptionType = "Methodology";
                            break;

                        default:
                            description = dominios.GetValueOrDefault(row.skill_id);
                            break;
                    }

                    if (description == null)
                    {
                        continue;
                    }
                    
                    descriptionNivel = niveis.GetValueOrDefault(row.skill_nivel_id);

                    if (row.skill_nivel_id != null)
                    {
                        switch (row.skill_nivel_id)
                        {
                            case 1:
                                descriptionNivelType = "Trainee";
                                break;

                            case 2:
                                descriptionNivelType = "Júnior";
                                break;

                            case 3:
                                descriptionNivelType = "Pleno";
                                break;

                            case 4:
                                descriptionNivelType = "Sênior";
                                break;

                            case 5:
                                descriptionNivelType = "Ensino Médio";
                                break;

                            case 6:
                                descriptionNivelType = "Graduação";
                                break;

                            case 7:
                                descriptionNivelType = "Pós Graduação";
                                break;

                            case 8:
                                descriptionNivelType = "MBA";
                                break;

                            case 9:
                                descriptionNivelType = "Doutorado";
                                break;

                            case 10:
                            case 17:
                                descriptionNivelType = "Certificado";
                                break;

                            case 11:
                            case 18:
                                descriptionNivelType = "Alto";
                                break;

                            case 12:
                            case 19:
                                descriptionNivelType = "Médio";
                                break;

                            case 13:
                            case 20:
                                descriptionNivelType = "Baixo";
                                break;

                            case 14:
                            case 21:
                                descriptionNivelType = "Desenvolvido";
                                break;

                            case 15:
                            case 22:
                                descriptionNivelType = "Em desenvolvimento";
                                break;

                            case 16:
                            case 23:
                                descriptionNivelType = "A desenvolver";
                                break;

                            case 24:
                                descriptionNivelType = "Básico";
                                break;

                            case 25:
                                descriptionNivelType = "Intermediário";
                                break;

                            case 26:
                                descriptionNivelType = "Avançado";
                                break;

                            case 27:
                                descriptionNivelType = "Fluente";
                                break;

                            default:
                                descriptionNivelType = "";
                                break;
                        }
                    }

                    var sRSSkillsVagas = new SkillsVagasDTO
                    {
                        SkillId = row.skill_id,
                        SkillDescription = description,
                        TypeSkills = row.tipo_skill_id,
                        TypeSkillsDescription = descriptionType,
                        SkillNivelId = row.skill_nivel_id,
                        SkillNivelDescription = descriptionNivelType,
                        Id = row.tb_vagas_srs_id
                    };

                    ret.Add(sRSSkillsVagas);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FavoritarVagasDTO> FavoritarVagas(long id_vaga, long tb_usuario_id)
        {
            try
            {
                var ret = new List<FavoritarVagasDTO>();
                var vaga = _colaboradorContext.tb_vagas_srs.FirstOrDefault(x => x.id_vaga == id_vaga);
                if (vaga != null)
                {
                    var result = _colaboradorContext.tb_vaga_favorito.Where(x => x.tb_vagas_srs_id == id_vaga && x.tb_usuario_id == tb_usuario_id).ToList();
                    if (result.Count == 0) // Verifica se o usuário ainda não favoritou essa vaga para evitar duplicatas
                    {
                        // Adiciona o registro de favorito na tabela
                        var novoFavorito = new tb_vaga_favorito
                        {
                            tb_vagas_srs_id = id_vaga,
                            tb_usuario_id = tb_usuario_id,
                            ativo = 1,
                            data_criacao = DateTime.Now,
                            data_alteracao = DateTime.Now
                        };
                        _colaboradorContext.tb_vaga_favorito.Add(novoFavorito);
                        _colaboradorContext.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("A vaga já está favoritada para esse usuário");
                    }
                }
                else
                {
                    throw new Exception("ID da vaga inválido");
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DesfavoritarResult DesfavoritarVagas(long id_vaga, long tb_usuario_id)
        {
            try
            {
                var ret = new DesfavoritarResult();
                if (id_vaga > 0)
                {
                    var result = _colaboradorContext.tb_vaga_favorito
                        .FirstOrDefault(x => x.tb_vagas_srs_id == id_vaga && x.tb_usuario_id == tb_usuario_id);

                    if (result != null) // Verifica se a vaga já foi favoritada pelo usuário
                    {
                        _colaboradorContext.tb_vaga_favorito.Remove(result);
                        _colaboradorContext.SaveChanges();

                        ret.Sucess = true;
                        ret.Message = "Vaga desfavoritada com sucesso";
                    }
                    else
                    {
                        throw new Exception("A vaga não está favoritada pelo usuário");
                    }
                }
                else
                {
                    throw new Exception("ID da vaga inválido");
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FavoritarVagasDTO> ListarVagasFavoritadasPorUsuario(long tb_usuario_id)
        {
            try
            {
                // Consulta as vagas favoritadas para o usuário específico usando o tb_usuario_id
                var result = _colaboradorContext.tb_vaga_favorito

                    .Where(x => x.tb_usuario_id == tb_usuario_id)
                    .Select(x => new FavoritarVagasDTO
                    {
                        Id_vaga = x.tb_vagas_srs_id,
                        Tb_usuario_id = x.tb_usuario_id,
                        Ativo = x.ativo,
                    })
                    .ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RecomendarVagasDTO> ListarVagasRecomendadas(List<SkillsDTO> skills)
        {
            try
            {
                List<RecomendarVagasDTO> ret = new List<RecomendarVagasDTO>();

                foreach (var skill in skills)
                {
                    var skillVaga = _colaboradorContext.tb_skill_vaga
                        .Where(x => x.skill_id == skill.Skill.Id && x.ativo == 1)
                        .ToList();

                    foreach (var vaga in skillVaga)
                    {
                        ret.Add(new RecomendarVagasDTO
                        {
                            SkillId = vaga.skill_id,
                            SkillNivelId = vaga.skill_nivel_id,
                            TipoSkillId = vaga.tipo_skill_id,
                            VagasSrsId = vaga.tb_vagas_srs_id,
                            Cargo = vaga.tb_vagas_srs.cargo,
                            Modalidade = vaga.tb_vagas_srs.loc_trabalho,
                            Estado = vaga.tb_vagas_srs.state,
                            Nivel = vaga.tb_vagas_srs.nivel,
                            SkillVagaId = vaga.skill_id,
                            StatusVaga = FormatStatusVagaSrs(vaga.tb_vagas_srs.status_vaga),
                            DataAbertura = vaga.tb_vagas_srs.data_abertura ?? DateTime.MinValue,
                            Vaga = new DetalheDTO
                            {
                                TextoLinkedin = vaga.tb_vagas_srs.textForLinkedin,
                                Id_Vaga = vaga.tb_vagas_srs.id_vaga,
                                Titulo = vaga.tb_vagas_srs.titulo,
                                Modalidade = vaga.tb_vagas_srs.loc_trabalho,
                                Estado = vaga.tb_vagas_srs.state,
                                TipoVaga = (TipoVagaEnum?)vaga.tb_vagas_srs.tipoVaga,
                                SkillsVagasDTO = new List<SkillsVagasDTO>()
                            }
                        });
                    }
                }

                return ret.Distinct().ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<SkillsDTO> GetSkillsUsuario(string cpfSolicitante)
        {
            try
            {
                List<SkillsDTO> ret = new List<SkillsDTO>();

                var hardskills = _colaboradorContext.tb_colaborador_competencia.Where(x => x.codigo_interno_colaborador == cpfSolicitante && x.ativo == 1).ToList();

                var softskills = _colaboradorContext.tb_colaborador_softskill.Where(x => x.codigo_interno_colaborador == cpfSolicitante && x.ativo == 1).ToList();

                foreach (var hardskill in hardskills)
                {
                    ItemPerfilDTO item = new ItemPerfilDTO();
                    item.Descricao = hardskill.competencia.descricao;
                    item.Id = hardskill.competencia_id;
                    if (hardskill.competencia.confirmada == 1)
                    {
                        item.Pendente = true;
                    }
                    else
                    {
                        item.Pendente = false;
                    }
                    item.Nome = hardskill.competencia.descricao;
                    if (hardskill.tb_nivel != null)
                    {
                        NivelDTO nivel = new NivelDTO { Descricao = hardskill.tb_nivel.descricao, Id = hardskill.tb_nivel_id };
                        ret.Add(new SkillsDTO { Skill = item, Nivel = nivel });
                    }
                    else
                    {
                        ret.Add(new SkillsDTO { Skill = item, Nivel = null });
                    }
                }

                foreach (var softskill in softskills)
                {
                    ItemPerfilDTO item = new ItemPerfilDTO();
                    item.Descricao = softskill.softskill.descricao;
                    item.Id = softskill.softskill_id;
                    if (softskill.softskill.confirmada == 1)
                    {
                        item.Pendente = true;
                    }
                    else
                    {
                        item.Pendente = false;
                    }
                    item.Nome = softskill.softskill.descricao;

                    if (softskill.tb_nivel != null)
                    {
                        NivelDTO nivel = new NivelDTO { Descricao = softskill.tb_nivel.descricao, Id = softskill.tb_nivel_id };
                        ret.Add(new SkillsDTO { Skill = item, Nivel = nivel });
                    }
                    else
                    {
                        ret.Add(new SkillsDTO { Skill = item, Nivel = null });
                    }
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public long BuscaQuatidadeVagas()
        {
            try
            {
                var vagas = _colaboradorContext.tb_vagas_srs.Where(x => x.ativo == 1).Distinct().OrderBy(x => x.id_vaga);
                List<long> quantidadeVagas = new List<long>();
                long vagaId = 0;

                foreach (var vaga in vagas)
                {
                    if (vagaId == 0 || vagaId == vaga.id_vaga)
                    {
                        if (vagaId == 0)
                        {
                            quantidadeVagas.Add(vaga.id_vaga);
                        }
                        vagaId = vaga.id_vaga;
                    }
                    else
                    {
                        quantidadeVagas.Add(vaga.id_vaga);
                        vagaId = vaga.id_vaga;
                    }
                }

                return quantidadeVagas.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<VagaIndicadaDTO> ListarVagasIndicadas(string cpf, int orgId)
        {
            var ret = new List<VagaIndicadaDTO>();

            var idUsuario = _colaboradorContext.tb_usuario
                .Where(u => u.codigo_interno_colaborador == cpf && u.tb_org_id == orgId)
                .Select(u => u.id)
                .FirstOrDefault();

            if (idUsuario != 0)
            {
                var vagasIndicadas = _colaboradorContext.tb_vagas_indicadas
                    .Where(vi => vi.id_usuario_indicou == idUsuario && vi.ativo == true)
                    .ToList();

                foreach (var item in vagasIndicadas)
                {
                    var vaga = _colaboradorContext.tb_vagas_srs
                        .Where(vs => vs.id_vaga == item.id_vaga)
                        .FirstOrDefault();

                    ret.Add(new VagaIndicadaDTO
                    {
                        vagaId = item.id_vaga,
                        vagaLink = item.link_URL,
                        candidatoNome = item.nome_candidato,
                        vagaNome = vaga?.titulo,
                        vagaStatus = FormatStatusVagaSrs(vaga?.status_vaga),
                        tipoVaga = vaga?.tipoVaga
                    });
                }
            }

            return ret;
        }

        public void InserirVagaIndicada(long idVaga, DateTime dataCriacao, string urlLinkedin, string nomeCandidato, string emailUsuarioIndicou, string link, int orgId)
        {
            try
            {
                var urlBase = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE);
                var userIdRow = _colaboradorContext.tb_usuario.Where(x => x.email == emailUsuarioIndicou && x.tb_org_id == orgId).Select(x => x.id).FirstOrDefault();
                var url = urlBase + "/vaga/candidatar/" + idVaga + "/" + link;
                var row = _colaboradorContext.tb_vagas_indicadas.Where(x => x.url_linkedin_candidato == urlLinkedin && x.ativo == false).FirstOrDefault();
                if (row == null)
                {
                    var vaga = new tb_vagas_indicadas()
                    {
                        id_vaga = idVaga,
                        ativo = true,
                        data_aplicacao = DateTime.UtcNow,
                        data_criacao = dataCriacao,
                        url_linkedin_candidato = urlLinkedin,
                        nome_candidato = nomeCandidato,
                        id_usuario_indicou = userIdRow,
                        link_URL = url
                    };
                    _colaboradorContext.tb_vagas_indicadas.Add(vaga);
                }
                else
                {
                    row.id_usuario_indicou = userIdRow;
                    row.link_URL = url;
                    row.ativo = true;
                }

                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoverVagaIndicada(long joborder_id, string cpf)
        {
            try
            {
                var userIdRow = _colaboradorContext.tb_usuario.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault();
                var row = _colaboradorContext.tb_vagas_indicadas.Where(x => x.url_linkedin_candidato == userIdRow.codigo_interno_colaboradorNavigation.url_linkedin && x.id_vaga == joborder_id && x.ativo == true).FirstOrDefault();

                if (row != null)
                {
                    row.ativo = false;
                    _colaboradorContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long IndicarVaga(long usuarioId, long vagaId, string nome, string email, string telefone, string deOndeConhece, bool autorizou, bool estaDisponivel, string linkedin)
        {
            try
            {
                var parseAutorizou = autorizou ? 1 : 0;
                var parseDisponivel = estaDisponivel ? 1 : 0;

                var indicacao = new tb_indicacao_premiada_parcial
                {
                    nome_indicado = nome,
                    telefone_indicado = telefone,
                    linkedin = linkedin,
                    data_criacao = DateTime.Now,
                    email_indicado = email,
                    id_usuario_indicou = usuarioId,
                    relacao_indicado = deOndeConhece,
                    id_vaga = vagaId,
                    link_indicacao = "",
                    autorizou = (sbyte)parseAutorizou,
                    disponivel = (sbyte)parseDisponivel
                };

                _colaboradorContext.tb_indicacao_premiada_parcial.Add(indicacao);
                _colaboradorContext.SaveChanges();

                return indicacao.id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IndicacaoPremiadaParcialDTO>> ListarIndicacoesPremiadas(ListarIndicacaoPremiadaParcialParam param, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string limitClause = param.Limite.HasValue && param.Limite > 0 ? $"LIMIT @Limite" : "";

            var orderByClause = "ORDER BY DataIndicacao";

            if (!string.IsNullOrEmpty(param.OrderByPropertie))
            {
                var validColumns = new List<string>
                {
                    "DataIndicacao",
                    "NomeDoFourTalent",
                    "EmailFourTalent",
                    "VagaId",
                    "TituloDaVaga",
                    "DeOndeConhece",
                    "DisponivelParaParticipar",
                    "AutorizouOEnvioDoCV",
                    "LinkedinCandidato",
                    "CvValido",
                    "RetornoAoProfissional",
                    "PerfilAvaliado",
                    "Status", //Status Indicacao
                    "StatusDaVaga", // Status da Vaga
                    "Descricao", // Unidade Responsável
                    "Nome",// Analista Responsável
                    "NomeCompletoCandidado"
                };

                if (validColumns.Contains(param.OrderByPropertie))
                {
                    orderByClause = $"ORDER BY {param.OrderByPropertie}";
                }
                else
                {
                    throw new Exception("Invalid OrderByPropertie value");
                }
            }

            if (string.IsNullOrEmpty(param.OrderType))
            {
                param.OrderType = "DESC";
            }

            if (param.OrderType != "ASC" && param.OrderType != "DESC")
            {
                throw new Exception("OrderType must be 'ASC' or 'DESC'.");
            }

            param.AjustarParametros();

            orderByClause += $" {param.OrderType}";

            var query = $@"
                WITH diretoria AS (
                    SELECT
                        tco.cod_diretoria,
                        tco.diretoria,
                        ROW_NUMBER() OVER (PARTITION BY tco.cod_diretoria ORDER BY tco.diretoria) AS rn
                    FROM
                        tb_colaborador_org tco
                    WHERE
                        tco.tb_org_id = @OrgId
                )
                SELECT
                    tipp.id AS Id,
                    tipp.id_vaga AS VagaId,
                    tc.nome_completo AS NomeDoFourTalent,
                    tu.email AS EmailFourTalent,
                    tipp.nome_indicado AS NomeCompletoCandidado,
                    tipp.relacao_indicado AS DeOndeConhece,
                    tipp.disponivel AS DisponivelParaParticipar,
                    tipp.Autorizou AS AutorizouOEnvioDoCV,
                    tipp.linkedin AS LinkedinCandidato,
                    tipp.cv_valido AS CvValido,
                    tipp.retorno_ao_profissional AS RetornoAoProfissional,
                    tipp.perfil_avaliado AS PerfilAvaliado,
                    tipp.status AS Status,
                    tipp.data_criacao AS DataIndicacao,
                    tvs.titulo AS TituloDaVaga,
                    tvs.status_vaga AS StatusDaVaga,
                    dir.cod_diretoria AS Id,
                    dir.diretoria AS Descricao,
                    tc2.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc2.nome_completo AS Nome
                FROM
                    tb_indicacao_premiada_parcial tipp
                LEFT JOIN
                    tb_usuario tu ON tu.id = tipp.id_usuario_indicou
                LEFT JOIN
                    tb_colaborador tc ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                LEFT JOIN
                    tb_vagas_srs tvs ON tvs.id_vaga = tipp.id_vaga
                LEFT JOIN (
                    SELECT * FROM diretoria WHERE rn = 1
                ) dir ON dir.cod_diretoria = tipp.cod_diretoria
                LEFT JOIN
                    tb_colaborador tc2 ON tc2.codigo_interno_colaborador = tipp.codigo_colaborador_interno_analista
                WHERE
                    (@DataDe IS NULL OR tipp.data_criacao > @DataDe )
                    AND (@DataAte IS NULL OR tipp.data_criacao < @DataAte )
                    AND (@CodigoInternoColaboradorAnalista IS NULL OR tc2.codigo_interno_colaborador = @CodigoInternoColaboradorAnalista )
                    AND (@CvValido IS NULL OR tipp.cv_valido = @CvValido )
                    AND (@RetornoAoProfissional IS NULL OR tipp.retorno_ao_profissional = @RetornoAoProfissional )
                    AND (@CodDiretoria IS NULL OR dir.cod_diretoria = @CodDiretoria )
                    AND (@Pesquisa IS NULL OR
                        (
                                LOWER(tu.email) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tc.nome_completo) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tvs.titulo) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tvs.status_vaga) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tipp.nome_indicado) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tipp.status) LIKE LOWER(CONCAT('%', @Pesquisa, '%')) OR
                                LOWER(tipp.id_vaga) LIKE LOWER(CONCAT('%', @Pesquisa, '%'))
                        )
                    )
                {orderByClause}
                {limitClause};";

            var parametros = new
            {
                param.Limite,
                param.OrderByPropertie,
                param.OrderType,
                param.DataDe,
                param.DataAte,
                param.CodigoInternoColaboradorAnalista,
                param.CvValido,
                param.RetornoAoProfissional,
                param.CodDiretoria,
                param.Pesquisa,
                OrgId = orgId
            };

            var pagedQuery = await connection.QueryAsync<IndicacaoPremiadaParcialDTO, UnidadesDTO, AnalistaResponsavelDTO, IndicacaoPremiadaParcialDTO>(query,
                (indicacaoDTO, unidadeDTO, analistaResponsavelDTO) =>
                {
                    indicacaoDTO.Diretoria = unidadeDTO;
                    indicacaoDTO.AnalistaResponasvel = analistaResponsavelDTO;
                    return indicacaoDTO;
                }, splitOn: "Id,CodigoInternoColaborador", param: parametros);

            return pagedQuery.ToList();
        }

        public async Task<IndicacaoPremiadaParcialDTO> EditarIndicacaoPremiadaParcial(EditarIndicacaoPremiadaParcialParam param, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            try
            {
                var query = @"
                    UPDATE tb_indicacao_premiada_parcial tipp
                    SET
                        tipp.cv_valido = @CvValido,
                        tipp.retorno_ao_profissional = @RetornoAoProfissional,
                        tipp.perfil_avaliado = @PerfilAvaliado,
                        tipp.status = @Status,
                        tipp.cod_diretoria = @CodDiretoria,
                        tipp.codigo_colaborador_interno_analista = @CodigoInternoColaboradorAnalista
                    WHERE
                        tipp.id = @Id;
                ";

                var parametros = new
                {
                    param.Id,
                    param.CvValido,
                    param.RetornoAoProfissional,
                    param.PerfilAvaliado,
                    param.Status,
                    param.CodDiretoria,
                    param.CodigoInternoColaboradorAnalista
                };

                await connection.ExecuteAsync(query, parametros);

                var indicacaoAtualizada = await BuscarIndicacaoPremiadaPorId(param.Id, orgId);

                return indicacaoAtualizada;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> VerificaSeExisteIndicacaoPremiadaParcial(int id)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 1 FROM tb_indicacao_premiada_parcial tipp
                WHERE
                    tipp.id = @Id;
            ";
            var parametros = new { Id = id };

            var result = await connection.QueryAsync(query, parametros);

            return result.FirstOrDefault() != null;
        }

        public async Task<IndicacaoPremiadaParcialDTO> BuscarIndicacaoPremiadaPorId(int id, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = $@"
                WITH diretoria AS (
                    SELECT
                        tco.cod_diretoria,
                        tco.diretoria,
                        ROW_NUMBER() OVER (PARTITION BY tco.cod_diretoria ORDER BY tco.diretoria) AS rn
                    FROM
                        tb_colaborador_org tco
                    WHERE
                        tco.tb_org_id = @OrgId
                )
                SELECT
                    tipp.id AS Id,
                    tipp.id_vaga AS VagaId,
                    tc.nome_completo AS NomeDoFourTalent,
                    tu.email AS EmailFourTalent,
                    tipp.nome_indicado AS NomeCompletoCandidado,
                    tipp.relacao_indicado AS DeOndeConhece,
                    tipp.disponivel AS DisponivelParaParticipar,
                    tipp.Autorizou AS AutorizouOEnvioDoCV,
                    tipp.linkedin AS LinkedinCandidato,
                    tipp.cv_valido AS CvValido,
                    tipp.retorno_ao_profissional AS RetornoAoProfissional,
                    tipp.perfil_avaliado AS PerfilAvaliado,
                    tipp.status AS Status,
                    tipp.data_criacao AS DataIndicacao,
                    tvs.titulo AS TituloDaVaga,
                    tvs.status_vaga AS StatusDaVaga,
                    dir.cod_diretoria AS Id,
                    dir.diretoria AS Descricao,
                    tc2.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc2.nome_completo AS Nome
                FROM
                    tb_indicacao_premiada_parcial tipp
                LEFT JOIN
                    tb_usuario tu ON tu.id = tipp.id_usuario_indicou
                LEFT JOIN
                    tb_colaborador tc ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador
                LEFT JOIN
                    tb_vagas_srs tvs ON tvs.id_vaga = tipp.id_vaga
                LEFT JOIN (
                    SELECT * FROM diretoria WHERE rn = 1
                ) dir ON dir.cod_diretoria = tipp.cod_diretoria
                LEFT JOIN
                    tb_colaborador tc2 ON tc2.codigo_interno_colaborador = tipp.codigo_colaborador_interno_analista
                WHERE
                   tipp.id = @Id;";

            var parametros = new
            {
                Id = id,
                OrgId = orgId
            };

            var pagedQuery = await connection.QueryAsync<IndicacaoPremiadaParcialDTO, UnidadesDTO, AnalistaResponsavelDTO, IndicacaoPremiadaParcialDTO>(query,
                (indicacaoDTO, unidadeDTO, analistaResponsavelDTO) =>
                {
                    indicacaoDTO.Diretoria = unidadeDTO;
                    indicacaoDTO.AnalistaResponasvel = analistaResponsavelDTO;
                    return indicacaoDTO;
                }, splitOn: "Id , CodigoInternoColaborador", param: parametros);

            return pagedQuery.FirstOrDefault();
        }

        public void ValidarIndicacaoParcial(long idIndicacaoParc)
        {
            var vagasIndicadas = _colaboradorContext.tb_indicacao_premiada_parcial.Where(x => x.id == idIndicacaoParc && x.utilizado == 0).First();
            if (vagasIndicadas != null)
            {
                vagasIndicadas.utilizado = 1;
                _colaboradorContext.SaveChanges();
            }
            else
            {
                throw new Exception("Indicação não encontrada!");
            }
        }

        public void AddConviteIndicacaoParcial(long idTb, string convite, string pathCurriculo)
        {
            var indicacaoRow = _colaboradorContext.tb_indicacao_premiada_parcial.Where(x => x.id == idTb).FirstOrDefault();
            indicacaoRow.link_indicacao = convite;
            indicacaoRow.path_curriculo = pathCurriculo;
            _colaboradorContext.SaveChanges();
        }

        //public async Task CriarOuAtualizarOrdemDeTrabalho(List<VagaDTO> vagas)
        //{
        //    try
        //    {
        //        foreach (var vagaDTO in vagas)
        //        {
        //            var vagaExistente = await VerificarExistenciaVaga(vagaDTO.IdVaga);

        //            if (vagaExistente != null)
        //            {
        //                AtualizarVaga(vagaExistente, vagaDTO);
        //            }
        //            else
        //            {
        //                CriarNovaVaga(vagaDTO);
        //            }
        //        }
        //        foreach (var vagaDTO in vagas)
        //        {
        //            await AtualizarOuCriarSkills(vagaDTO);
        //            await AtualizarOuCriarCandidaturas(vagaDTO);
        //            await AtualizarOuCriarSkillsCandidatos(vagaDTO);

        //        }
        //        await _colaboradorContext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred while updating the entries: " + ex.Message);
        //        Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
        //        throw;
        //    }
        //}

        //private async Task<tb_vaga> VerificarExistenciaVaga(int idVaga)
        //{
        //    if (idVaga == null)
        //    {
        //        return null;
        //    }
        //    return await _colaboradorContext.tb_vaga.FirstOrDefaultAsync(v => v.id_vaga == idVaga);
        //}

        //private void AtualizarVaga(tb_vaga vagaExistente, VagaDTO novaVaga)
        //{
        //    vagaExistente.id_vaga = novaVaga.IdVaga;
        //    vagaExistente.email = novaVaga.Email;
        //    vagaExistente.titulo = novaVaga.Titulo;
        //    vagaExistente.status = novaVaga.Status;
        //    vagaExistente.tipo = novaVaga.Tipo;
        //    vagaExistente.numero_de_vagas = novaVaga.NumeroDeVagas;
        //    vagaExistente.taxa_maxima_hora = novaVaga.TaxaMaximaPorHora;
        //    vagaExistente.nivel_de_urgencia = novaVaga.NivelDeUrgencia;
        //    vagaExistente.descricao = novaVaga.Descricao;
        //    vagaExistente.funcao = novaVaga.Funcao;
        //    vagaExistente.data_criacao = novaVaga.DataCriacao;
        //    vagaExistente.data_atualizacao = novaVaga.DataAtualizacao;
        //    vagaExistente.data_publicacao = novaVaga.DataPublicacao;
        //    vagaExistente.data_aceitacao = novaVaga.DataAceitacao;
        //    vagaExistente.tipo_localizacao = novaVaga.TipoLocalizacao;
        //    vagaExistente.observacao_localizacao = novaVaga.ObservacaoLocalizacao;
        //    vagaExistente.estado = novaVaga.Estado;
        //    vagaExistente.cidade = novaVaga.Cidade;

        //    _colaboradorContext.tb_vaga.Update(vagaExistente);
        //}

        //private void CriarNovaVaga(VagaDTO novaVaga)
        //{
        //    var dadosOrdemTrabalho = new tb_vaga
        //    {
        //        id_vaga = novaVaga.IdVaga,
        //        email = novaVaga.Email,
        //        titulo = novaVaga.Titulo,
        //        status = novaVaga.Status,
        //        tipo = novaVaga.Tipo,
        //        numero_de_vagas = novaVaga.NumeroDeVagas,
        //        taxa_maxima_hora = novaVaga.TaxaMaximaPorHora,
        //        nivel_de_urgencia = novaVaga.NivelDeUrgencia,
        //        descricao = novaVaga.Descricao,
        //        funcao = novaVaga.Funcao,
        //        data_criacao = novaVaga.DataCriacao,
        //        data_atualizacao = novaVaga.DataAtualizacao,
        //        data_publicacao = novaVaga.DataPublicacao,
        //        data_aceitacao = novaVaga.DataAceitacao,
        //        tipo_localizacao = novaVaga.TipoLocalizacao,
        //        observacao_localizacao = novaVaga.ObservacaoLocalizacao,
        //        estado = novaVaga.Estado,
        //        cidade = novaVaga.Cidade
        //    };
        //    _colaboradorContext.tb_vaga.Add(dadosOrdemTrabalho);
        //}
        //private async Task AtualizarOuCriarSkills(VagaDTO vaga)
        //{
        //    if (vaga.Skills == null || vaga.Skills.Count == 0)
        //        return;

        //    foreach (var novaSkill in vaga.Skills)
        //    {
        //        var skillExistente = await _colaboradorContext.tb_skill_vaga_srs.FirstOrDefaultAsync(s =>
        //            s.vaga_id == vaga.IdVaga &&
        //            s.nivel_id == novaSkill.NivelId &&
        //            s.categoria_id == novaSkill.CategoriaId &&
        //            s.descricao_id == novaSkill.DescricaoId);

        //        if (skillExistente != null)
        //        {
        //            skillExistente.nivel_id = novaSkill.NivelId;
        //            skillExistente.categoria_id = novaSkill.CategoriaId;
        //            skillExistente.descricao_id = novaSkill.DescricaoId;
        //            skillExistente.data_criacao = novaSkill.DataCriacao;
        //            skillExistente.data_alteracao = novaSkill.DataAlteracao;
        //            _colaboradorContext.tb_skill_vaga_srs.Update(skillExistente);
        //        }
        //        else
        //        {
        //            var novaSkillVaga = new tb_skill_vaga_srs
        //            {
        //                vaga_id = vaga.IdVaga,
        //                nivel_id = novaSkill.NivelId,
        //                categoria_id = novaSkill.CategoriaId,
        //                descricao_id = novaSkill.DescricaoId,
        //                data_criacao = novaSkill.DataCriacao,
        //                data_alteracao = novaSkill.DataAlteracao
        //            };

        //            _colaboradorContext.tb_skill_vaga_srs.Add(novaSkillVaga);

        //        }
        //    }
        //}

        //private async Task AtualizarOuCriarCandidaturas(VagaDTO vaga)
        //{
        //    if (vaga.Candidatos == null || vaga.Candidatos.Count == 0)
        //        return;

        //    foreach (var novaCandidatura in vaga.Candidatos)
        //    {
        //        var candidaturaExistente = await _colaboradorContext.tb_candidatos_vaga.FirstOrDefaultAsync(c =>
        //            c.id_vaga == vaga.IdVaga &&
        //            c.email == novaCandidatura.CandidatoEmail);

        //        if (candidaturaExistente != null)
        //        {
        //            candidaturaExistente.nome = novaCandidatura.CandidatoNome;
        //            candidaturaExistente.status = novaCandidatura.Status;
        //            candidaturaExistente.candidato_id = novaCandidatura.CandidatoId;
        //            _colaboradorContext.tb_candidatos_vaga.Update(candidaturaExistente);
        //        }
        //        else
        //        {
        //            var novaCandidaturaVaga = new tb_candidatos_vaga
        //            {
        //                id_vaga = vaga.IdVaga,
        //                nome = novaCandidatura.CandidatoNome,
        //                email = novaCandidatura.CandidatoEmail,
        //                candidato_id = novaCandidatura.CandidatoId,
        //                status = novaCandidatura.Status
        //            };

        //            _colaboradorContext.tb_candidatos_vaga.Add(novaCandidaturaVaga);
        //        }
        //    }
        //}

        //private async Task AtualizarOuCriarSkillsCandidatos(VagaDTO vaga)
        //{
        //    if (vaga.SkillsCandidatos == null || vaga.SkillsCandidatos.Count == 0)
        //        return;
        //    foreach (var novaSkillCandidato in vaga.SkillsCandidatos)
        //    {
        //        var skillExistente = await _colaboradorContext.tb_skill_candidato_srs.FirstOrDefaultAsync(s =>
        //           s.candidato_id == novaSkillCandidato.CandidatoId &&
        //           s.nivel_id == novaSkillCandidato.NivelId &&
        //           s.categoria_id == novaSkillCandidato.CategoriaId &&
        //           s.descricao_id == novaSkillCandidato.DescricaoId);

        //        if (skillExistente != null)
        //        {
        //            skillExistente.nivel_id = novaSkillCandidato.NivelId;
        //            skillExistente.categoria_id = novaSkillCandidato.CategoriaId;
        //            skillExistente.descricao_id = novaSkillCandidato.DescricaoId;
        //            skillExistente.data_criacao = novaSkillCandidato.DataCriacao;
        //            skillExistente.data_alteracao = novaSkillCandidato.DataAlteracao;
        //            _colaboradorContext.tb_skill_candidato_srs.Update(skillExistente);
        //        }
        //        else
        //        {
        //            var novaSkillVaga = new tb_skill_candidato_srs
        //            {
        //                candidato_id = novaSkillCandidato.CandidatoId,
        //                nivel_id = novaSkillCandidato.NivelId,
        //                categoria_id = novaSkillCandidato.CategoriaId,
        //                descricao_id = novaSkillCandidato.DescricaoId,
        //                data_criacao = novaSkillCandidato.DataCriacao,
        //                data_alteracao = novaSkillCandidato.DataAlteracao
        //            };
        //            _colaboradorContext.tb_skill_candidato_srs.Add(novaSkillVaga);
        //        }
        //    }
        //}

        public bool ValidaAcessoAdministrador(string token)
        {
            try
            {
                if (token == VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
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
        }

        public async Task<IEnumerable<VagaOrquestracaoDTO>> ListarVagaOrquestracao(string email)
        {
            try
            {
                var vagaOrquestracaoDTOs = await _colaboradorContext.tb_vaga
                    .Where(v => v.email == email)
                    .Select(v => new VagaOrquestracaoDTO
                    {
                        IdVaga = v.id_vaga,
                        Titulo = v.titulo,
                        Status = v.status,
                        DataAprovacao = v.data_aceitacao,
                        Candidaturas = _colaboradorContext.tb_candidatos_vaga
                            .Count(vc => vc.id_vaga == v.id_vaga).ToString(),
                        HardSkill = _colaboradorContext.tb_skill_vaga_srs
                            .Where(s => s.vaga_id == v.id_vaga && s.categoria_id == (int)CategoriaSkillEnum.Hardskills && s.descricao_id != null)
                            .Select(s => _colaboradorContext.tb_competencia
                                .FirstOrDefault(c => c.id == s.descricao_id))
                            .Where(c => c != null)
                            .Select(c => c.descricao)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return vagaOrquestracaoDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }

        public async Task<IEnumerable<TotalizadoresVagaDTO>> TotalizadoresVaga(long? idVaga)
        {
            try
            {
                var totalizadoresDTO = await _colaboradorContext.tb_vaga
                    .Where(v => v.id_vaga == idVaga)
                    .Select(v => new TotalizadoresVagaDTO
                    {
                        Candidaturas = _colaboradorContext.tb_candidatos_vaga
                            .Count(vc => vc.id_vaga == v.id_vaga).ToString(),
                        Aprovados = _colaboradorContext.tb_candidatos_vaga
                    .Count(x => x.status == "Aprovado" && x.id_vaga == v.id_vaga).ToString(),
                        Reprovados = _colaboradorContext.tb_candidatos_vaga
                    .Count(x => x.status == "Reprovado" && x.id_vaga == v.id_vaga).ToString()
                    })
                    .ToListAsync();

                return totalizadoresDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }
        public async Task<ActionResult<IEnumerable<CanditatoVagaSrsDTO>>> ListarCandidatosPorVaga(long? idVaga)
        {
            try
            {
                var competenciasDaVaga = await ObterCompetenciasDaVaga(idVaga);

                var candidatosDTO = await _colaboradorContext.tb_candidatos_vaga
                    .Where(vc => vc.id_vaga == idVaga)
                    .Select(vc => new CanditatoVagaSrsDTO
                    {
                        IdVaga = vc.id_vaga,
                        CandidatoNome = vc.nome,
                        CandidatoEmail = vc.email,
                        SoftSkills = _colaboradorContext.tb_skill_candidato_srs
                            .Where(scs => scs.candidato_id == vc.candidato_id && scs.categoria_id == (int)CategoriaSkillEnum.Softskills && scs.candidato_id != null)
                            .Select(scs => _colaboradorContext.tb_softskill
                                .FirstOrDefault(c => c.id == scs.descricao_id))
                            .Where(c => c != null)
                            .Select(c => c.descricao)
                            .FirstOrDefault(),
                        HardSkills = _colaboradorContext.tb_skill_candidato_srs
                            .Where(scs => scs.candidato_id == vc.candidato_id && scs.categoria_id == (int)CategoriaSkillEnum.Hardskills && scs.candidato_id != null)
                            .Select(scs => _colaboradorContext.tb_competencia
                                .FirstOrDefault(c => c.id == scs.descricao_id))
                            .Where(c => c != null)
                            .Select(c => c.descricao)
                            .FirstOrDefault(),
                        Status = vc.status,
                        Aderencia = CalcularAderencia(vc, competenciasDaVaga)
                    })
                    .ToListAsync();

                return candidatosDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }

        private static double CalcularAderencia(tb_candidatos_vaga candidato, List<FiltroCompetenciaDTO> competencias)
        {
            int quantidadeTotalCompetencias = competencias.Count;

            double totalAderencia = competencias.Select(competencia =>
            {
                using (var innerContext = new ColaboradorContext())
                {
                    var hardskill = innerContext.tb_skill_candidato_srs.FirstOrDefault(h => h.candidato_id == candidato.candidato_id && h.categoria_id == (int)CategoriaSkillEnum.Hardskills && h.descricao_id == competencia.Id);
                    var softskill = innerContext.tb_skill_candidato_srs.FirstOrDefault(s => s.candidato_id == candidato.candidato_id && s.categoria_id == (int)CategoriaSkillEnum.Softskills && s.descricao_id == competencia.Id);

                    if ((competencia.Tipo.ToUpper() == "HARDSKILL" && hardskill != null) ||
                        (competencia.Tipo.ToUpper() == "SOFTSKILL" && softskill != null))
                    {
                        double peso = 1.0 / quantidadeTotalCompetencias;

                        if ((competencia.NivelId == TODOS_NIVEIS) ||
                            (hardskill != null && hardskill.nivel_id == competencia.NivelId) ||
                            (softskill != null && softskill.nivel_id == competencia.NivelId))
                        {
                            return peso;
                        }
                        else
                        {
                            return peso / MEIO_PESO;
                        }
                    }
                }

                return 0.0;
            }).Sum();

            double aderenciaPercentual = Math.Round(totalAderencia * 100.0, 2);
            return aderenciaPercentual;
        }

        private async Task<List<FiltroCompetenciaDTO>> ObterCompetenciasDaVaga(long? idVaga)
        {
            var competenciasDaVaga = await _colaboradorContext.tb_skill_vaga_srs
                .Where(sv => sv.vaga_id == idVaga && sv.categoria_id == (int)CategoriaSkillEnum.Hardskills)
                .Select(sv => sv.descricao_id)
                .ToListAsync();

            var competencias = await _colaboradorContext.tb_competencia
                .Where(c => competenciasDaVaga.Contains((int?)c.id))
                .Select(c => new FiltroCompetenciaDTO
                {
                    Id = c.id,
                    Tipo = "HARDSKILL"
                })
                .ToListAsync();

            return competencias;
        }

        public async Task<bool> VerificarVagaExistente(long? idVaga)
        {
            try
            {
                var vaga = await _colaboradorContext.tb_vaga.FirstOrDefaultAsync(v => v.id_vaga == idVaga);

                if (vaga == null)
                {
                    return false;
                }
                return vaga.status == StatusVagaConst.EmAndamento;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while verifying the vacancy: " + ex.Message);
                Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
                throw;
            }
        }

        public bool CandidatoIndicadoVaga(long idVaga, string urlLinkedin)
        {
            return _colaboradorContext.tb_vagas_indicadas.Where(x => x.id_vaga == idVaga && x.url_linkedin_candidato == urlLinkedin).Any();
        }

        public async Task InserirVagaCandidato(string codInternoColaborador, long vagaId, int orgId, int candidatoId, int status)
        {
            var connection = _dapperConnection.GetConnection();

            var id = Guid.NewGuid();

            string query = @$"
                                INSERT INTO
                                    tb_vaga_candidato (
                                        id,
                                        codigo_interno_colaborador,
                                        vaga_id,
                                        tb_org_id,
                                        candidato_id,
                                        status_id
                                    )
                                VALUES
                                    (
                                        @id,
                                        @codigo_interno_colaborador,
                                        @vaga_id,
                                        @tb_org_id,
                                        @candidato_id,
                                        @status
                                    );
                            ";

            var parametros = new
            {
                id,
                codigo_interno_colaborador = codInternoColaborador,
                vaga_id = vagaId,
                tb_org_id = orgId,
                candidato_id = candidatoId,
                status
            };

            await connection.ExecuteAsync(query, parametros);
        }

        public async Task RemoverVagaCandidato(int candidatoId, long vagaId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @$"
                                UPDATE
                                    tb_vaga_candidato
                                SET
                                    ativo = 0
                                WHERE
                                    candidato_id = @candidato_id
                                    AND vaga_id = @vaga_id;
                            ";

            var parametros = new
            {
                candidato_id = candidatoId,
                vaga_id = vagaId
            };

            await connection.ExecuteAsync(query, parametros);
        }

    }
}