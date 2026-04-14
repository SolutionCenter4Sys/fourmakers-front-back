using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Pricing.Equipe;
using DataTransferObject.Domain.Util.Enum;
using Foursys.Domain.Interfaces.Services;
using iText.IO.Util;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class GestaoAlocadosService : IGestaoAlocadosService
    {
        private readonly IGestaoAlocadosRepository _gestaoAlocadosRepository;
        private readonly IGestaoAlocadosValidarAcessoService _gestaoAlocadosValidarAcessoService;
        private readonly ISincronizaCRMService _sincronizaCRMService;
        private readonly IPricingClient _pricingClient;
        private readonly IGestorExternoPerfilService _gestorExternoPerfilService;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IAderenciaService _aderenciaService;
        private readonly IMatchClient _matchClient;
        private readonly ILogCore _log;

        public GestaoAlocadosService(IGestaoAlocadosRepository gestaoAlocadosRepository,
                                     IGestaoAlocadosValidarAcessoService gestaoAlocadosValidarAcessoService,
                                     ISincronizaCRMService sincronizaCRMService,
                                     IPricingClient pricingClient,
                                     IGestorExternoPerfilService gestorExternoPerfilService,
                                     IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
                                     IAderenciaService aderenciaService,
                                     IMatchClient matchClient,
                                     ILogCore log)
        {
            _gestaoAlocadosRepository = gestaoAlocadosRepository;
            _gestaoAlocadosValidarAcessoService = gestaoAlocadosValidarAcessoService;
            _pricingClient = pricingClient;
            _sincronizaCRMService = sincronizaCRMService;
            _gestorExternoPerfilService = gestorExternoPerfilService;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _aderenciaService = aderenciaService;
            _matchClient = matchClient;
            _log = log;
        }

        public async Task<ApiGenericResult<IEnumerable<ClienteOrgDaGestaoAlocadosResult>>> ListarClienteOrgDaGestaoDeAlocados(string cpfRequest, int limite, int cursor, bool semPerfil, string buscaCodigoOuNome, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ClienteOrgDaGestaoAlocadosResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var result = await _gestaoAlocadosRepository.ListarClienteOrgDaGestaoDeAlocados(buscaCodigoOuNome, limite, cursor, semPerfil, orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarClienteOrgDaGestaoDeAlocados));
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<GestoresEPerfisDaGestaoDeAlocadosResult>>> ListarGestoresEPerfisDaGestaoDeAlocados(string cpfRequest, int limite, int cursor, string busca, string codigoCliente, bool semPerfil, bool semAreaAtuacao, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestoresEPerfisDaGestaoDeAlocadosResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var result = await _gestaoAlocadosRepository.ListarGestoresEPerfisDaGestaoDeAlocados(limite, cursor, busca, codigoCliente, semPerfil, semAreaAtuacao, orgId);

                if (result == null)
                {
                    throw new ApplicationException("Erro ao Listar Gestores e Perfis.");
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarGestoresEPerfisDaGestaoDeAlocados));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<RateCardsDosPerfisResult>>> ListarRateCardsDosPerfisPorCliente(string cpfRequest, int limite, int cursor, string busca, string codigoCliente, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RateCardsDosPerfisResult>>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var result = await _gestaoAlocadosRepository.ListarRateCardsDosPerfisPorCliente(limite, cursor, busca, codigoCliente, orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarRateCardsDosPerfisPorCliente));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<RateCardsDosPerfisHistoricoResult>>> ListarRateCardsDosPerfisHistoricoPorPerfilId(string cpfRequest, int limite, int cursor, Guid gestorExternoPerfilId, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RateCardsDosPerfisHistoricoResult>>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var result = await _gestaoAlocadosRepository.ListarRateCardsDosPerfisHistoricoPorPerfilId(limite, cursor, gestorExternoPerfilId, orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarRateCardsDosPerfisHistoricoPorPerfilId));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<PermanenciaResult>>> ListarPermanencias(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<PermanenciaResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                var result = await _gestaoAlocadosRepository.ListarPermanenciasAsync();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarPermanencias));
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>> ListarModelosTrabalho(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                var result = await _gestaoAlocadosRepository.ListarModelosTrabalhoAsync();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarModelosTrabalho));
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>> ListarModelosTrabalhoPublico()
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ModeloTrabalhoResult>>();
            try
            {
                var result = await _gestaoAlocadosRepository.ListarModelosTrabalhoAsync();
                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarModelosTrabalhoPublico));
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<ProfissionalLocalidadeResult>>> ListarProfissionaisLocalidades(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ProfissionalLocalidadeResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                var result = await _gestaoAlocadosRepository.ListarProfissionaisLocalidadesAsync();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarProfissionaisLocalidades));
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<RatecardClienteResult>>> ObterRatecardPorCodigoCliente(string cpfRequest, string codCliente, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RatecardClienteResult>>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var propostas = await _gestaoAlocadosRepository.ListarPropostasPorCliente(codCliente, orgId);
                if (propostas == null)
                {
                    throw new ApplicationException("Erro ao consultar as propostas dos clientes.");
                }

                var resultPricing = await BuscarRatecardNoPricing(propostas);
                apiGenericResult.Retorno = resultPricing;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ObterRatecardPorCodigoCliente));
            }

            return apiGenericResult;
        }

        private async Task<List<RatecardClienteResult>> BuscarRatecardNoPricing(IEnumerable<string> propostas)
        {
            try
            {
                if (!propostas.Any())
                {
                    return new List<RatecardClienteResult>();
                }

                var resultClient = await _pricingClient.GetRatecardByPropostas(propostas);
                return resultClient.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao buscar informações de Ratecard no Pricing.", ex);
            }
        }

        public async Task<ApiGenericResult<SincronizarCRMResult>> SincronizarClientesEGestoresCRM(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<SincronizarCRMResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                await _sincronizaCRMService.SincronizarCRM();

                var result = await _sincronizaCRMService.GetDataUltimaSincronizacaoClientesEGestoresCRM();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(SincronizarClientesEGestoresCRM));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<SincronizarCRMResult>> GetDataUltimaSincronizacaoClientesEGestoresCRM(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<SincronizarCRMResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _sincronizaCRMService.GetDataUltimaSincronizacaoClientesEGestoresCRM();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(GetDataUltimaSincronizacaoClientesEGestoresCRM));
            }
            return apiGenericResult;
        }
        public async Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorCliente(string cpfRequest, int limite, int cursor, string busca, string codCliente, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var result = await _gestaoAlocadosRepository.ListarColaboradoresAlocadosAtualmentePorCliente(limite, cursor, busca, codCliente, orgId);

                var listaParaCalculoAderencia = result
                    .Select(x => new PerfilEColaboradorAderenciaDTO
                    {
                        CodigoInternoColaborador = x.CodigoInternoColaborador, 
                        PerfilId = x.PerfilId ?? Guid.NewGuid()
                    }
                ).ToList();

                foreach (var colab in result)
                {
                    try
                    {
                        if(colab.PerfilId.HasValue)
                        {
                            var perfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorId(colab.PerfilId.Value, cpfRequest, orgId);
                            colab.RetornoMatch = await _aderenciaService.BuscarMatchPerfilAsync(perfil, colab.CodigoInternoColaborador, orgId);
                        }
                        else
                        {
                            _log.Log($"Perfil nulo ListarColaboradoresAlocadosPorCliente ao buscar match do colab:{colab.CodigoInternoColaborador}", LevelsEnum.Information);

                            colab.RetornoMatch = new CandidatosMatchResponse();
                            colab.RetornoMatch.DetalhamentoCalculo = new DetalhamentoCalculo();
                            colab.RetornoMatch.ComparativoPorSkill = new ComparativoPorSkill();
                            colab.RetornoMatch.DetalhamentoCalculo.HardSkills = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.SoftSkills = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Idiomas = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Metodologias = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.DominiosNegocio = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Disponibilidades = new CategoriaScore();
                            colab.RetornoMatch.ComparativoPorSkill.HardSkills = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.SoftSkills = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Idiomas = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Metodologias = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.DominiosNegocio = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Disponibilidades = new List<SkillComparativa>();
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ListarColaboradoresAlocadosPorCliente ao buscar match do colab:{colab.CodigoInternoColaborador}", LevelsEnum.Information);

                        colab.RetornoMatch = new CandidatosMatchResponse();
                        colab.RetornoMatch.DetalhamentoCalculo = new DetalhamentoCalculo();
                        colab.RetornoMatch.ComparativoPorSkill = new ComparativoPorSkill();
                        colab.RetornoMatch.DetalhamentoCalculo.HardSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.SoftSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Idiomas = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Metodologias = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.DominiosNegocio = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Disponibilidades = new CategoriaScore();
                        colab.RetornoMatch.ComparativoPorSkill.HardSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.SoftSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Idiomas = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Metodologias = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.DominiosNegocio = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Disponibilidades = new List<SkillComparativa>();
                    }
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarClienteOrgDaGestaoDeAlocados));
            }

            return apiGenericResult;
        }
        public async Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorClienteSemAderencia(string cpfRequest, int limite, int cursor, string busca, string codCliente, int orgId)
        {
            _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

            //Sem Aderência
            var apiGenericResult = new ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>();

            try
            {
                var result = await _gestaoAlocadosRepository.ListarColaboradoresAlocadosAtualmentePorCliente(limite, cursor, busca, codCliente, orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarClienteOrgDaGestaoDeAlocados));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>> ListarColaboradoresAlocadosPorClienteCompleto(string codColaborador, int limite, int cursor, string codCliente, int orgId, string codGestorAdm, string codGestorOper)
        {
            _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(codColaborador, orgId);

            //Completo com Aderência e Match
            var apiGenericResult = new ApiGenericResult<IEnumerable<ColaboradoresAlocadosDaGestaoAlocadosResult>>();

            try
            {
                var result = await _gestaoAlocadosRepository.ListarColaboradoresAlocadosPorClienteCompletoDetalhe(limite, cursor, codCliente, orgId, codGestorAdm, codGestorOper);

                var listaParaCalculoAderencia = result
                    .Select(x => new PerfilEColaboradorAderenciaDTO
                    {
                        CodigoInternoColaborador = x.CodigoInternoColaborador,
                        PerfilId = x.PerfilId ?? Guid.NewGuid()
                    }
                ).ToList();

                //Calculo Aderencia
                var Aderencias = await _aderenciaService.GerarListaAderenciaSimplificada(listaParaCalculoAderencia, codColaborador, orgId);

                foreach (var aderencia in Aderencias)
                {
                    var colaborador = result.Where(x => x.CodigoInternoColaborador == aderencia.CodigoInternoColaborador && x.PerfilId == aderencia.PerfilId).FirstOrDefault();
                    colaborador.Aderencia = aderencia.CalculoAderencia;
                }

                foreach (var colab in result)
                {
                    try
                    {
                        if (colab.PerfilId.HasValue)
                        {
                            var perfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorId(colab.PerfilId.Value, codColaborador, orgId);
                            colab.RetornoMatch = await _aderenciaService.BuscarMatchPerfilAsync(perfil, colab.CodigoInternoColaborador, orgId);
                        }
                        else
                        {
                            _log.Log($"Perfil nulo ListarColaboradoresAlocadosPorCliente ao buscar match do colab:{colab.CodigoInternoColaborador}", LevelsEnum.Information);

                            colab.RetornoMatch = new CandidatosMatchResponse();
                            colab.RetornoMatch.DetalhamentoCalculo = new DetalhamentoCalculo();
                            colab.RetornoMatch.ComparativoPorSkill = new ComparativoPorSkill();
                            colab.RetornoMatch.DetalhamentoCalculo.HardSkills = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.SoftSkills = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Idiomas = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Metodologias = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.DominiosNegocio = new CategoriaScore();
                            colab.RetornoMatch.DetalhamentoCalculo.Disponibilidades = new CategoriaScore();
                            colab.RetornoMatch.ComparativoPorSkill.HardSkills = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.SoftSkills = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Idiomas = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Metodologias = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.DominiosNegocio = new List<SkillComparativa>();
                            colab.RetornoMatch.ComparativoPorSkill.Disponibilidades = new List<SkillComparativa>();
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ListarColaboradoresAlocadosPorCliente ao buscar match do colab:{colab.CodigoInternoColaborador}", LevelsEnum.Information);

                        colab.RetornoMatch = new CandidatosMatchResponse();
                        colab.RetornoMatch.DetalhamentoCalculo = new DetalhamentoCalculo();
                        colab.RetornoMatch.ComparativoPorSkill = new ComparativoPorSkill();
                        colab.RetornoMatch.DetalhamentoCalculo.HardSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.SoftSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Idiomas = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Metodologias = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.DominiosNegocio = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Disponibilidades = new CategoriaScore();
                        colab.RetornoMatch.ComparativoPorSkill.HardSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.SoftSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Idiomas = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Metodologias = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.DominiosNegocio = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Disponibilidades = new List<SkillComparativa>();
                    }
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarClienteOrgDaGestaoDeAlocados));
            }

            return apiGenericResult;
        }
    }
}