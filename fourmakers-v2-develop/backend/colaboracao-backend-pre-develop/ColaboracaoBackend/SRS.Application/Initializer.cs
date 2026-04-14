using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Calculos;
using Colaboracao.Infra.Repositories.Candidato;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Competencia;
using Colaboracao.Infra.Repositories.Competencia.Dominio;
using Colaboracao.Infra.Repositories.Fourmakers;
using Colaboracao.Infra.Repositories.Idioma;
using Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.SSO;
using Colaboracao.Infra.Repositories.TemplateEmail;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Vaga;
using Colaboracao.Infra.Repositories.SRS;
using Colaboracao.Infra.Repositories.Contratacao;
using Core.Domain.Contratacao;
using Core.Domain.SRS;
using Core.Domain.Labs;
using Colaboracao.Infra.Repositories.Labs;
using Colaboracao.Initializer;
using ColaboracaoBridge.Domain.Impl.Services;
using ColaboracaoBridge.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Validadores;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Labs.Domain.Impl;
using Labs.Domain.Interfaces;
using Logs.Infra.Extensions;
using Core.Domain.Candidato;
using Core.Domain.Colaborador;
using Core.Domain.Competencia.Metodologia;
using Core.Domain.Dominio;
using Core.Domain.IIdioma;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.ParametroOrg;
using Core.Domain.Projeto;
using Core.Domain.Social;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using Core.DomainModel;
using Core.DomainModel.Calculos;
using Core.DomainModel.Competencia;
using Core.DomainModel.Org;
using Core.DomainModel.Projeto;
using Core.DomainModel.Softskill;
using Core.DomainModel.SSO;
using CRM.Infra;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Impl.Perfil;
using MapaDeAlocacao.Domain.Impl.Perfil.PerfilValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.Perfil;
using MapaDeAlocacao.Domain.Interfaces.Perfil.PerfilValidaAcesso;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Domain.Impl;
using Social.Domain.Interfaces;
using SRS.Domain.Impl.Service;
using SRS.Domain.Impl.Service.Validadores;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;
using TemplateOrg.Impl;
using TemplateOrg.Interfaces;
using Usuario.Domain.Impl.Services;
using Usuario.Domain.Interfaces.Services;

