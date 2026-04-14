using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.Candidato;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Competencia;
using Colaboracao.Infra.Repositories.MapaAlocacao;
using Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.Notificacao;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.TemplateEmail;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Vaga;
using Colaboracao.Infra.Repositories.Labs;
using Colaboracao.Initializer;
using ColaboracaoBridge.Domain.Impl.Services;
using ColaboracaoBridge.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Validadores;
using Competencia.Domain.Impl.Factorys;
using Competencia.Domain.Impl.Models;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Apontamento;
using Core.Domain.Candidato;
using Core.Domain.Colaborador;
using Core.Domain.MapaAlocacao;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Notificacao;
using Core.Domain.Projeto;
using Core.Domain.Social;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using Core.Domain.Labs;
using Core.DomainModel;
using Core.DomainModel.Competencia;
using Core.DomainModel.Projeto;
using CRM.Infra;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces;
using Firebase.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Impl;
using MapaDeAlocacao.Domain.Impl.Externo;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces;
using MapaDeAlocacao.Domain.Interfaces.Externo;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SRS.Domain.Impl.Service;
using SRS.Domain.Impl.Service.Validadores;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;
using Labs.Domain.Impl;
using Labs.Domain.Interfaces;
using Logs.Infra.Extensions;

namespace MapaDeAlocacao.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);

            services.AddBuscaParametroConfiguracaoServicesDependencies();
            services.AddGestaoDeAlocadosServicesDependencies();
            services.AddTokenSistemaDependencies();
            services.AddCargaClienteSincronizarCRMServiceDependencies();
            services.AddServicoLogDBDependencies();

            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
            services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(ICompetenciaClient), typeof(CompetenciaClient));
            services.AddScoped(typeof(IDominioClient), typeof(DominioClient));
            services.AddScoped(typeof(ISoftskillClient), typeof(SoftskillClient));
            services.AddScoped(typeof(IMetodologiaClient), typeof(MetodologiaClient));
            services.AddScoped(typeof(IIdiomaClient), typeof(IdiomaClient));
            services.AddScoped(typeof(IMatchClient), typeof(MatchClient));
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
            services.AddScoped(typeof(IExtracaoAlocacaoRepository), typeof(ExtracaoAlocacaoRepository));
            services.AddScopedDomainService<IExtracaoAlocacaoService, ExtracaoAlocacaoService>();
            services.AddScoped(typeof(IMapaAlocacaoRepository), typeof(MapaAlocacaoRepository));
            services.AddScoped(typeof(IMapaDeAlocacaoExternoRepository), typeof(MapaDeAlocacaoExternoRepository));
            services.AddScopedDomainService<IMapaDeAlocacaoService, MapaDeAlocacaoService>();
            services.AddScopedDomainService<IMapaDeAlocacaoExternoService, MapaDeAlocacaoExternoService>();
            services.AddScoped(typeof(IMapaDeAlocacaoValidadorService), typeof(MapaDeAlocacaoValidadorService));
            services.AddScoped(typeof(IProjetoMapaDeAlocacaoRepository), typeof(ProjetoMapaDeAlocacaoRepository));
            services.AddScopedDomainService<IProjetoMapaDeAlocacaoService, ProjetoMapaDeAlocacaoService>();
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
            services.AddScoped(typeof(ITbdRepository), typeof(TbdRepository));
            services.AddScopedDomainService<ITbdService, TbdService>();
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorDapperRepository));
            // Removido duplicado - já registrado acima
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
            services.AddScoped(typeof(IApontamentoRepository), typeof(ApontamentoRepository));
            services.AddScoped(typeof(INotificacaoService), typeof(NotificacaoService));
            services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddScoped<ILabsLogRankCandidatesIdsRepository, LabsLogRankCandidatesIdsRepository>();
            services.AddScoped<ILabsLogExtractorExtractVagaRepository, LabsLogExtractorExtractVagaRepository>();
            services.AddScoped<ILabsLogScoreSingleCandidatesRepository, LabsLogScoreSingleCandidatesRepository>();
            services.AddScopedDomainService<IMatchService, MatchService>();
            services.AddScopedDomainService<IExtractorService, ExtractorService>();
            services.AddScoped(typeof(IVagaService), typeof(VagaService));
            services.AddScoped(typeof(ISRSRepository), typeof(SRSRepository));
            services.AddScoped(typeof(ISRSColaboracaoClient), typeof(SRSColaboracaoClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.AddScoped(typeof(ISRSVagaClient), typeof(SRSVagaClient));
            services.AddScoped(typeof(IVagaFourmakersRepository), typeof(VagaFourmakersRepository));
            services.AddScoped(typeof(IVagaValidatorService), typeof(VagaValidatorService));
            services.AddScoped(typeof(ITemplateRepository), typeof(TemplateRepository));
            services.AddScoped(typeof(ILogCore), typeof(LogCore));
            services.AddScoped(typeof(ICandidatoRepository), typeof(CandidatoRepository));
            services.AddScoped(typeof(ICandidaturaRepository), typeof(CandidaturaRepository));
            services.AddScoped(typeof(IFeedbackLabIARepository), typeof(FeedbackLabIARepository));
            services.AddScopedDomainService<IFeedbackLabIAService, FeedbackLabIAService>();
            services.AddScoped(typeof(IParceirosRepository), typeof(ParceirosRepository));
            services.AddScopedDomainService<IParceirosService, ParceirosService>();
            services.AddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.TryAddScoped(typeof(ICompetenciaDtoRepository), typeof(Colaboracao.Infra.Repositories.CompetenciaRepository));
            services.TryAddScoped(typeof(ICompetenciaHistoricoService), typeof(CompetenciaHistoricoService));
            services.TryAddScoped(typeof(ICompetenciaHistoricoRepository), typeof(CompetenciaHistoricoRepository));
            services.TryAddScoped(typeof(IHistoricoCVRepository), typeof(HistoricoCVRepository));

            services.AddScoped(typeof(IEncontrosRepository), typeof(EncontrosRepository));
            services.AddScopedDomainService<IEncontrosService, EncontrosService>();

            services.AddScopedDomainService<IMapaDemograficoService, MapaDemograficoService>();
            services.AddScoped(typeof(IMapaDemograficoRepository), typeof(MapaDemograficoRepository));
            services.AddScopedDomainService<IMinhaJornadaService, MinhaJornadaService>();
            services.AddScoped(typeof(IMinhaJornadaRepository), typeof(MinhaJornadaRepository));
            services.AddScopedDomainService<IMinhaEquipeService, MinhaEquipeService>();
            services.AddScoped(typeof(IMinhaEquipeRepository), typeof(MinhaEquipeRepository));

            services.AddScopedDomainService<ICRMBridgeService, CRMBridgeService>();
            services.AddScoped(typeof(ICRMRepository), typeof(CRMRepository));

            services.AddScoped(typeof(IFirebaseSDK), typeof(FirebaseSDK));
            services.AddScoped(typeof(IComentarioVagaRepository), typeof(ComentarioVagaRepository));
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}