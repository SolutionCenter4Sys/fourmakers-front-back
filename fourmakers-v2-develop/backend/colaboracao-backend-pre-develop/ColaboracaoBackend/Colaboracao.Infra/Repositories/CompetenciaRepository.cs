using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util.Competencia;
using Colaboracao.Infra.Context;
using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class CompetenciaRepository : ICompetenciaDtoRepository
    {
        private const int ITEM_PERFIL_HARDSKILL = 1;
        private const int ITEM_PERFIL_SOFTSKILL = 8;
        private const int ITEM_PERFIL_IDIOMA = 9;
        private const int ITEM_PERFIL_DOMINIO = 4;
        private const int ITEM_PERFIL_METODOLOGIA = 3;
        private const int TODOS_NIVEIS = 0;
        private const double MEIO_PESO = 2.0;
        private const string NIVEL_HARDSKILL = "NivelHardskill";
        private const string NIVEL_SOFTSKILL = "NivelSoftskill";
        private const string NIVEL_IDIOMA = "NivelIdioma";
        private const string NIVEL_DOMINIO = "NivelDominio";
        private const string NIVEL_METODOLOGIA = "NivelMetodologia";
        private const string SOFTSKILL = "SOFTSKILL";
        private const string HARDSKILL = "HARDSKILL";
        private const string METODOLOGIA = "METODOLOGIA";
        private const string DOMINIO = "DOMINIO";
        private const string IDIOMA = "IDIOMA";
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IHistoricoCVRepository _historicoCV;
        private readonly IDBConnection _dapperConnection;

        public CompetenciaRepository(ColaboradorContext colaboradorContext,
                                     IConnectionStringCore connectionString,
                                     IDBConnection dapperConnection,
                                     IHistoricoCVRepository historicoCV)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
            _historicoCV = historicoCV;
        }

        public long? GetCompetenciaColabRowId(long competenciaColaboradorId)
        {
            return _colaboradorContext.tb_colaborador_competencia.Find(competenciaColaboradorId)?.id;
        }

        private List<CompetenciaNivelDTO> ObterCompetencias(IEnumerable<tb_colaborador_competencia> competencias)
        {
            if (competencias.Count() == 0)
            {
                return new List<CompetenciaNivelDTO>();
            }
            var comp = competencias
                .Where(c => c.ativo == 1 && (c.tb_nivel_id != null || c.tb_nivel_id == null)) // Modificação aqui
                .Select(h => new CompetenciaNivelDTO
                {
                    Id = h.competencia_id,
                    Descricao = h.competencia.descricao,
                    Nivel = h.tb_nivel_id != null ? ObterNivel((int)h.tb_nivel_id) : null // Modificação aqui
                })
                .ToList();
            return comp;
        }

        private List<SkillNivelDTO> ObterSkills(IEnumerable<tb_colaborador_alocado_skill> skills, int tipoSkill)
        {
            if (skills.Count() == 0)
            {
                return new List<SkillNivelDTO>();
            }
            List<SkillNivelDTO> comp = skills.Where(x => x.tb_item_perfil_id == tipoSkill)
                .Select(h => new SkillNivelDTO
                {
                    Id = h.skill_id,
                    Descricao = GetSkillRealizacaoDescricao(h.skill_id, tipoSkill),
                    Nivel = h.tb_nivel_id != null ? ObterNivel((int)h.tb_nivel_id) : null
                })
                .ToList();

            return comp;
        }

        private string GetSkillRealizacaoDescricao(long skillId, int tipoSkill)
        {
            return tipoSkill switch
            {
                ITEM_PERFIL_HARDSKILL => _colaboradorContext.tb_competencia.Where(x => x.id == skillId)?.FirstOrDefault()?.descricao ?? "",
                ITEM_PERFIL_IDIOMA => _colaboradorContext.tb_idioma.Where(x => x.id == skillId)?.FirstOrDefault()?.descricao ?? "",
                ITEM_PERFIL_SOFTSKILL => _colaboradorContext.tb_softskill.Where(x => x.id == skillId)?.FirstOrDefault()?.descricao ?? "",
                ITEM_PERFIL_METODOLOGIA => _colaboradorContext.tb_metodologia.Where(x => x.id == skillId)?.FirstOrDefault()?.descricao ?? "",
                ITEM_PERFIL_DOMINIO => _colaboradorContext.tb_dominionegocio.Where(x => x.id == skillId)?.FirstOrDefault()?.descricao ?? "",
                _ => "",
            };
        }

        private List<IdiomaNivelDTO> ObterIdiomas(IEnumerable<tb_colaborador_idioma> idiomas)
        {
            if (idiomas.Count() == 0)
            {
                return new List<IdiomaNivelDTO>();
            }

            return idiomas
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new IdiomaNivelDTO
                {
                    Id = h.idioma_id,
                    Descricao = h.idioma.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        private List<SoftskillNivelDTO> ObterSoftskills(IEnumerable<tb_colaborador_softskill> softskills)
        {
            if (softskills.Count() == 0)
            {
                return new List<SoftskillNivelDTO>();
            }

            return softskills
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new SoftskillNivelDTO
                {
                    Id = h.softskill_id,
                    Descricao = h.softskill.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        private List<MetodologiaNivelDTO> ObterMetodologias(IEnumerable<tb_colaborador_metodologia> metodologias)
        {
            if (metodologias.Count() == 0)
            {
                return new List<MetodologiaNivelDTO>();
            }

            return metodologias
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new MetodologiaNivelDTO
                {
                    Id = h.metodologia_id,
                    Descricao = h.metodologia.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        private List<DominioNivelDTO> ObterDominio(IEnumerable<tb_colaborador_dominionegocio> dominios)
        {
            if (dominios.Count() == 0)
            {
                return new List<DominioNivelDTO>();
            }

            return dominios
                .Where(c => c.ativo == 1 && c.tb_nivel_id != null)
                .Select(h => new DominioNivelDTO
                {
                    Id = h.dominionegocio_id,
                    Descricao = h.dominionegocio.descricao,
                    Nivel = ObterNivel((int)h.tb_nivel_id)
                })
                .ToList();
        }

        private NivelDTO ObterNivel(int nivelId)
        {
            if (nivelId == 0)
            {
                return new NivelDTO();
            }

            var nivel = _colaboradorContext.tb_nivel
                .Where(n => n.id == nivelId)
                .Select(n => new NivelDTO
                {
                    Id = n.id,
                    Descricao = n.descricao
                })
                .FirstOrDefault();

            return nivel;
        }

        public CompetenciasSumarioResult BuscarColaboradorSumario(FiltroSumarioCompetenciasParam param, string token, int orgId)
        {
            var paramCompetencias = new List<FiltroCompetenciaDTO>(param.competencia);
            string serviceMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
            AtualizarSkillsComNivelTodos(param.competencia);

            var unidadesIdsList = param.UnidadeId.Select(id => id.ToString()).ToList();

            var colaboradoresUnidade = _colaboradorContext.tb_colaborador_org.Where(colaboradorOrg => colaboradorOrg.tb_org_id == orgId &&
                                                                                                      colaboradorOrg.ativo == 1 &&
                                                                                                      (unidadesIdsList.Contains(colaboradorOrg.cod_diretoria)
                                                                                                      || (param.UnidadeId.Contains(0))));
            List<string> perfilGenerico = new List<string>();
            List<string> perfilMapa = new List<string>();
            if (param.perfilId != null && param.perfilId.Any())
            {
                perfilGenerico = param.perfilId.Where(x => x.StartsWith("1|")).Select(x => x.Split("|")[1]).ToList();
                perfilMapa = param.perfilId.Where(x => x.StartsWith("2|")).Select(x => x.Split("|")[1]).ToList();
            }

            if (param.competencia != null && param.competencia.Any())
            {
                var competenciaIds = param.competencia
                                        .Where(comp => comp.Tipo == "HARDSKILL")
                                        .Select(comp => comp.Id)
                                        .ToList();

                var idiomaIds = param.competencia
                                    .Where(comp => comp.Tipo == "IDIOMA")
                                    .Select(comp => comp.Id)
                                    .ToList();

                var softskillIds = param.competencia
                                        .Where(comp => comp.Tipo == "SOFTSKILL")
                                        .Select(comp => comp.Id)
                                        .ToList();

                var metodologiaIds = param.competencia
                                        .Where(comp => comp.Tipo == "METODOLOGIA")
                                        .Select(comp => comp.Id)
                                        .ToList();

                var dominioIds = param.competencia
                                        .Where(comp => comp.Tipo == "DOMINIO")
                                        .Select(comp => comp.Id)
                                        .ToList();

                colaboradoresUnidade = colaboradoresUnidade
                    .Where(u =>
                        //hardskill
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_competencia
                            .Any(c => competenciaIds.Contains(c.competencia_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_colaborador_alocado_skill.Any(x => competenciaIds.Contains(x.skill_id) && x.tb_item_perfil_id == ITEM_PERFIL_HARDSKILL) && c.ativo == 1 && c.retroalimenta_cv == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        //idioma
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_idioma
                            .Any(c => idiomaIds.Contains(c.idioma_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_colaborador_alocado_skill.Any(x => idiomaIds.Contains(x.skill_id) && x.tb_item_perfil_id == ITEM_PERFIL_IDIOMA) && c.ativo == 1 && c.retroalimenta_cv == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        //softskill
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_softskill
                            .Any(c => softskillIds.Contains(c.softskill_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_colaborador_alocado_skill.Any(x => softskillIds.Contains(x.skill_id) && x.tb_item_perfil_id == ITEM_PERFIL_SOFTSKILL) && c.ativo == 1 && c.retroalimenta_cv == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        //metodologia
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_metodologia
                            .Any(c => metodologiaIds.Contains(c.metodologia_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_colaborador_alocado_skill.Any(x => metodologiaIds.Contains(x.skill_id) && x.tb_item_perfil_id == ITEM_PERFIL_METODOLOGIA) && c.ativo == 1 && c.retroalimenta_cv == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        //dominio de negocio
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_dominionegocio
                            .Any(c => dominioIds.Contains(c.dominionegocio_id) && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_colaborador_alocado_skill.Any(x => dominioIds.Contains(x.skill_id) && x.tb_item_perfil_id == ITEM_PERFIL_DOMINIO) && c.ativo == 1 && c.retroalimenta_cv == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                         u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_perfil_alocacao.Where(x => perfilGenerico.Contains(x.tb_perfil_id)).Any() && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_perfil_alocacao.Where(x => perfilMapa.Contains(x.tb_gestor_externo_perfil_id)).Any() && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                    );
            }
            else if (perfilGenerico.Count > 0 || perfilMapa.Count > 0)
            {
                colaboradoresUnidade = colaboradoresUnidade
                    .Where(u =>
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_perfil_alocacao.Where(x => perfilGenerico.Contains(x.tb_perfil_id)).Any() && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                        ||
                        u.codigo_interno_colaboradorNavigation.tb_colaborador_periodo_alocacao
                            .Any(c => c.tb_perfil_alocacao.Where(x => perfilMapa.Contains(x.tb_gestor_externo_perfil_id)).Any() && c.ativo == 1
                            && c.codigo_interno_colaboradorNavigation.tb_colaborador_org.Any(x => x.codigo_interno_colaborador == u.codigo_interno_colaborador && x.tb_org_id == orgId))
                    );
            }

            var resultColabsUnidade = colaboradoresUnidade.ToList();

            var colaboradoresCpf = _colaboradorContext.tb_colaborador_org
                .Where(u =>
                    (param.Cpf == null || param.Cpf.Contains(u.codigo_interno_colaborador))
                    && u.ativo == 1 && u.tb_org_id == orgId
                )
                .ToList();//.Where(x => !resultColabsUnidade.Where(y => x.codigo_interno_colaborador == y.codigo_interno_colaborador).Any()).ToList();

            if (param.Cpf.Count >= 1 && param.competencia.Count < 1)
            {
                resultColabsUnidade.RemoveAll(x => x.codigo_interno_colaborador != null);
            }

            var resultado = colaboradoresCpf
            .Union(resultColabsUnidade)
            .Skip(param.Cursor)
            .Take(param.Limite);

            var ret = resultado
                    .Select(x =>
                    {
                        var skillsRealizacoes = _colaboradorContext.tb_colaborador_alocado_skill
                            .Where(y => y.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == x.codigo_interno_colaborador
                                && y.tb_colaborador_periodo_alocacao.tb_org_id == x.tb_org_id && y.tb_colaborador_periodo_alocacao.ativo == 1
                                && y.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1)
                            .ToList();
                        var competencias = ObterCompetencias(x.codigo_interno_colaboradorNavigation.tb_colaborador_competencia);
                        competencias.AddRange(ObterSkills(skillsRealizacoes, ITEM_PERFIL_HARDSKILL)
                            .Select(y => new CompetenciaNivelDTO
                            {
                                Id = y.Id,
                                Descricao = y.Descricao,
                                Nivel = y.Nivel
                            })
                        );
                        competencias = competencias.Select(x => x.Id).Distinct().Select(x =>
                        {
                            var compReg = competencias.First(y => y.Id == x);
                            return new CompetenciaNivelDTO
                            {
                                Id = compReg.Id,
                                Descricao = compReg.Descricao,
                                Nivel = compReg.Nivel
                            };
                        }).OrderBy(x => x.Descricao).ToList();
                        var idiomas = ObterIdiomas(x.codigo_interno_colaboradorNavigation.tb_colaborador_idioma);
                        idiomas.AddRange(ObterSkills(skillsRealizacoes, ITEM_PERFIL_IDIOMA)
                            .Select(y => new IdiomaNivelDTO
                            {
                                Id = y.Id,
                                Descricao = y.Descricao,
                                Nivel = y.Nivel
                            })
                        );
                        idiomas = idiomas.Select(x => x.Id).Distinct().Select(x =>
                        {
                            var compReg = idiomas.First(y => y.Id == x);
                            return new IdiomaNivelDTO
                            {
                                Id = compReg.Id,
                                Descricao = compReg.Descricao,
                                Nivel = compReg.Nivel
                            };
                        }).OrderBy(x => x.Descricao).ToList();
                        var softskills = ObterSoftskills(x.codigo_interno_colaboradorNavigation.tb_colaborador_softskill);
                        softskills.AddRange(ObterSkills(skillsRealizacoes, ITEM_PERFIL_SOFTSKILL)
                            .Select(y => new SoftskillNivelDTO
                            {
                                Id = y.Id,
                                Descricao = y.Descricao,
                                Nivel = y.Nivel
                            })
                        );
                        softskills = softskills.Select(x => x.Id).Distinct().Select(x =>
                        {
                            var compReg = softskills.First(y => y.Id == x);
                            return new SoftskillNivelDTO
                            {
                                Id = compReg.Id,
                                Descricao = compReg.Descricao,
                                Nivel = compReg.Nivel
                            };
                        }).OrderBy(x => x.Descricao).ToList();
                        var metodologias = ObterMetodologias(x.codigo_interno_colaboradorNavigation.tb_colaborador_metodologia);
                        metodologias.AddRange(ObterSkills(skillsRealizacoes, ITEM_PERFIL_METODOLOGIA)
                            .Select(y => new MetodologiaNivelDTO
                            {
                                Id = y.Id,
                                Descricao = y.Descricao,
                                Nivel = y.Nivel
                            })
                        );
                        metodologias = metodologias.Select(x => x.Id).Distinct().Select(x =>
                        {
                            var compReg = metodologias.First(y => y.Id == x);
                            return new MetodologiaNivelDTO
                            {
                                Id = compReg.Id,
                                Descricao = compReg.Descricao,
                                Nivel = compReg.Nivel
                            };
                        }).OrderBy(x => x.Descricao).ToList();
                        var dominios = ObterDominio(x.codigo_interno_colaboradorNavigation.tb_colaborador_dominionegocio);
                        dominios.AddRange(ObterSkills(skillsRealizacoes, ITEM_PERFIL_DOMINIO)
                            .Select(y => new DominioNivelDTO
                            {
                                Id = y.Id,
                                Descricao = y.Descricao,
                                Nivel = y.Nivel
                            })
                        );
                        dominios = dominios.Select(x => x.Id).Distinct().Select(x =>
                        {
                            var compReg = dominios.First(y => y.Id == x);
                            return new DominioNivelDTO
                            {
                                Id = compReg.Id,
                                Descricao = compReg.Descricao,
                                Nivel = compReg.Nivel
                            };
                        }).OrderBy(x => x.Descricao).ToList();
                        var imagePath = "";
                        var nomeGestor = ObterNomeGestor(x.codigo_interno_colaboradorNavigation);
                        var ultimaAtualizacao = _historicoCV.GetUltimaAtualizacao(x.codigo_interno_colaborador);
                        if (x.codigo_interno_colaboradorNavigation.imagem != null && x.codigo_interno_colaboradorNavigation.imagem.path != null)
                        {
                            imagePath = serviceMidia.Replace("$1", token) + x.codigo_interno_colaboradorNavigation.imagem.path;
                        }
                        return new ColaboradorSumarioDTO()
                        {
                            Id = x.codigo_interno_colaboradorNavigation.tb_usuario.Where(a => a.tb_org_id == x.tb_org_id).FirstOrDefault()?.id,
                            NomeCompleto = x.codigo_interno_colaboradorNavigation.nome_completo,
                            Aderencia = CalcularAderencia(x.codigo_interno_colaboradorNavigation, paramCompetencias, _colaboradorContext),
                            ImagePath = imagePath,
                            Cpf = x.codigo_interno_colaborador,
                            Gestor = nomeGestor,
                            Unidade = x?.diretoria ?? "",
                            Hardskills = competencias,
                            Idiomas = idiomas,
                            Softskills = softskills,
                            Dominios = dominios,
                            Metodologias = metodologias,
                            UltimaAtualizacao = ultimaAtualizacao
                        };
                    });

            var retorno = new CompetenciasSumarioResult
            {
                ListaColaboradoresSumario = ret.OrderByDescending(x => x.Aderencia).ToList()
            };

            return retorno;
        }

        private string ObterNomeGestor(tb_colaborador x)
        {
            var ret = _colaboradorContext.vw_gestores_colaboradores_org.Where(c => c.codigo_interno_colaborador_subordinado == x.codigo_interno_colaborador).Select(c => c.nome_completo_gestor).FirstOrDefault();
            if (ret == null)
            {
                ret = "";
            }
            return ret;
        }

        private void AtualizarSkillsComNivelTodos(List<FiltroCompetenciaDTO> param)
        {
            var mapeamentoCompetencia = new Dictionary<string, int>
                {{ HARDSKILL, ITEM_PERFIL_HARDSKILL },
                { SOFTSKILL, ITEM_PERFIL_SOFTSKILL },
                { IDIOMA, ITEM_PERFIL_IDIOMA}};

            var competencias = param
                .Where(competencia => competencia.NivelId == TODOS_NIVEIS)
                .ToList();

            foreach (var item in competencias)
            {
                if (mapeamentoCompetencia.TryGetValue(item.Tipo.ToUpper(), out var nivel))
                {
                    var niveis = _colaboradorContext.tb_nivel.Where(c => c.tb_item_perfil_id == nivel).Select(c => c.id).ToList();
                    foreach (var n in niveis)
                    {
                        param.Add(new FiltroCompetenciaDTO()
                        {
                            Id = item.Id,
                            Tipo = item.Tipo,
                            NivelId = n
                        });
                    }

                    param.Remove(item);
                }
            }
        }

        private static double CalcularAderencia(tb_colaborador colaborador, List<FiltroCompetenciaDTO> competencias, ColaboradorContext context)
        {
            int quantidadeTotalCompetencias = competencias.Count;
            double totalAderencia = competencias
            .Select(competencia =>
            {
                var hardskill = colaborador.tb_colaborador_competencia.FirstOrDefault(h => h.competencia_id == competencia.Id && h.ativo == 1);
                var hardskill_realizacao = context.tb_colaborador_alocado_skill
                        .FirstOrDefault(x => x.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == colaborador.codigo_interno_colaborador
                            && x.skill_id == competencia.Id && x.tb_item_perfil_id == ITEM_PERFIL_HARDSKILL
                            && x.tb_colaborador_periodo_alocacao.ativo == 1 && x.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1);
                var sofskill = colaborador.tb_colaborador_softskill.FirstOrDefault(h => h.softskill_id == competencia.Id && h.ativo == 1);
                var softskill_realizacao = context.tb_colaborador_alocado_skill
                        .FirstOrDefault(x => x.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == colaborador.codigo_interno_colaborador
                            && x.skill_id == competencia.Id && x.tb_item_perfil_id == ITEM_PERFIL_SOFTSKILL
                            && x.tb_colaborador_periodo_alocacao.ativo == 1 && x.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1);
                var idioma = colaborador.tb_colaborador_idioma.FirstOrDefault(h => h.idioma_id == competencia.Id && h.ativo == 1);
                var idioma_realizacao = context.tb_colaborador_alocado_skill
                        .FirstOrDefault(x => x.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == colaborador.codigo_interno_colaborador
                            && x.skill_id == competencia.Id && x.tb_item_perfil_id == ITEM_PERFIL_IDIOMA
                            && x.tb_colaborador_periodo_alocacao.ativo == 1 && x.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1);
                var metodologia = colaborador.tb_colaborador_metodologia.FirstOrDefault(h => h.metodologia_id == competencia.Id && h.ativo == 1);
                var metodologia_realizacao = context.tb_colaborador_alocado_skill
                        .FirstOrDefault(x => x.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == colaborador.codigo_interno_colaborador
                            && x.skill_id == competencia.Id && x.tb_item_perfil_id == ITEM_PERFIL_METODOLOGIA
                            && x.tb_colaborador_periodo_alocacao.ativo == 1 && x.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1);
                var dominio = colaborador.tb_colaborador_dominionegocio.FirstOrDefault(h => h.dominionegocio_id == competencia.Id && h.ativo == 1);
                var dominio_realizacao = context.tb_colaborador_alocado_skill
                        .FirstOrDefault(x => x.tb_colaborador_periodo_alocacao.codigo_interno_colaborador == colaborador.codigo_interno_colaborador
                            && x.skill_id == competencia.Id && x.tb_item_perfil_id == ITEM_PERFIL_DOMINIO
                            && x.tb_colaborador_periodo_alocacao.ativo == 1 && x.tb_colaborador_periodo_alocacao.retroalimenta_cv == 1);
                try
                {
                    if ((competencia.Tipo.ToUpper() == HARDSKILL && hardskill != null) ||
                    (competencia.Tipo.ToUpper() == HARDSKILL && hardskill_realizacao != null) ||
                    (competencia.Tipo.ToUpper() == SOFTSKILL && sofskill != null) ||
                    (competencia.Tipo.ToUpper() == SOFTSKILL && softskill_realizacao != null) ||
                    (competencia.Tipo.ToUpper() == IDIOMA && idioma != null) ||
                    (competencia.Tipo.ToUpper() == IDIOMA && idioma_realizacao != null) ||
                    (competencia.Tipo.ToUpper() == METODOLOGIA && metodologia != null) ||
                    (competencia.Tipo.ToUpper() == METODOLOGIA && metodologia_realizacao != null) ||
                    (competencia.Tipo.ToUpper() == DOMINIO && dominio != null) ||
                    (competencia.Tipo.ToUpper() == DOMINIO && dominio_realizacao != null))
                    {
                        double peso = 1.0 / quantidadeTotalCompetencias;

                        if (competencia == null || competencia.NivelId == TODOS_NIVEIS ||
                            (hardskill != null && hardskill.tb_nivel_id == competencia.NivelId) ||
                            (hardskill_realizacao != null && hardskill_realizacao.tb_nivel_id == competencia.NivelId) ||
                            (sofskill != null && sofskill.tb_nivel_id == competencia.NivelId) ||
                            (softskill_realizacao != null && softskill_realizacao.tb_nivel_id == competencia.NivelId) ||
                            (idioma != null && idioma.tb_nivel_id == competencia.NivelId) ||
                            (idioma_realizacao != null && idioma_realizacao.tb_nivel_id == competencia.NivelId) ||
                            (metodologia != null && metodologia.tb_nivel_id == competencia.NivelId) ||
                            (metodologia_realizacao != null && metodologia_realizacao.tb_nivel_id == competencia.NivelId) ||
                            (dominio != null && dominio.tb_nivel_id == competencia.NivelId) ||
                            (dominio_realizacao != null && dominio_realizacao.tb_nivel_id == competencia.NivelId)
                            )

                        {
                            // Peso sem alteração
                        }
                        else
                        {
                            peso /= MEIO_PESO;
                        }

                        return peso;
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

                return 0.0;
            })
            .Sum();
            double aderenciaPercentual = Math.Round(totalAderencia * 100.0, 2);
            return aderenciaPercentual;
        }

        public async Task<ItensSumarioResult> ListarSkillsSumario(int cursor, int limite, string? descricao)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    id AS Id,
                    descricao AS Descricao,
                    CASE 
                        WHEN tipo_id = 1  THEN 'HARDSKILL'
                        WHEN tipo_id = 3  THEN 'METODOLOGIA'
                        WHEN tipo_id = 4  THEN 'DOMINIO'
                        WHEN tipo_id = 8  THEN 'SOFTSKILL'
                        WHEN tipo_id = 9  THEN 'IDIOMA'
                        WHEN tipo_id = 14 THEN 'DESCONHECIDO'
                        ELSE 'OUTRO'
                    END AS Tipo
                FROM vw_skills
                WHERE
                    (@Descricao IS NULL OR @Descricao = '' OR descricao LIKE CONCAT('%', @Descricao, '%'))
                    AND tipo_id <> 14
                ORDER BY 
                    confirmada DESC,   -- TRUE primeiro, depois FALSE
                    descricao ASC      -- dentro de cada grupo, A-Z
                LIMIT @Cursor, @Limite; 
            ";


            var parametros = new
            {
                Cursor = cursor,
                Limite = limite,
                Descricao = descricao
            };
            
            var result = await connection.QueryAsync<SkillSumarioDTO>(query, parametros);

            var queryNivies = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao,
                    CASE 
                        WHEN tb_item_perfil_id = 1 THEN @HardSkill
                        WHEN tb_item_perfil_id = 3 THEN @Metodologia
                        WHEN tb_item_perfil_id = 4 THEN @Dominio
                        WHEN tb_item_perfil_id = 8 THEN @Softskill
                        WHEN tb_item_perfil_id = 9 THEN @Idioma
                        WHEN tb_item_perfil_id = 14 THEN 'DESCONHECIDO'
                        ELSE 'OUTRO'
                    END AS Tipo
                    FROM tb_nivel
            ";

            var resultNiveis = await connection.QueryAsync<SkillSumarioDTO>(queryNivies, new
            {
                HardSkill = NIVEL_HARDSKILL,
                Metodologia = NIVEL_METODOLOGIA,
                Dominio = NIVEL_DOMINIO,
                Softskill = NIVEL_SOFTSKILL,
                Idioma = NIVEL_IDIOMA,
            });
            
            var ret = new ItensSumarioResult();
            ret.skills = result.ToList();
            ret.niveis = new();
            mockNivelSkillsTodos(ret);
            
            ret.niveis.AddRange(resultNiveis.ToList());
            

            return ret;
        }

        public List<SkillSumarioDTO> ListarNiveisSumario()
        {
            var ret = new ItensSumarioResult();
            ret.niveis = new List<SkillSumarioDTO>();

            mockNivelSkillsTodos(ret);

            AddNiveisRange(ret, NIVEL_HARDSKILL, ITEM_PERFIL_HARDSKILL);
            AddNiveisRange(ret, NIVEL_SOFTSKILL, ITEM_PERFIL_SOFTSKILL);
            AddNiveisRange(ret, NIVEL_IDIOMA, ITEM_PERFIL_IDIOMA);
            AddNiveisRange(ret, NIVEL_METODOLOGIA, ITEM_PERFIL_METODOLOGIA);
            AddNiveisRange(ret, NIVEL_DOMINIO, ITEM_PERFIL_DOMINIO);

            return ret.niveis;
        }

        private static void mockNivelSkillsTodos(ItensSumarioResult ret)
        {
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_HARDSKILL
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_SOFTSKILL
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_IDIOMA
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_METODOLOGIA
            });
            ret.niveis.Add(new SkillSumarioDTO()
            {
                Descricao = "Todos",
                Id = 0,
                Tipo = NIVEL_DOMINIO
            });
        }

        private void AddRangeSkills<T>(ItensSumarioResult ret, string skillTipo, DbSet<T> tabela)
        where T : class
        {
            var ativoProperty = typeof(T).GetProperty("ativo");

            if (ativoProperty != null)
            {
                var registros = tabela.ToList();

                var registrosAtivos = registros.Where(x => Convert.ToBoolean(ativoProperty.GetValue(x))).ToList();

                ret.skills.AddRange(registrosAtivos.Select(x => new SkillSumarioDTO()
                {
                    Tipo = skillTipo,
                    Descricao = x.GetType().GetProperty("descricao")?.GetValue(x)?.ToString(),
                    Id = GetIdValue(x)
                }));
            }
        }

        private long GetIdValue<T>(T item)
        {
            object idValue = item.GetType().GetProperty("id")?.GetValue(item);

            if (idValue != null && long.TryParse(idValue.ToString(), out long parsedId))
            {
                return parsedId;
            }
            return 0;
        }

        private void AddNiveisRange(ItensSumarioResult ret, string nivelTipo, int itemPerfilId)
        {
            ret.niveis.AddRange(_colaboradorContext.tb_nivel.Where(x => x.tb_item_perfil_id == itemPerfilId).Select(x => new SkillSumarioDTO()
            {
                Tipo = nivelTipo,
                Descricao = x.descricao,
                Id = x.id
            }).ToList());
        }

        public List<CompetenciaSugeridaDTO> ListarCompetenciasSugeridas(TipoCompetenciaSRSEnum competencia)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string query = competencia switch
            {
                TipoCompetenciaSRSEnum.HardSkill or
                TipoCompetenciaSRSEnum.SoftSkill or
                TipoCompetenciaSRSEnum.Metodologia or
                TipoCompetenciaSRSEnum.Dominio or
                TipoCompetenciaSRSEnum.Idioma or
                TipoCompetenciaSRSEnum.Desconhecida => @"
                    SELECT
                        cs.CompetenciaId as idCompetencia,
                        cs.Descricao as descricaoCompetencia,
                        cs.qtdUsuariosCompetencia as qtdUsuariosCompetencia,
                        cs.DataCriacao as dataSolicitacao
                    FROM vw_competencias_sugeridas cs
                    WHERE cs.CompetenciaTipo = @CompetenciaTipo
                    ORDER BY cs.Descricao ASC",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            var parametros = new
            {
                CompetenciaTipo = competencia.ToString()
            };

            var result = _connectionDapper.Query<CompetenciaSugeridaDTO>(query, parametros);

            return result.ToList();
        }

        public async Task<List<CompetenciaConsolidadaDTO>> ListarCompetenciasConsolidadas(TipoCompetenciaSRSEnum competencia)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string query = competencia switch
            {
                TipoCompetenciaSRSEnum.HardSkill => @"
                    SELECT
                        tc.id AS idCompetencia,
                        tc.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_competencia tcc
                         WHERE tcc.competencia_id = tc.id
                         AND tcc.ativo = 1) AS qtdUsuariosCompetencia,
                        tc.ativo
                    FROM tb_competencia tc
                    WHERE tc.confirmada = 1
                    ORDER BY tc.descricao ASC",
                TipoCompetenciaSRSEnum.SoftSkill => @"
                    SELECT
                        ts.id AS idCompetencia,
                        ts.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_softskill tcs
                         WHERE tcs.softskill_id = ts.id
                         AND tcs.ativo = 1) AS qtdUsuariosCompetencia,
                        ts.ativo
                    FROM tb_softskill ts
                    WHERE ts.confirmada = 1
                    ORDER BY ts.descricao ASC",
                TipoCompetenciaSRSEnum.Metodologia => @"
                    SELECT
                        tm.id AS idCompetencia,
                        tm.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_metodologia tcm
                         WHERE tcm.metodologia_id = tm.id
                         AND tcm.ativo = 1) AS qtdUsuariosCompetencia,
                        tm.ativo
                    FROM tb_metodologia tm
                    WHERE tm.confirmada = 1
                    ORDER BY tm.descricao ASC",
                TipoCompetenciaSRSEnum.Dominio => @"
                    SELECT
                        td.id AS idCompetencia,
                        td.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_dominionegocio tcd
                         WHERE tcd.dominionegocio_id = td.id
                         AND tcd.ativo = 1) AS qtdUsuariosCompetencia,
                        td.ativo
                    FROM tb_dominionegocio td
                    WHERE td.confirmada = 1
                    ORDER BY td.descricao ASC",
                TipoCompetenciaSRSEnum.Idioma => @"
                    SELECT
                        ti.id AS idCompetencia,
                        ti.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_idioma tci
                         WHERE tci.idioma_id = ti.id
                         AND tci.ativo = 1) AS qtdUsuariosCompetencia,
                        ti.ativo
                    FROM tb_idioma ti
                    WHERE ti.confirmada = 1
                    ORDER BY ti.descricao ASC",
                TipoCompetenciaSRSEnum.Desconhecida => @"
                    SELECT
                        tsd.id AS idCompetencia,
                        tsd.descricao AS descricaoCompetencia,
                        (SELECT COUNT(0)
                         FROM tb_colaborador_skill_desconhecida tci
                         WHERE tci.skill_desconhecida_id = tsd.id
                         AND tci.ativo = 1) AS qtdUsuariosCompetencia,
                        tsd.ativo
                    FROM tb_skill_desconhecida tsd
                    WHERE tsd.confirmada = 1
                    AND tsd.ativo = 1
                    ORDER BY tsd.descricao ASC",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            var result = await _connectionDapper.QueryAsync<CompetenciaConsolidadaDTO>(query);

            return result.ToList();
        }

        public async Task ReprovarCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var existeCompetencia = await ObterCompetenciaAtivaPorTipoEId(idCompetenciaASerReprovada, competenciaEnum);

            if (existeCompetencia == null)
            {
                throw new Exception("Competência não encontrada!");
            }
            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $"UPDATE {tabela} SET confirmada = 0, ativo = 0,  data_alteracao = NOW() WHERE id = @IdCompetenciaASerReprovada";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada });
        }

        public async Task AprovarCompetencia(int idCompetenciaASerAprovada, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string queryBuscaCompetencia = $"SELECT id, confirmada, descricao, usuario_criacao_id FROM {tabela} WHERE id = @IdCompetenciaASerAprovada";
            var competencia = await _connectionDapper.QueryFirstOrDefaultAsync<dynamic>(queryBuscaCompetencia, new { IdCompetenciaASerAprovada = idCompetenciaASerAprovada });

            if (competencia != null)
            {
                bool confirmada = (competencia.confirmada == 1);
                string descricao = competencia.descricao;
                long? usuarioCriacaoId = competencia.usuario_criacao_id;

                if (confirmada)
                {
                    throw new Exception("Competência já aprovada");
                }

                string queryUpdate = $"UPDATE {tabela} SET confirmada = 1 WHERE id = @IdCompetenciaASerAprovada";
                int rowsAffected = await _connectionDapper.ExecuteAsync(queryUpdate, new { IdCompetenciaASerAprovada = idCompetenciaASerAprovada });

                if (rowsAffected <= 0)
                {
                    throw new Exception("Competência não encontrada!");
                }

            }
            else
            {
                throw new Exception("Nenhuma competência encontrada com o ID especificado.");
            }
        }

        public async Task ReprovarAssociacaoCompetenciaColaborador(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaColaboradorCompetencia(competenciaEnum);
            string campo = ObterNomeCampoChaveCompetenciaColaborador(competenciaEnum);
            
            string query = $"DELETE FROM {tabela} WHERE {campo} = @IdCompetenciaASerReprovada";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada });
           
        }

        public async Task ReprovarAssociacaoCompetenciarGestorExternoPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            string query = $"DELETE FROM tb_gestor_externo_perfil_skill WHERE skill_id = @IdCompetenciaASerReprovada AND tb_item_perfil_id = @TipoCompetencia ";
             await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
            
        }
        
        public async Task ReprovarAssociacaoCompetenciarAlocado(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            string query = $"DELETE FROM tb_colaborador_alocado_skill WHERE skill_id = @IdCompetenciaASerReprovada AND tb_item_perfil_id = @TipoCompetencia ";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }
        
        public async Task ReprovarAssociacaoCompetenciarPerfil(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            
            string query = $"DELETE FROM tb_perfil_skill WHERE skill_id = @IdCompetenciaASerReprovada AND tb_item_perfil_id = @TipoCompetencia ";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }
        
        public async Task ReprovarAssociacaoCompetenciarVaga(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            
            string query = $"DELETE FROM tb_skill_vaga WHERE skill_id = @IdCompetenciaASerReprovada AND tipo_skill_id = @TipoCompetencia ";
             await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }
        
        public async Task ReprovarAssociacaoCompetenciarVagaFourmakers(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            
            string query = $"DELETE FROM tb_vaga_skill WHERE skill_id = @IdCompetenciaASerReprovada AND tb_item_perfil_id = @TipoCompetencia ";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }
        
        public async Task ReprovarAssociacaoCompetenciarVagaSRS(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            
            string query = $"DELETE FROM tb_skill_vaga_srs WHERE descricao_id = @IdCompetenciaASerReprovada AND categoria_id = @TipoCompetencia ";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }
        
        public async Task ReprovarAssociacaoCompetenciarVagaCandidato(int idCompetenciaASerReprovada, ItemPerfilEnum itemPerfilEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();
            
            
            string query = $"DELETE FROM tb_skill_candidato_srs WHERE descricao_id = @IdCompetenciaASerReprovada AND categoria_id = @TipoCompetencia ";
            await _connectionDapper.ExecuteAsync(query, new { IdCompetenciaASerReprovada = idCompetenciaASerReprovada, TipoCompetencia = itemPerfilEnum });
        }

        public async Task InsereLogReprovacaoCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum competenciaEnum, string cpfUsrLogado)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $@"INSERT INTO tb_historico_competencia (
                                          id,
                                          tipo_competencia_enum,
                                          descricao_competencia,
                                          situacao,
                                          observacao,
                                          data_alteracao,
                                          codigo_interno_colaborador_criacao,
                                          codigo_interno_colaborador_alteracao
                                      )
                                      SELECT
                                          UUID(),
                                          {competenciaEnum.GetHashCode()},
                                          tb.descricao,
                                          3,
                                          (SELECT CONCAT('A competência ', tb.descricao, ' foi reprovada.')),
                                          NOW(),
                                          (SELECT codigo_interno_colaborador FROM tb_usuario WHERE id = tb.usuario_criacao_id),
                                          ""{cpfUsrLogado}""
                                      FROM
                                          {tabela} tb
                                      WHERE
                                          tb.id = @Id;";

            var linhaInserida = await _connectionDapper.ExecuteAsync(query, new
            {
                Id = idCompetenciaASerReprovada
            });
        }

        public async Task<int> ObterQuantidadeDeSkillsAtivas(List<int> ids, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $@"SELECT
                                        COUNT(*)
                                    FROM
                                        {tabela} tb
                                    WHERE
                                        id in @Ids
                                    and ativo = 1;";

            int rowsAffected = await _connectionDapper.ExecuteScalarAsync<int>(query, new
            {
                Ids = ids
            });
            return rowsAffected;
        }

        public async Task<int> ObterQuantidadeDeSkillSugerida(int idCompetencia, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $@"SELECT
                                  COUNT(*)
                              FROM
                                  {tabela}
                              WHERE
                                  id = @Id
                              and confirmada = 0;";

            int rowsAffected = await _connectionDapper.ExecuteScalarAsync<int>(query, new
            {
                Id = idCompetencia
            });
            return rowsAffected;
        }

        public async Task<int> ObterQuantidadeDeSkillConsolidada(int id, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $@"SELECT
                                        COUNT(*)
                                    FROM
                                        {tabela}
                                    WHERE
                                        id = @Id
                                    and confirmada = 1;";

            int rowsAffected = await _connectionDapper.ExecuteScalarAsync<int>(query, new
            {
                Id = id
            });
            return rowsAffected;
        }

        public async Task<int> AtualizarCompetenciaColaboradorDeSugeridaParaConsolidada(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaColaboradorCompetencia(competenciaEnum);
            string campoChave = ObterNomeCampoChaveCompetenciaColaborador(competenciaEnum);

            string query = $@"UPDATE
                                {tabela}
                             SET
                               {campoChave} = @IdCompetenciaConsolidada,
                               data_alteracao = NOW()
                             WHERE
                               {campoChave} = @IdCompetenciaSugerida";

            var result = await _connectionDapper.ExecuteAsync(query, new
            {
                IdCompetenciaConsolidada = idCompetenciaConsolidada,
                IdCompetenciaSugerida = idCompetenciaSugerida
            });

            return result;
        }

        public async Task AtualizarCompetenciasColaboradorCuradoria(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipo)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaColaboradorCompetencia(tipo);
            string campoChave = ObterNomeCampoChaveCompetenciaColaborador(tipo);

            // 1. Buscar colaboradores com conflito (já têm a competência)
            string sqlColaboradoresConflitantes = $@"
                SELECT codigo_interno_colaborador
                FROM {tabela}
                WHERE {campoChave} IN (@IdCompetenciaSugerida, @IdCompetenciaConsolidada)
                GROUP BY codigo_interno_colaborador
                HAVING COUNT(*) > 1";

            var parametrosConflito = new
            {
                IdCompetenciaSugerida = idCompetenciaSugerida,
                IdCompetenciaConsolidada = idCompetenciaConsolidada
            };

            var colaboradoresConflitantes = (await _connectionDapper.QueryAsync<int>(
                sqlColaboradoresConflitantes, parametrosConflito)).ToList();

            // 2. Deletar registros com a competência sugerida onde já existe a consolidada
            string sqlDeleteConflitantes = $@"
                DELETE FROM {tabela}
                WHERE codigo_interno_colaborador = @Codigo
                  AND {campoChave} = @IdCompetenciaSugerida";

            foreach (var colaborador in colaboradoresConflitantes)
            {
                var parametrosDelete = new
                {
                    Codigo = colaborador,
                    IdCompetenciaSugerida = idCompetenciaSugerida
                };

                await _connectionDapper.ExecuteAsync(sqlDeleteConflitantes, parametrosDelete);
            }

            // 3. Atualizar registros restantes da sugerida para consolidada
            string sqlUpdate = $@"
                UPDATE {tabela}
                SET {campoChave} = @IdCompetenciaConsolidada,
                    data_alteracao = NOW()
                WHERE {campoChave} = @IdCompetenciaSugerida";

            // Se houver colaboradores conflitantes, adiciona filtro para evitar update neles
            if (colaboradoresConflitantes.Any())
            {
                sqlUpdate += @"
                    AND codigo_interno_colaborador NOT IN @Conflitantes";
            }

            var parametrosUpdate = new
            {
                IdCompetenciaSugerida = idCompetenciaSugerida,
                IdCompetenciaConsolidada = idCompetenciaConsolidada,
                Conflitantes = colaboradoresConflitantes
            };

            await _connectionDapper.ExecuteAsync(sqlUpdate, parametrosUpdate);
        }

        public async Task DesativarCompetencia(int idCompetencia, TipoCompetenciaSRSEnum competenciaEnum)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            // Busca a descrição atual
            string descricaoAtual = await _connectionDapper.QueryFirstOrDefaultAsync<string>(
                $"SELECT descricao FROM {tabela} WHERE id = @IdCompetencia",
                new { IdCompetencia = idCompetencia });

            if (descricaoAtual == null)
                throw new Exception("Competência não encontrada!");

            string novaDescricaoBase = descricaoAtual.EndsWith("_INATIVO")
                ? descricaoAtual
                : $"{descricaoAtual}_INATIVO";

            string novaDescricao = novaDescricaoBase;
            int contador = 1;

            // Enquanto já existir uma com essa descrição, incrementa sufixo
            while (await _connectionDapper.ExecuteScalarAsync<int>(
                       $"SELECT COUNT(*) FROM {tabela} WHERE descricao = @Descricao",
                       new { Descricao = novaDescricao }) > 0)
            {
                novaDescricao = $"{novaDescricaoBase}_{contador}";
                contador++;
            }

            // Faz o update seguro
            string query = $@"
            UPDATE {tabela}
            SET ativo = 0,
                descricao = @NovaDescricao
            WHERE id = @IdCompetencia";

            int rowsAffected = await _connectionDapper.ExecuteAsync(query, new
            {
                IdCompetencia = idCompetencia,
                NovaDescricao = novaDescricao
            });

            if (rowsAffected <= 0)
                throw new Exception("Não foi possível unificar as competências. Competência não encontrada!");
        }

        public async Task GravaLogUnificacao(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum competenciaEnum, string cpfUsrLogado)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(competenciaEnum);

            string query = $@"INSERT INTO tb_historico_competencia (
                                          id,
                                          tipo_competencia_enum,
                                          descricao_competencia,
                                          situacao,
                                          observacao,
                                          data_alteracao,
                                          codigo_interno_colaborador_criacao,
                                          codigo_interno_colaborador_alteracao
                                      )
                                      SELECT
                                          UUID(),
                                          @CompetenciaEnum,
                                          tb.descricao,
                                          1,
                                          (SELECT CONCAT('A competência ', tb.descricao, ' foi unificada com ', (SELECT descricao FROM {tabela} WHERE id = @IdConsolidada))),
                                          NOW(),
                                          (SELECT codigo_interno_colaborador FROM tb_usuario WHERE id = tb.usuario_criacao_id),
                                          @CpfAlteracao
                                      FROM
                                          {tabela} tb
                                      WHERE
                                          tb.id = @IdSugerida;";

            var linhaInserida = await _connectionDapper.ExecuteAsync(query, new
            {
                IdSugerida = idCompetenciaSugerida,
                IdConsolidada = idCompetenciaConsolidada,
                CpfAlteracao = cpfUsrLogado,
                CompetenciaEnum = competenciaEnum.GetHashCode()
            });
        }

        public async Task<CompetenciaDTO> ObterCompetenciaAtivaPorTipoEId(int idCompetencia, TipoCompetenciaSRSEnum competencia)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var tabela = ObterNomeTabelaCompetencia(competencia);

            string query = $@"
                                        SELECT
                                            tc.id,
                                            tc.descricao,
                                            tc.usuario_criacao_id AS UsuarioCriacaoId,
                                            tc.confirmada,
                                            tu.codigo_interno_colaborador AS CpfUsuarioCriacao
                                        FROM
                                            {tabela} tc
                                        LEFT JOIN tb_usuario tu ON tu.id = tc.usuario_criacao_id
                                        WHERE
                                            tc.id = @IdCompetencia AND tc.ativo = 1";

            var result = await _connectionDapper.QuerySingleOrDefaultAsync<dynamic>(query, new { IdCompetencia = idCompetencia });

            if (result != null)
            {
                return new CompetenciaDTO
                {
                    Id = result.id,
                    Descricao = result.descricao,
                    UsuarioCriacaoId = result.UsuarioCriacaoId,
                    CpfUsuarioCriacao = result.CpfUsuarioCriacao,
                    Pendente = result.confirmada != 1
                };
            }

            return null;
        }
        
        public async Task<CompetenciaDTO> ObterCompetenciaAtivaPorTipoENome(string nomeCompetencia, TipoCompetenciaSRSEnum competencia)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var tabela = ObterNomeTabelaCompetencia(competencia);

            string query = $@"
                                        SELECT
                                            tc.id,
                                            tc.descricao,
                                            tc.usuario_criacao_id,
                                            tc.confirmada
                                        FROM
                                            {tabela} tc
                                        WHERE
                                            tc.descricao = @Descricao 
                                            AND tc.ativo = 1";

            var result = await _connectionDapper.QuerySingleOrDefaultAsync<dynamic>(query, new { Descricao = nomeCompetencia });

            if (result != null)
            {
                return new CompetenciaDTO
                {
                    Id = result.id,
                    Descricao = result.descricao,
                    UsuarioCriacaoId = result.usuario_criacao_id,
                    Pendente = result.confirmada != 1
                };
            }

            return null;
        }

        private string ObterNomeTabelaColaboradorCompetencia(TipoCompetenciaSRSEnum competenciaEnum)
        {
            return competenciaEnum switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "tb_colaborador_competencia",
                TipoCompetenciaSRSEnum.SoftSkill => "tb_colaborador_softskill",
                TipoCompetenciaSRSEnum.Metodologia => "tb_colaborador_metodologia",
                TipoCompetenciaSRSEnum.Dominio => "tb_colaborador_dominionegocio",
                TipoCompetenciaSRSEnum.Idioma => "tb_colaborador_idioma",
                _ => throw new Exception("Tipo de competência inválido.")
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

        private string ObterNomeCampoChaveCompetenciaColaborador(TipoCompetenciaSRSEnum competenciaEnum)
        {
            return competenciaEnum switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "competencia_id",
                TipoCompetenciaSRSEnum.SoftSkill => "softskill_id",
                TipoCompetenciaSRSEnum.Metodologia => "metodologia_id",
                TipoCompetenciaSRSEnum.Dominio => "dominionegocio_id",
                TipoCompetenciaSRSEnum.Idioma => "idioma_id",
                _ => throw new Exception("Tipo de competência inválido.")
            };
        }

        public async Task<List<LogCompetenciaDTO>> ListarLogCompetencias(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string query = $@"
                                    SELECT
	                                    descricao_competencia AS DescricaoCompetencia,
	                                    situacao,
	                                    observacao,
	                                    data_alteracao AS DataAlteracao
                                    FROM
	                                    tb_historico_competencia
                                    WHERE
                                        tipo_competencia_enum = @TipoCompetenciaEnum
                                    ORDER BY data_alteracao DESC";

            var registros = await _connectionDapper.QueryAsync<LogCompetenciaDTO>(query, new { TipoCompetenciaEnum = enumTipoCompetencia });

            return registros.ToList();
        }

        public async Task<EditarCompetenciaDTO> EditarCompetencia(EditarCompetenciaParam param, bool pulaValidacaoDuplicidade = false)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string tabela = ObterNomeTabelaCompetencia(param.TipoCompetencia);

            dynamic competenciaAtual = await BuscaCompetenciaAtual(param, _connectionDapper, tabela, pulaValidacaoDuplicidade);

            bool ativo = competenciaAtual.ativo == 1;
            string descricao = competenciaAtual.descricao;
            long? usuarioCriacaoId = competenciaAtual.usuario_criacao_id;
            bool pendente = competenciaAtual.confirmada == 0;
            param.Descricao = param.Descricao;

            string queryUpdate = $"UPDATE {tabela} SET ativo = @Ativo, descricao = @Descricao WHERE id = @Id";
            int rowsAffected = await _connectionDapper.ExecuteAsync(queryUpdate, new { param.Id, param.Descricao, param.Ativo });

            if (rowsAffected <= 0)
            {
                throw new Exception("Competência não encontrada!");
            }

            var ret = new EditarCompetenciaDTO()
            {
                Ativo = param.Ativo,
                Descricao = param.Descricao,
                Id = param.Id,
                Pendente = pendente
            };

            return ret;
        }

        private static async Task<dynamic> BuscaCompetenciaAtual(EditarCompetenciaParam param, MySqlConnection _connection, string tabela, bool pulaValidacaoDeDuplicidade = false)
        {
            string queryBuscaCompetenciaAtual = @$"
                                    SELECT
                                        id,
                                        confirmada,
                                        ativo,
                                        descricao,
                                        usuario_criacao_id,
                                        confirmada
                                    FROM
                                        {tabela}
                                    WHERE
                                        id = @Id";
            var competenciaAtual = await _connection.QueryFirstOrDefaultAsync<dynamic>(queryBuscaCompetenciaAtual, new { param.Id })
            ?? throw new KeyNotFoundException("Nenhuma competência encontrada com o ID especificado.");

            string queryBuscaCompetencia = @$"
                                    SELECT
                                        id,
                                        descricao,
                                        ativo
                                    FROM
                                        {tabela}
                                    WHERE
                                        descricao = @Descricao
                                        AND ativo = 1";
            if (!pulaValidacaoDeDuplicidade)
            {
                var competencia = await _connection.QueryAsync<CompetenciaDTO>(queryBuscaCompetencia, new { param.Descricao });

                foreach (var item in competencia)
                {
                    if (item != null && item.Id != param.Id)
                    {
                        throw new Exception("Já há uma skill com este nome");
                    }
                }
            }

            return competenciaAtual;
        }

        public async Task<AdicionarCompetenciaDTO> AdicionarCompetenciaCvGestaoDeSkills(string descricao, TipoCompetenciaSRSEnum competencia, string cpf, int orgId)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var tabela = ObterNomeTabelaCompetencia(competencia);

            string queryUsuario = "SELECT id FROM tb_usuario WHERE codigo_interno_colaborador = @Cpf AND tb_org_id = @OrgId;";
            var usuarioId = await _connectionDapper.QuerySingleOrDefaultAsync<int>(queryUsuario, new { Cpf = cpf, OrgId = orgId });

            if (usuarioId.IsNull())
            {
                throw new Exception("Usuário não encontrado.");
            }
            int id = 0;
            try
            {
                descricao = descricao.ToUpper();
                string query = $@"INSERT INTO {tabela} (descricao, ativo, confirmada, usuario_criacao_id) VALUES (@Descricao, 1, 1, @UsuarioId);
                            SELECT LAST_INSERT_ID();";

                id = await _connectionDapper.ExecuteScalarAsync<int>(query, new { Descricao = descricao, UsuarioId = usuarioId });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar competência.", ex);
            }

            return new AdicionarCompetenciaDTO()
            {
                Ativo = true,
                Pendente = false,
                DescricaoCompetencia = descricao,
                IdCompetencia = id
            };
        }

        public async Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            var tabela = ObterNomeTabelaCompetencia(competencia);

            string queryUsuario = "SELECT id FROM tb_usuario WHERE codigo_interno_colaborador = @Cpf;";
            var usuarioId = await _connectionDapper.QuerySingleOrDefaultAsync<int>(queryUsuario, new { Cpf = cpf });

            if (usuarioId.IsNull())
            {
                throw new Exception("Usuário não encontrado.");
            }

            var transaction = _connectionDapper.BeginTransaction();
            int id = 0;

            try
            {
                descricao = descricao.ToUpper();
                string query = $@"INSERT INTO {tabela} (descricao, ativo, confirmada, usuario_criacao_id) VALUES (@Descricao, 1, 1, @UsuarioId);
                            SELECT LAST_INSERT_ID();";

                id = await _connectionDapper.ExecuteScalarAsync<int>(query, new { Descricao = descricao, UsuarioId = usuarioId }, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Erro ao adicionar competência.", ex);
            }

            return new AdicionarCompetenciaDTO()
            {
                Ativo = true,
                Pendente = false,
                DescricaoCompetencia = descricao,
                IdCompetencia = id
            };
        }

        public async Task<List<CompetenciaSumarioDTO>> ListarCompetenciaSumario(TipoCompetenciaSRSEnum competencia, int orgId)
        {
            var _connectionDapper = _dapperConnection.GetConnection();

            string labelSkill = competencia switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "competencia",
                TipoCompetenciaSRSEnum.SoftSkill => "softskill",
                TipoCompetenciaSRSEnum.Metodologia => "metodologia",
                TipoCompetenciaSRSEnum.Dominio => "dominionegocio",
                TipoCompetenciaSRSEnum.Idioma => "idioma",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            string nomeSkill = competencia switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "hard skill",
                TipoCompetenciaSRSEnum.SoftSkill => "soft skill",
                TipoCompetenciaSRSEnum.Metodologia => "metodologia",
                TipoCompetenciaSRSEnum.Dominio => "domínio de negócio",
                TipoCompetenciaSRSEnum.Idioma => "idioma",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            string sql = @$"SELECT
                                        COUNT(tcc.codigo_interno_colaborador) AS qtd,
                                        tc.descricao,
                                        tc.id AS skill_id,
                                        CASE WHEN tn.descricao IS NULL THEN ""Não definido"" ELSE tn.descricao END AS nivel
                                    FROM
                                        tb_colaborador_{labelSkill} tcc
                                    JOIN
                                        tb_{labelSkill} tc ON tc.id = tcc.{labelSkill}_id
                                    LEFT JOIN
                                        tb_nivel tn ON tcc.tb_nivel_id = tn.id
                                    JOIN
                                        tb_colaborador_org tco ON tco.codigo_interno_colaborador = tcc.codigo_interno_colaborador
                                    WHERE
                                        tc.ativo = 1 AND tcc.ativo = 1 AND tco.ativo = 1 AND tco.tb_org_id = {orgId}
                                    GROUP BY
                                        tc.descricao, tn.descricao
                                 ;";

            string sqlColabsSemSkill = @$"SELECT
                                                      COUNT(DISTINCT tco.codigo_interno_colaborador) AS qtd
                                                  FROM
                                                      tb_colaborador_org tco
                                                  LEFT JOIN
                                                      tb_colaborador_{labelSkill} tcc ON tco.codigo_interno_colaborador = tcc.codigo_interno_colaborador
                                                  WHERE
                                                      tcc.{labelSkill}_id IS NULL AND tco.tb_org_id = {orgId} AND tco.ativo = 1;
                                               ";

            var resultdb = await _connectionDapper.QueryAsync<dynamic>(sql);
            var resultColabsSemSkill = await _connectionDapper.QueryAsync<dynamic>(sqlColabsSemSkill);

            var result = resultdb
                        .GroupBy(x => x.descricao)
                        .Select(group => new CompetenciaSumarioDTO
                        {
                            Skill = group.Key,
                            CdSkill = group.First().skill_id,
                            QtdUsuarios = group.Sum(x => (int)x.qtd),
                            QtdSenioridade = resultdb.Where(x => x.skill_id == group.First().skill_id).Select(sen => new SenioridadeDTO()
                            {
                                CdSkill = group.First().skill_id,
                                Senioridade = sen.nivel,
                                QtdUsuarios = resultdb.Where(sen2 => sen2.skill_id == group.First().skill_id && sen2.nivel == sen.nivel).Sum(x => (int)x.qtd)
                            }).ToList()
                        })
                        .OrderByDescending(x => x.QtdUsuarios)
                        .ToList();

            var objetoColabsSemSkill = new CompetenciaSumarioDTO()
            {
                Skill = $"Sem {nomeSkill}",
                CdSkill = 0,
                QtdUsuarios = (int)resultColabsSemSkill.Select(x => x.qtd).FirstOrDefault(),
                QtdSenioridade = null
            };

            result.Insert(0, objetoColabsSemSkill);

            return result;
        }

        public async Task<List<CompetenciaNomeEIdDTO>> ObterNomeSkillsPorTipo(TipoCompetenciaSRSEnum tipo)
        {
            var connection = _dapperConnection.GetConnection();
            
            string labelSkill = tipo switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "competencia",
                TipoCompetenciaSRSEnum.SoftSkill => "softskill",
                TipoCompetenciaSRSEnum.Metodologia => "metodologia",
                TipoCompetenciaSRSEnum.Dominio => "dominionegocio",
                TipoCompetenciaSRSEnum.Idioma => "idioma",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            var query = @$"
                SELECT 
                    tcc.id AS Id,
                    tcc.descricao AS Descricao
                FROM tb_{labelSkill} tcc
                WHERE tcc.ativo = 1;
            ";

            var result = await connection.QueryAsync<CompetenciaNomeEIdDTO>(query);

            return result.ToList();
        }

        public async Task<List<CompetenciaNomeEIdDTO>> ObterNomeSkillsPorTipoEIds(TipoCompetenciaSRSEnum tipo, IReadOnlyList<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<CompetenciaNomeEIdDTO>();

            string labelSkill = tipo switch
            {
                TipoCompetenciaSRSEnum.HardSkill => "competencia",
                TipoCompetenciaSRSEnum.SoftSkill => "softskill",
                TipoCompetenciaSRSEnum.Metodologia => "metodologia",
                TipoCompetenciaSRSEnum.Dominio => "dominionegocio",
                TipoCompetenciaSRSEnum.Idioma => "idioma",
                _ => throw new Exception("Tipo de competência inválido.")
            };

            var connection = _dapperConnection.GetConnection();

            var query = @$"
                SELECT
                    tcc.id AS Id,
                    tcc.descricao AS Descricao
                FROM tb_{labelSkill} tcc
                WHERE tcc.ativo = 1
                    AND tcc.id IN @Ids;
            ";

            var result = await connection.QueryAsync<CompetenciaNomeEIdDTO>(query, new { Ids = ids });

            return result.ToList();
        }

        public async Task<List<CompetenciaGroupDTO>> GetCompetenciasByTypeAndGroupId(int ItemPerfilTipoID, List<long> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<CompetenciaGroupDTO>();

            var tipoBancoHard = CompetenciaUtils.ConverterPerfilItemParaTipoCompetenciaSRS((ItemPerfilEnum)ItemPerfilTipoID); // 1 no banco            

            var connection = _dapperConnection.GetConnection();

            var query = @"SELECT
	                        v.id AS Id,
	                        v.descricao AS Descricao,
	                        @tipoBancoHard AS TipoIdSRS,
	                        v.confirmada AS Confirmada
                        FROM vw_skills v
                        WHERE
	                        v.tipo_id = @itemPerfilSRS
	                        AND v.id IN @Ids";
            var parametros = new { itemPerfilSRS = ItemPerfilTipoID, tipoBancoHard = (int)tipoBancoHard, Ids = ids };
            var result = await connection.QueryAsync<CompetenciaGroupDTO>(query, parametros);
            return result.ToList();
        }

        public async Task<List<VwSkillColaboradorDTO>> BuscarSkillsPorCodigoInternoColaborador(string codigoInternoColaborador)
        {

            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT
                    id AS Id,
                    codigo_interno_colaborador AS CodigoInternoColaborador,
                    nivel_id As NivelId,
                    nivel AS Nivel,
                    tipo AS Tipo,
                    descricao AS Descricao
                FROM vw_colaborador_skills
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                ORDER BY
                    descricao
                ASC
            ";

            var parametros = new
            {
                CodigoInternoColaborador = codigoInternoColaborador
            };

            var result = await connection.QueryAsync<VwSkillColaboradorDTO>(query, parametros);
            return result.ToList();
        }

        public async Task<SkillsLog> GravarLogsSkillsMinhaJornada(SkillsLog logSkills)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                bool validado = ValidarSkillsLog(logSkills);

                if (!validado)
                    return logSkills;

                string sql = @"
                            INSERT INTO tb_skills_log (
                                id, 
                                tb_colaborador_codigo_interno_colaborador, 
                                tb_gestor_externo_perfil_id, 
                                skill_id, 
                                tb_item_perfil_id, 
                                tb_nivel_id, 
                                tb_skills_movimentacao_id,
                                tb_colaborador_codigo_interno_colaborador_logado,
                                log_automatico
                            ) VALUES (
                                (UUID()),
                                @CodigoInternoColaborador,
                                @GestorExternoPerfil,
                                @SkillId,
                                @ItemPerfil,
                                @NivelId,
                                @SkillsMovimentacaoId,
                                @CodigoInternoColaboradorLogado,
                                @LogAutomatico
                            );";

                var parametros = new
                {
                    logSkills.CodigoInternoColaborador,
                    logSkills.GestorExternoPerfil,
                    logSkills.SkillId,
                    logSkills.ItemPerfil,
                    logSkills.NivelId,
                    logSkills.SkillsMovimentacaoId,
                    logSkills.CodigoInternoColaboradorLogado,
                    logSkills.LogAutomatico
                };

                var result = await connection.ExecuteAsync(sql, parametros);
                return logSkills;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir log de skill minha jornada. ERRO: {ex.Message}");
            }
        }

        private bool ValidarSkillsLog(SkillsLog log)
        {
            if (log == null) return false;

            if (string.IsNullOrWhiteSpace(log.CodigoInternoColaborador))
                return false;

            if (string.IsNullOrWhiteSpace(log.GestorExternoPerfil))
                return false;

            if (log.SkillId <= 0)
                return false;

            if (log.ItemPerfil == 0)
                return false;

            if (log.SkillsMovimentacaoId == 0)
                return false;

            if (!log.NivelId.HasValue || log.NivelId <= 0)
                return false;

            return true;
        }

        public CompetenciaDTO GetCompetenciaById(long id)
        {
            var row = _colaboradorContext.tb_competencia.Where(x => x.id == id).FirstOrDefault();
            if (row != null)
            {
                return new CompetenciaDTO
                {
                    Id = row.id,
                    Descricao = row.descricao,
                    Pendente = !Convert.ToBoolean(row.confirmada),
                    UsuarioCriacaoId = row.usuario_criacao_id
                };
            }
            return null;
        }

        public long GetUsuarioCriacaoIdByCpf(string cpf)
        {
            var userRow = _colaboradorContext.tb_usuario.FirstOrDefault(x => x.codigo_interno_colaborador.Equals(cpf));
            return userRow?.id ?? throw new Exception("Usuário não encontrado");
        }

        public long SaveCompetencia(string descricao, long usuarioCriacaoId)
        {
            var userRow = _colaboradorContext.tb_usuario.Find(usuarioCriacaoId);
            if (_colaboradorContext.tb_competencia
                .Where(x => x.descricao.ToUpper() == descricao.ToUpper() && x.ativo == 1).Count() > 0)
                throw new Exception("Essa competencia já existe.");

            var oldRow = _colaboradorContext.tb_competencia
                .Where(x => x.descricao.ToUpper() == descricao.ToUpper() && x.ativo == 0).FirstOrDefault();
            if (oldRow != null)
            {
                oldRow.ativo = 1;
                _colaboradorContext.tb_competencia.Update(oldRow);
                _colaboradorContext.SaveChanges();
                return oldRow.id;
            }
            else
            {
                var row = new tb_competencia();
                row.ativo = 1;
                row.descricao = descricao;
                row.usuario_criacao = userRow ?? throw new Exception("Usuário não encontrado");

                _colaboradorContext.tb_competencia.Add(row);
                _colaboradorContext.SaveChanges();
                return row.id;
            }
        }

        public List<CompetenciaDTO> ListCompetencias(string busca, int cursor, int limite)
        {
            var ret = new List<CompetenciaDTO>();
            List<tb_competencia> rows;

            if (string.IsNullOrEmpty(busca))
            {
                rows = _colaboradorContext.tb_competencia.Where(x => x.ativo == 1)
                    .OrderBy(x => x.descricao).Skip(cursor).Take(limite).ToList();
            }
            else
            {
                rows = _colaboradorContext.tb_competencia
                    .Where(x => x.ativo == 1 && EF.Functions.Like(x.descricao.ToUpper(), "%" + busca.ToUpper() + "%"))
                    .OrderBy(x => x.descricao).Skip(cursor).Take(limite).ToList();
            }

            foreach (var row in rows)
            {
                ret.Add(new CompetenciaDTO
                {
                    Id = row.id,
                    Descricao = row.descricao,
                    UsuarioCriacaoId = row.usuario_criacao_id,
                    Pendente = !Convert.ToBoolean(row.confirmada)
                });
            }
            return ret;
        }

        public CertificadoDTO GetCertificadoById(long certificadoId)
        {
            var row = _colaboradorContext.tb_certificado.Where(x => x.id == certificadoId).FirstOrDefault();
            if (row == null) return null;
            return new CertificadoDTO
            {
                IdCertificado = row.id,
                descricao = row.descricao,
                conclusao = row.data_conclusao,
                instituicao = row.instituicao,
                ativo = Convert.ToBoolean(row.ativo),
                Path = row.path,
                cargaHoraria = row.carga_horaria
            };
        }

        public void UpdateCertificado(CertificadoDTO dto)
        {
            var row = _colaboradorContext.tb_certificado
                .Where(x => x.id == dto.IdCertificado).FirstOrDefault();
            if (row == null) throw new Exception("Certificado não encontrado.");
            row.path = dto.Path;
            row.descricao = dto.descricao;
            row.instituicao = dto.instituicao;
            row.data_conclusao = dto.conclusao;
            row.carga_horaria = dto.cargaHoraria;
            row.ativo = Convert.ToSByte(dto.ativo);
            _colaboradorContext.tb_certificado.Update(row);
            _colaboradorContext.SaveChanges();
        }

        public ColaboradorCompetenciaCertificadoDTO GetColaboradorCompetenciaCertificado(long idCertificadoCompetencia)
        {
            var row = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);
            if (row == null) return null;
            return new ColaboradorCompetenciaCertificadoDTO
            {
                Id = row.id,
                Ativo = row.ativo,
                CpfTbColaboradroCompetencia = row.tb_colaborador_competencia.codigo_interno_colaborador,
                Principal = row.principal
            };
        }

        public ColaboradorCompetenciaCertificadoDTO GetColaboradorCertificadoByCertificadoId(long idCertificadoCompetencia)
        {
            var row = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.tb_certificado_id == idCertificadoCompetencia).FirstOrDefault();
            if (row == null) return null;
            return new ColaboradorCompetenciaCertificadoDTO
            {
                Id = row.id,
                Ativo = row.ativo,
                CpfTbColaboradroCompetencia = row.tb_colaborador_competencia.codigo_interno_colaborador,
                Principal = row.principal
            };
        }

        public void UpdateColaboradorCompetenciaCertificado(long id, sbyte ativo, sbyte principal)
        {
            var row = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.id == id).FirstOrDefault();
            if (row == null) throw new Exception("Registro não encontrado.");
            row.ativo = ativo;
            row.principal = principal;
            _colaboradorContext.tb_colaborador_competencia_certificado.Update(row);
            _colaboradorContext.SaveChanges();
        }

        public ColaboradorCompetenciaCertificadoDTO BuscarColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia)
        {
            var certificadoColabRow = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);
            if (certificadoColabRow == null) throw new Exception("Certificado não encontrado.");

            var antigoPrincipal = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.id != certificadoColabRow.id
                    && x.tb_colaborador_competencia_id == certificadoColabRow.tb_colaborador_competencia_id
                    && x.ativo == 1
                    && x.principal == 1)
                .FirstOrDefault();

            if (antigoPrincipal != null)
            {
                return new ColaboradorCompetenciaCertificadoDTO
                {
                    Id = antigoPrincipal.id,
                    Ativo = antigoPrincipal.ativo,
                    CpfTbColaboradroCompetencia = antigoPrincipal.tb_colaborador_competencia.codigo_interno_colaborador,
                    Principal = antigoPrincipal.principal
                };
            }
            throw new Exception("Colaborador não possui ainda um certificado principal para alterá-lo.");
        }

        public ColaboradorCompetenciaCertificadoDTO BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(long idCertificadoCompetencia)
        {
            var certificadoCompetenciaRow = _colaboradorContext.tb_colaborador_competencia_certificado.Find(idCertificadoCompetencia);
            if (certificadoCompetenciaRow == null) return null;

            var penultimo = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.id != certificadoCompetenciaRow.id
                    && x.tb_colaborador_competencia_id == certificadoCompetenciaRow.tb_colaborador_competencia_id
                    && x.ativo == 1)
                .FirstOrDefault();

            if (penultimo != null)
            {
                return new ColaboradorCompetenciaCertificadoDTO
                {
                    Id = penultimo.id,
                    Ativo = penultimo.ativo,
                    CpfTbColaboradroCompetencia = penultimo.tb_colaborador_competencia.codigo_interno_colaborador,
                    Principal = penultimo.principal
                };
            }
            return null;
        }

        public long SaveCertificado(CertificadoDTO certificado, string codigoInternoColaborador, long? competenciaColaboradorId)
        {
            var certificadoRow = new tb_certificado();
            certificadoRow.ativo = 1;
            certificadoRow.path = certificado.Path;
            certificadoRow.descricao = certificado.descricao;
            certificadoRow.instituicao = certificado.instituicao;
            certificadoRow.data_conclusao = certificado.conclusao;
            certificadoRow.carga_horaria = certificado.cargaHoraria;
            certificadoRow.codigo_interno_colaborador = codigoInternoColaborador;

            _colaboradorContext.tb_certificado.Add(certificadoRow);
            _colaboradorContext.SaveChanges();

            if (competenciaColaboradorId != null)
            {
                var certificadoColabRow = new tb_colaborador_competencia_certificado();
                certificadoColabRow.ativo = 1;
                certificadoColabRow.tb_colaborador_competencia_id = competenciaColaboradorId;
                certificadoColabRow.tb_certificado = certificadoRow;
                if (_colaboradorContext.tb_colaborador_competencia_certificado
                    .Where(x => x.tb_colaborador_competencia_id == competenciaColaboradorId && x.ativo == 1).Count() == 0)
                {
                    certificadoColabRow.principal = 1;
                }
                _colaboradorContext.tb_colaborador_competencia_certificado.Add(certificadoColabRow);
                _colaboradorContext.SaveChanges();
            }

            var pathFile = certificado.Path;
            if (!string.IsNullOrEmpty(pathFile))
            {
                if (pathFile.Contains(".png"))
                    certificadoRow.path = pathFile.Replace(".png", "/" + certificadoRow.id.ToString() + ".png");
                else
                    certificadoRow.path = pathFile.Replace(".pdf", "/" + certificadoRow.id.ToString() + ".pdf");
                _colaboradorContext.SaveChanges();
            }

            certificado.Path = certificadoRow.path;
            certificado.IdCertificado = certificadoRow.id;
            return certificadoRow.id;
        }

        public long SaveCompetenciaColaborador(long competenciaId, long? nivelId, string cpf)
        {
            if (_colaboradorContext.tb_colaborador_competencia
                .Any(x => x.competencia_id == competenciaId && x.codigo_interno_colaborador == cpf && x.ativo == 1))
                throw new Exception("Colaborador já possui essa competência.");

            var row = new tb_colaborador_competencia
            {
                competencia_id = competenciaId,
                tb_nivel_id = nivelId,
                codigo_interno_colaborador = cpf,
                ativo = 1,
                data_alteracao = DateTime.Now
            };
            _colaboradorContext.tb_colaborador_competencia.Add(row);
            _colaboradorContext.SaveChanges();
            return row.id;
        }

        public void RemoveCompetenciaColaborador(string cpf, long competenciaId)
        {
            var row = _colaboradorContext.tb_colaborador_competencia
                .Where(x => x.codigo_interno_colaborador == cpf && x.competencia_id == competenciaId && x.ativo == 1)
                .FirstOrDefault();
            if (row != null)
            {
                row.ativo = 0;
                _colaboradorContext.SaveChanges();
            }
        }

        public void AtualizaCompetenciaColaborador(string cpf, long competenciaId, long? nivelId)
        {
            var row = _colaboradorContext.tb_colaborador_competencia
                .Where(x => x.codigo_interno_colaborador == cpf && x.competencia_id == competenciaId && x.ativo == 1)
                .FirstOrDefault();
            if (row != null)
            {
                row.tb_nivel_id = nivelId;
                row.data_alteracao = DateTime.Now;
                _colaboradorContext.SaveChanges();
            }
        }

        public List<long> ListarIdsPorCompetenciaId(long id)
        {
            return _colaboradorContext.tb_colaborador_competencia
                .Where(x => x.competencia_id == id && x.ativo == 1)
                .Select(x => x.id)
                .ToList();
        }

        public NivelDTO GetNivelById(long? id)
        {
            if (id == null) return null;
            var row = _colaboradorContext.tb_nivel.Where(x => x.id == id).FirstOrDefault();
            if (row == null) return null;
            return new NivelDTO { Id = row.id, Descricao = row.descricao };
        }

        public List<NivelDTO> ListNiveisCompetencia()
        {
            var itemPerfilId = _colaboradorContext.tb_item_perfil
                .Where(x => x.descricao == "COMPETENCIA")
                .Select(x => x.id)
                .FirstOrDefault();
            if (itemPerfilId == 0) return new List<NivelDTO>();

            return _colaboradorContext.tb_nivel
                .Where(x => x.tb_item_perfil_id == itemPerfilId)
                .OrderBy(x => x.id)
                .Select(n => new NivelDTO { Id = n.id, Descricao = n.descricao })
                .ToList();
        }

        public StatusEndossoDTO GetEndossoByCompetenciaColaboradorId(long idCompetenciaColaborador)
        {
            var rowsEndosso = _colaboradorContext.tb_endosso_competencia
                .Where(x => x.ativo == 1 && x.colaborador_competencia_id == idCompetenciaColaborador)
                .ToList();

            var dto = new StatusEndossoDTO();
            if (rowsEndosso == null || rowsEndosso.Count == 0) return dto;

            var statusFinalizado = rowsEndosso
                .Where(x => x.tb_status_endosso_id == 2)
                .OrderByDescending(x => x.data_alteracao)
                .FirstOrDefault();

            dto.Endossado = statusFinalizado != null;
            dto.QuantidadeSolicitacaoEndosso = rowsEndosso.Count;
            dto.QuantidadeEndosso = rowsEndosso.Count(x => x.tb_status_endosso_id == 2);

            var finalizados = rowsEndosso.Where(x => x.tb_status_endosso_id == 2).ToList();
            if (finalizados.Count > 0)
                dto.NivelEndosso = finalizados.Average(x => x.tb_tipo_endosso.nivel);
            else
                dto.NivelEndosso = 0;

            return dto;
        }

        public CertificadoDTO GetCertificadoByIdRelacao(long? idCertificado)
        {
            if (idCertificado == null) return null;
            var relacao = _colaboradorContext.tb_colaborador_competencia_certificado
                .Where(x => x.tb_certificado_id == idCertificado && x.ativo == 1)
                .FirstOrDefault();
            if (relacao?.tb_certificado == null) return null;
            var cert = relacao.tb_certificado;
            var path = string.IsNullOrWhiteSpace(cert.path) ? null : cert.path;
            string thumb = null;

            if (!string.IsNullOrEmpty(path) && path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                thumb = path.Substring(0, path.Length - 4) + "_thumb.pdf";
            }

            return new CertificadoDTO
            {
                IdCertificado = cert.id,
                descricao = cert.descricao,
                conclusao = cert.data_conclusao,
                instituicao = cert.instituicao,
                ativo = Convert.ToBoolean(cert.ativo),
                Path = path,
                Thumb = thumb,
                Principal = true,
                cargaHoraria = cert.carga_horaria
            };
        }
    }
}