namespace SRS.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddServicoLogDBDependencies();
            services.AddColaboradorServicesDependencies();
            services.AddCidadaniaServicesDependencies();

            services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
            services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(ICompetenciaClient), typeof(CompetenciaClient));
            services.AddScoped(typeof(IMetodologiaClient), typeof(MetodologiaClient));
            services.AddScoped(typeof(IDominioClient), typeof(DominioClient));
            services.AddScoped(typeof(ISoftskillClient), typeof(SoftskillClient));
            services.AddScoped(typeof(ISkillDesconhecidaClient), typeof(SkillDesconhecidaClient));
            services.AddScoped(typeof(ICurriculoClient), typeof(CurriculoClient));
            services.AddScoped(typeof(IIdiomaClient), typeof(IdiomaClient));
            services.AddScoped(typeof(ICompetenciaColaboradorRepository), typeof(Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository));
            services.AddScoped(typeof(IEnvioEmail), typeof(EnvioEmail));
            services.AddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
            services.AddScopedDomainService<ISRSCandidateService, SRSCandidateService>();
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.AddScoped(typeof(ISRSRepository), typeof(SRSRepository));
            services.AddScopedDomainService<ISRSService, SRSService>();
            services.AddScopedDomainService<IVagaSRSService, VagaSRSService>();
            services.AddScopedDomainService<IVagaService, VagaService>();
            services.AddScoped(typeof(IHardSkillRepository), typeof(HardSkillRepository));
            services.AddScoped(typeof(ITemplateOrgService), typeof(TemplateOrgService));
            services.AddScoped(typeof(IUsuarioService), typeof(UsuarioService));
            services.AddScoped(typeof(ITemplateRepository), typeof(TemplateRepository));
            services.AddScoped(typeof(IAcessoUsuarioRepository), typeof(AcessoUsuarioRepository));
            services.AddScoped(typeof(ISSOClient), typeof(SSOClient));
            services.AddScoped(typeof(ISSORepository), typeof(SSORepository));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.AddScoped(typeof(IUsuarioExternoRepository), typeof(UsuarioExternoRepository));
            services.AddScoped(typeof(IUsuarioService), typeof(UsuarioService));
            services.AddScoped<IColaboradorKeeperRepository, ColaboradorKeeperRepository>();
            services.AddScoped<IComentarioVagaRepository, ComentarioVagaRepository>();
            services.AddScoped<ISRSColaboracaoClient, SRSColaboracaoClient>();
            services.AddScoped<IConnectionStringCore, ConnectionStringCore>();
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
            services.AddScoped(typeof(IAcessoUsuarioRepository), typeof(AcessoUsuarioRepository));
            services.AddScoped(typeof(IAcessoUsuarioService), typeof(AcessoUsuarioService));
            services.AddTokenSistemaDependencies();
            services.AddScoped<IOrgRepository, OrgRepository>();

            services.AddScoped(typeof(ISoftskillRepository), typeof(SoftskillRepository));
            services.AddScoped(typeof(ISoftskillNivelRepository), typeof(SoftskillNivelRepository));
            services.AddScoped(typeof(IMetodologiasRepository), typeof(MetodologiasRepository));
            services.AddScoped(typeof(IDominioRepository), typeof(DominioRepository));
            services.AddScoped(typeof(IIdiomaRepository), typeof(IdiomaRepository));
            services.AddScoped(typeof(IIdiomaNivelRepository), typeof(IdiomaNivelRepository));
            services.AddScoped(typeof(ISRSVagaRepository), typeof(SRSVagaRepository));
            services.AddScoped(typeof(IPerfilAlocacaoRepository), typeof(PerfilAlocacaoRepository));
            services.AddScoped(typeof(IVagaFourmakersRepository), typeof(VagaFourmakersRepository));
            services.AddScoped<IColaboracaoBridgeClient, ColaboracaoBridgeClient>();
            services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
            services.AddScopedDomainService<ICandidaturaService, CandidaturaService>();
            services.AddScoped<ICandidaturaValidatorService, CandidaturaValidatorService>();
            services.AddScoped<IVagaValidatorService, VagaValidatorService>();

            services.AddScoped<IGestaoAlocadosRepository, GestaoAlocadosRepository>();
            services.AddScoped<IGestorExternoRepository, GestorExternoRepository>();

            services.AddScoped<ICandidatoRepository, CandidatoRepository>();
            services.AddQuestionarioServicesDependencies();

            // Contratacao
            services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
            services.AddScopedDomainService<IContratacaoService, ContratacaoService>();
            services.AddScoped<ITemplateContratacaoValidatorService, TemplateContratacaoValidatorService>();
            services.AddScoped<ITemplateContratacaoLogRepository, TemplateContratacaoLogRepository>();
            services.AddScopedDomainService<ITemplateContratacaoLogService, TemplateContratacaoLogService>();
            services.AddScoped<ISistemasLiberadosRepository, SistemasLiberadosRepository>();
            services.AddScopedDomainService<ISistemasLiberadosService, SistemasLiberadosService>();
            services.AddScoped<IDiretoriosRepository, DiretoriosRepository>();
            services.AddScopedDomainService<IDiretoriosService, DiretoriosService>();
            services.AddScoped<IGruposEmailsRepository, GruposEmailsRepository>();
            services.AddScopedDomainService<IGruposEmailsService, GruposEmailsService>();

            services.AddScoped<IMatchClient, MatchClient>();
            services.AddScoped<ILabsLogRankCandidatesIdsRepository, LabsLogRankCandidatesIdsRepository>();
            services.AddScoped<ILabsLogScoreSingleCandidatesRepository, LabsLogScoreSingleCandidatesRepository>();
            services.AddScopedDomainService<IMatchService, MatchService>();
            services.AddScoped<ILabsLogExtractorExtractVagaRepository, LabsLogExtractorExtractVagaRepository>();
            services.AddScopedDomainService<IExtractorService, ExtractorService>();
            services.AddScoped<IComentarioCandidaturaService, ComentarioCandidaturaService>();
            services.AddScoped<IComentarioCandidaturaRepository, ComentarioCandidaturaRepository>();

            services.AddScopedDomainService<ICRMBridgeService, CRMBridgeService>();
            services.AddScoped<ICRMRepository, CRMRepository>();


            services.AddScopedDomainService<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();
            services.AddScoped<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
            services.AddScoped<IPerfilService, PerfilService>();
            services.AddScoped<IPerfilRepository, PerfilRepository>(); 
            services.AddScoped<IPerfilValidarAcessoService, PerfilValidarAcessoService>();

            services.AddScoped<ICalculoTributosCltService, TributosCltService>();
            services.AddScoped<IRemuneracaoVerbasAnuaisCalculador, RemuneracaoVerbasAnuaisCalculador>();
            services.AddScopedDomainService<ICalculosService, CalculosService>();
            services.AddScoped<ICalculosValidatorService, CalculosValidatorService>();
            services.AddScoped<IInssRepository, InssRepository>();
            services.AddScoped<IIrrfRepository, IrrfRepository>();
            services.AddScoped<IIrrfReducaoRepository, IrrfReducaoRepository>();
            services.AddScoped<IRemuneracaoCltRepository, RemuneracaoCltRepository>();
            services.AddScoped<IParametrizacaoSimuladorRepository, ParametrizacaoSimuladorRepository>();
            services.AddScoped<IAdmissaoCargoRepository, AdmissaoCargoRepository>();

            services.AddTemplateRepositoryDependencies();
            services.AddHistoricoCVDependencies();

            // Clients
            services.AddScoped<ISRSVagaClient, SRSVagaClient>();
            services.AddScoped(typeof(IUsuarioClient), typeof(UsuarioClient));

            services.AddSingleton(typeof(ILogCore), typeof(LogCore));

            services.AddAwsSqsDependencies();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}