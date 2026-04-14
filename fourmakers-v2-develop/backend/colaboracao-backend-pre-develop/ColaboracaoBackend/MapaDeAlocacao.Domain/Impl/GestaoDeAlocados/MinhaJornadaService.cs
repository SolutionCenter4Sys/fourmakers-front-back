using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Core.DomainModel;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Util.Enum;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class MinhaJornadaService : IMinhaJornadaService
    {

        private readonly IMinhaJornadaRepository _minhaJornadaRepository;
        private readonly IGestorExternoPerfilService _gestorExternoPerfilService;
        private readonly IMatchClient _matchClient;
        private readonly IGestorExternoPerfilRepository _gestorExternoPerfilRepository;
        private readonly IGestorExternoPerfilSkillService _gestorExternoPerfilSkillService;
        private readonly ICompetenciaDtoRepository _competenciaRepository;

        public MinhaJornadaService(IMinhaJornadaRepository minhaJornadaRepository, IGestorExternoPerfilService gestorExternoPerfilService, IMatchClient matchClient, IGestorExternoPerfilRepository gestorExternoPerfilRepository, IGestorExternoPerfilSkillService gestorExternoPerfilSkillService, ICompetenciaDtoRepository competenciaRepository)
        {
            _minhaJornadaRepository = minhaJornadaRepository;
            _gestorExternoPerfilService = gestorExternoPerfilService;
            _matchClient = matchClient;
            _gestorExternoPerfilRepository = gestorExternoPerfilRepository;
            _gestorExternoPerfilSkillService = gestorExternoPerfilSkillService;
            _competenciaRepository = competenciaRepository;
        }

        public async Task<ApiGenericResult<List<MinhaJornadaColaboradorDTO>>> BuscarSkillColaborador(string codColaborador)
        {
            try
            {
                var list = await _minhaJornadaRepository.BuscarSkillColaborador(codColaborador);
                return list == null ? new ApiGenericResult<List<MinhaJornadaColaboradorDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Minha Skill não encontrados."

                } : new ApiGenericResult<List<MinhaJornadaColaboradorDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Minha Skill encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Minha Skill: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<MinhaJornadaDTO>>> BuscarSkillColaboradorAlocado(string codColaborador, int orgId)
        {
            try
            {
                var list = await _minhaJornadaRepository.BuscarSkillColaboradorAlocado(codColaborador, orgId);
                return list == null ? new ApiGenericResult<List<MinhaJornadaDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Minha Alocados não encontrados."

                } : new ApiGenericResult<List<MinhaJornadaDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Skill Alocados encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Skill Alocados: {e.Message}");
            }
        }
        //--------Sugestão
        public async Task<ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>> InserirSugestao(SugestaoParamDTO param, bool minhaJornada, string cpfUsuarioLogado)
        {
            try
            {
                var partner = await _minhaJornadaRepository.InserirSugestao(param);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = param.CodigoInternoColaborador,
                        GestorExternoPerfil = param.GestorExternoPerfil,
                        SkillId = param.Skill_Id,
                        ItemPerfil = param.Tipo_Id,
                        NivelId = param.Senioridade_Id,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.SUGERIDA,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = cpfUsuarioLogado
                    };

                    await _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return partner == null ? new ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Inserir o Sugestão."

                } : new ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão inserido. id: {partner.Id}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao inserir Sugestão: {e.Message}");
            }
        }
        
        public async Task<ApiGenericResult<List<SugestaoHistoricoDTO>>> AprovarRejeitarSugestao(SugestaoHistoricoParamDTO param)
        {
            try
            {
                var partner = await _minhaJornadaRepository.AprovarRejeitarSugestao(param);

                if (partner != null && param.TbStatusSugestaoId == 1)
                {
                    await _gestorExternoPerfilSkillService.InserirGestorExternoPerfilSkill(new GestorExternoPerfilSkillInput()
                    {
                        ItemPerfil = new ItemPerfilInput
                        {
                            Id = param.ItemPerfil
                        },
                        Skill = new SkillInput
                        {
                            Id = param.SkillId
                        },
                        Nivel = new NivelInput
                        {
                            Id = param.NivelId
                        }
                    }, 
                    Guid.Parse(param.Perfil_Id),
                    param.CodigoInternoColaborador);
                }

                return partner == null ? new ApiGenericResult<List<SugestaoHistoricoDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Aprovar/Rejeitar o Sugestão."

                } : new ApiGenericResult<List<SugestaoHistoricoDTO>>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão Aprovado/Rejeitado."
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao Aprovar/Rejeitar Sugestão: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<SugestaoSkillResponseDTO>> AtualizarSugestao(SugestaoAtualizacaoParamDTO param)
        {
            try
            {
                var partner = await _minhaJornadaRepository.AtualizarSugestao(param);
                return partner == null ? new ApiGenericResult<SugestaoSkillResponseDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Atualizar o Sugestão."

                } : new ApiGenericResult<SugestaoSkillResponseDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão atualizado. id: {partner.Id}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao atualizar Sugestão: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarSugestao(string id)
        {
            try
            {
                var partner = await _minhaJornadaRepository.DeletarSugestao(id);
                return partner == false ? new ApiGenericResult<bool>
                {
                    Retorno = false,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Deletar o Sugestão."

                } : new ApiGenericResult<bool>
                {
                    Retorno = true,
                    Sucesso = true,
                    Mensagem = $"Sugestão deletado. id: {id}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar Sugestão: {e.Message}");
            }
        }
        public async Task<ApiGenericResult<SugestaoSkillResponseDTO>> BuscarSugestaoPorId(string id)
        {
            try
            {
                var partner = await _minhaJornadaRepository.BuscarSugestaoPorId(id);
                return partner == null ? new ApiGenericResult<SugestaoSkillResponseDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Listar o Sugestão."

                } : new ApiGenericResult<SugestaoSkillResponseDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão listado. id: {id}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Sugestão: {e.Message}");
            }
        }
        public async Task<ApiGenericResult<List<SugestaoSkilleHistoricoResponseDTO>>> BuscarSugestaoPorCodColaboradorOuAdm(string codInternoColaborador, string codInternoGestor, string perfilId)
        {
            try
            {
                var partner = await _minhaJornadaRepository.BuscarSugestaoPorCodColaboradorOuAdm(codInternoColaborador, codInternoGestor, perfilId);
                return partner == null ? new ApiGenericResult<List<SugestaoSkilleHistoricoResponseDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Listar o Sugestão."

                } : new ApiGenericResult<List<SugestaoSkilleHistoricoResponseDTO>>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão listado."
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar BuscarSugestaoPorCodColaboradorOuAdm: {e.Message}");
            }
        }

        
        public async Task<ApiGenericResult<List<SugestaoHistoricoDTO>>> BuscarHistoricoPorId(string sugestaoId)
        {
            try
            {
                var partner = await _minhaJornadaRepository.BuscarHistoricoPorId(sugestaoId);
                return partner == null ? new ApiGenericResult<List<SugestaoHistoricoDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Listar o Sugestão/Histórico."

                } : new ApiGenericResult<List<SugestaoHistoricoDTO>>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão/Histórico listado. id: {partner.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Sugestão/Histórico: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>> BuscarSugestaoHistoricoPorId(string sugestaoId)
        {
            try
            {
                var partner = await _minhaJornadaRepository.BuscarSugestaoHistoricoPorId(sugestaoId);
                return partner == null ? new ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Não foi possível Listar o Sugestão/Histórico."

                } : new ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>
                {
                    Retorno = partner,
                    Sucesso = true,
                    Mensagem = $"Sugestão/Histórico listado."
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Sugestão/Histórico: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<CandidatosMatchResponse>> CalcularAderenciaDoColaboradorAoPerfil(Guid perfilId, string codigoInternoColaborador, int orgId, string cpfRequest)
        {
            try
            {
                var perfil = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorIdAsync(perfilId);

                if (perfil == null)
                {
                    ExceptionUtil.NaoEncontrado("Perfil");
                }

                var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(perfil.Id);
                perfil.GestorExternoPerfilSkills = resultSkill.Retorno;

                var baseRequest = new ScoreSingleCandidateRequest
                {
                    HardSkills = perfil.GestorExternoPerfilSkills
                        .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.COMPETENCIA && 
                                   !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                                   !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                        .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                    SoftSkills = perfil.GestorExternoPerfilSkills
                        .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.SOFTSKILL && 
                                   !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                                   !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                        .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                    Metodologias = perfil.GestorExternoPerfilSkills
                        .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.METODOLOGIA && 
                                   !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                                   !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                        .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                    DominiosNegocio = perfil.GestorExternoPerfilSkills
                        .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.DOMINIONEGOCIO && 
                                   !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                                   !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                        .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                    Idiomas = perfil.GestorExternoPerfilSkills
                        .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.IDIOMA && 
                                   !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                                   !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                        .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                    PesoHardSkills = 1,
                    PesoSoftSkills = 1,
                    PesoMetodologias = 1,
                    PesoDominiosNegocio = 1,
                    PesoIdiomas = 1,
                    PesoDisponibilidades = 1,
                    VisibleToOrgIds = new List<int> { orgId, EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                    Disponibilidades = new List<DisponibilidadeItem>(),
                    NumeroDeCandidatos = 1,
                    CodigoInternoColaborador = codigoInternoColaborador
                };

                var matchResponse = await _matchClient.ScoreSingleCandidate(baseRequest);

                return new ApiGenericResult<CandidatosMatchResponse>
                {
                    Retorno = matchResponse,
                    Sucesso = true,
                    Mensagem = "Aderência calculada com sucesso."
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao calcular aderência do colaborador ao perfil: {e.Message}");
            }
        }

    }
}
