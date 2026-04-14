using Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.MinhaEquipeValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class MinhaEquipeService : IMinhaEquipeService
    {

        private readonly IMinhaEquipeRepository _minhaEquipeRepository;
        private readonly IMinhaEquipeValidarAcessoService _minhaEquipeValidarAcessoService;
        private readonly IAderenciaService _aderenciaService;
        private readonly IGestorExternoPerfilService _gestorExternoPerfilService;

        public MinhaEquipeService(IMinhaEquipeRepository minhaEquipeRepository, IMinhaEquipeValidarAcessoService minhaEquipeValidarAcessoService, IAderenciaService aderenciaService, IGestorExternoPerfilService gestorExternoPerfilService)
        {
            _minhaEquipeRepository = minhaEquipeRepository;
            _minhaEquipeValidarAcessoService = minhaEquipeValidarAcessoService;
            _aderenciaService = aderenciaService;
            _gestorExternoPerfilService = gestorExternoPerfilService;
        }

        public async Task<ApiGenericResult<List<MinhaEquipeDTO>>> BuscarLideradosPorGestor(string codColaboradorGestor, string perfilId, int orgId)
        {
            try
            {
                var list = await _minhaEquipeRepository.BuscarLideradosPorGestor(codColaboradorGestor, perfilId, orgId);
                return list == null ? new ApiGenericResult<List<MinhaEquipeDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Minha Alocados não encontrados."

                } : new ApiGenericResult<List<MinhaEquipeDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Liderados por Gestor encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Liderados por Gestor: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorColaboradoresSkill>>> ListaIndicadoresDosLiderados(string codColaborador, string perfilId, int orgId)
        {
            try
            {
                var list = await _minhaEquipeRepository.ListaIndicadoresDosLiderados(codColaborador, perfilId, orgId);
                return list == null ? new ApiGenericResult<List<GestorColaboradoresSkill>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Lista Indicadores dos Liderados não encontrados."

                } : new ApiGenericResult<List<GestorColaboradoresSkill>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Lista Indicadores dos Liderados encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Indicadores dos Liderados: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDosLideradosPorGestor(string codGestorAdm, string perfilId, int orgId)
        {
            try
            {
                var list = await _minhaEquipeRepository.ListaAderenciaDosLideradosPorGestor(codGestorAdm, perfilId, orgId);
                return list == null ? new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Liderados por Gestor não encontrados."

                } : new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Liderados por Gestor encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Liderados por Gestor: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorCandidatosMatchResponsePerfil>>> ListaAderenciaDosLideradosPorGestorSemParamPerfil(string cpfRequest, string codGestorAdm, string codGestorOper, int orgId)
        {
            try
            {
                // Se codGestorAdm for nulo/vazio → MINHA_EQUIPE_ORQUESTRACAO, se preenchido → MINHA_EQUIPE
                if (string.IsNullOrEmpty(codGestorAdm))
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipeOrquestracao(cpfRequest, orgId);
                }
                else
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipe(cpfRequest, orgId);
                }
                var list = await _minhaEquipeRepository.ListaAderenciaDosLideradosPorGestorSemParamPerfil(codGestorAdm, codGestorOper, orgId);
                return list == null ? new ApiGenericResult<List<GestorCandidatosMatchResponsePerfil>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Liderados por Gestor com Match não encontrados."

                } : new ApiGenericResult<List<GestorCandidatosMatchResponsePerfil>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Liderados por Gestor com Match encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Liderados com Match por Gestor: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaborador(string codColaborador, string perfilId, int orgId)
        {
            try
            {
                var list = await _minhaEquipeRepository.ListaAderenciaDoColaborador(codColaborador, perfilId, orgId);
                return list == null ? new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Aderências do Colaborador não encontrados."

                } : new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Aderências do Colaborador encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Aderências do Colaborador: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaboradorAdmOper(string codColaborador, int orgId)
        {
            try
            {
                var list = await _minhaEquipeRepository.ListaAderenciaDoColaboradorAdmOper(codColaborador, orgId);
                return list == null ? new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Aderências do Colaborador não encontrados."

                } : new ApiGenericResult<List<GestorCandidatosMatchResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Aderências do Colaborador encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Aderências do Colaborador: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<GestorColaboradoresSkillAdmOper>>> ListaIndicadoresDosLideradosAdmOper(string cpfRequest, int orgId, int limite, int cursor, string codGestorAdm, string codGestorOper, string codCliente)
        {
            try
            {
                // Se codGestorAdm for nulo/vazio → MINHA_EQUIPE_ORQUESTRACAO, se preenchido → MINHA_EQUIPE
                if (string.IsNullOrEmpty(codGestorAdm))
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipeOrquestracao(cpfRequest, orgId);
                }
                else
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipe(cpfRequest, orgId);
                }
                var list = await _minhaEquipeRepository.ListaIndicadoresDosLideradosAdmOper(orgId, limite, cursor, codGestorAdm, codGestorOper, codCliente);
                if (list == null || !list.Any())
                {
                    return new ApiGenericResult<List<GestorColaboradoresSkillAdmOper>>
                    {
                        Retorno = null,
                        Sucesso = false,
                        Mensagem = "Liderados por Gestor, Colaboradores não encontrados."
                    };
                }

                // Coletar todos os PerfilIds únicos
                var perfilIds = new HashSet<Guid>();
                foreach (var gestor in list)
                {
                    foreach (var colaborador in gestor.Colaboradores)
                    {
                        foreach (var cliente in colaborador.Clientes)
                        {
                            if (!string.IsNullOrEmpty(cliente.PerfilId) && Guid.TryParse(cliente.PerfilId, out Guid perfilIdGuid))
                            {
                                perfilIds.Add(perfilIdGuid);
                            }
                        }
                    }
                }

                // Buscar todos os perfis de uma vez
                var perfisDict = new Dictionary<Guid, GestorExternoPerfilResult>();
                if (perfilIds.Any())
                {
                    var perfisResult = await _gestorExternoPerfilService.ObterGestoresExternoPerfilPorListaDeIds(perfilIds.ToList(), orgId);
                    if (perfisResult != null && perfisResult.Sucesso && perfisResult.Retorno != null)
                    {
                        foreach (var perfil in perfisResult.Retorno)
                        {
                            if (perfil != null)
                            {
                                perfisDict[perfil.Id] = perfil;
                            }
                        }
                    }
                }

                // Buscar match para cada cliente de cada colaborador usando o dicionário
                foreach (var gestor in list)
                {
                    foreach (var colaborador in gestor.Colaboradores)
                    {
                        foreach (var cliente in colaborador.Clientes)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(cliente.PerfilId) && Guid.TryParse(cliente.PerfilId, out Guid perfilIdGuid))
                                {
                                    if (perfisDict.TryGetValue(perfilIdGuid, out var perfil) && perfil != null)
                                    {
                                        var perfilWrapper = new ApiGenericResult<GestorExternoPerfilResult>
                                        {
                                            Retorno = perfil,
                                            Sucesso = true
                                        };
                                        cliente.RetornoMatch = await _aderenciaService.BuscarMatchPerfilAsync(perfilWrapper, colaborador.CodigoInternoColaborador, orgId);
                                    }
                                    else
                                    {
                                        cliente.RetornoMatch = CriarRetornoMatchVazio();
                                    }
                                }
                                else
                                {
                                    cliente.RetornoMatch = CriarRetornoMatchVazio();
                                }
                            }
                            catch (Exception ex)
                            {
                                // Em caso de erro, criar retorno match vazio
                                cliente.RetornoMatch = CriarRetornoMatchVazio();
                            }
                        }
                    }
                }

                // Remover clientes duplicados por PerfilId para cada colaborador
                // Mantém apenas o primeiro cliente encontrado para cada perfil único
                foreach (var gestor in list)
                {
                    foreach (var colaborador in gestor.Colaboradores)
                    {
                        if (colaborador.Clientes != null && colaborador.Clientes.Any())
                        {
                            var clientesUnicos = colaborador.Clientes
                                .GroupBy(c => c.PerfilId ?? string.Empty)
                                .Select(g => g.First())
                                .ToList();
                            
                            colaborador.Clientes = clientesUnicos;
                        }
                    }
                }

                // assume que cada gestor tem a lista 'Colaboradores'
                int totalColaboradores = list
                     .Sum(g => g.Colaboradores?.Count ?? 0); 

                return new ApiGenericResult<List<GestorColaboradoresSkillAdmOper>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Gestores: {list.Count} e seus colaboradores: {totalColaboradores}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Liderados por Gestor: {e.Message}");
            }
        }
        public async Task<ApiGenericResult<List<TotalizacaoIndicadoresPorGestorAdmOper>>> TotalizacaoIndicadoresDosLideradosAdmOper(string cpfRequest, int orgId, string codGestorAdm, string codGestorOper)
        {
            try
            {
                // Se codGestorAdm for nulo/vazio → MINHA_EQUIPE_ORQUESTRACAO, se preenchido → MINHA_EQUIPE
                if (string.IsNullOrEmpty(codGestorAdm))
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipeOrquestracao(cpfRequest, orgId);
                }
                else
                {
                    _minhaEquipeValidarAcessoService.ValidaAcessoMinhaEquipe(cpfRequest, orgId);
                }
                var list = await _minhaEquipeRepository.TotalizacaoIndicadoresDosLideradosAdmOper(orgId, codGestorAdm, codGestorOper);
                return list == null ? new ApiGenericResult<List<TotalizacaoIndicadoresPorGestorAdmOper>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Totalização Liderados por Gestores Adm/Oper não encontrados."

                } : new ApiGenericResult<List<TotalizacaoIndicadoresPorGestorAdmOper>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Totalização Liderados por Gestores Adm/Oper."
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao Totalizar Liderados por Gestores Adm/Oper: {e.Message}");
            }
        }

        private CandidatosMatchResponse CriarRetornoMatchVazio()
        {
            return new CandidatosMatchResponse
            {
                DetalhamentoCalculo = new DetalhamentoCalculo
                {
                    HardSkills = new CategoriaScore(),
                    SoftSkills = new CategoriaScore(),
                    Idiomas = new CategoriaScore(),
                    Metodologias = new CategoriaScore(),
                    DominiosNegocio = new CategoriaScore(),
                    Disponibilidades = new CategoriaScore()
                },
                ComparativoPorSkill = new ComparativoPorSkill
                {
                    HardSkills = new List<SkillComparativa>(),
                    SoftSkills = new List<SkillComparativa>(),
                    Idiomas = new List<SkillComparativa>(),
                    Metodologias = new List<SkillComparativa>(),
                    DominiosNegocio = new List<SkillComparativa>(),
                    Disponibilidades = new List<SkillComparativa>()
                }
            };
        }
    }

}
