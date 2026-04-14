using Amazon.S3;
using ApiClient.Domain;
using ApiClient.Domain.Impl;
using Aws.Infra.Impl.S3;
using Aws.Infra.Interfaces.S3;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using CargaCore.Domain.Interfaces;
using CargaCore.Domain.Services;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Context;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.Candidato;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.LogRepo;
using Colaboracao.Infra.Repositories.MapaAlocacao;
using Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.Notificacao;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Questionario;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Configuracao;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Publicacao;
using Colaboracao.Infra.Repositories.TemplateEmail;
using Colaboracao.Infra.Repositories.RotinaContratosVencidos;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.SSO;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Vaga;
using Colaboracao.Infra.RotinaIntegracao;
using Colaboracao.Initializer;
using ColaboracaoBridge.Domain.Impl.Services;
using ColaboracaoBridge.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Apontamento;
using Core.Domain.Candidato;
using Core.Domain.Colaborador;
using Core.Domain.LogRepo;
using Core.Domain.MapaAlocacao;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Notificacao;
using Core.Domain.Projeto;
using Core.Domain.Questionario;
using Core.Domain.Marketing.Comunicacao.Configuracao;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using Core.Domain.TemplateEmail;
using Core.Domain.RotinaContratosVencidos;
using Core.Domain.Social;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using Core.DomainModel;
using Core.DomainModel.Competencia;
using Core.DomainModel.Org;
using Core.DomainModel.Projeto;
using Core.DomainModel.SSO;
using CRM.Infra;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RotinasBackoffice.API.Mock.Impl;
using RotinasBackoffice.API.Mock.Interface;
using RotinasBackoffice.Domain.Impl.Services;
using RotinasBackoffice.Domain.Interfaces.Services;
using SRS.Domain.Impl.Service;
using SRS.Domain.Impl.Service.Validadores;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;
using Usuario.Domain.Impl.Services;
using Usuario.Domain.Interfaces.Services;

namespace RotinasBackoffice.Application
{
    public static class Initializer
    {
        public static void Configure(IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);

            // RotinasBackoffice não tem autenticação

            // Não utilizamos InitializerCore para AddDBGColbColaboracaoContext e
            // ServicosComunsColaboracao pois é transient aqui no Rotinas Backoffice

            var connectionString = ConnectionStringCore.DBGColbConnectionString;
            services.AddDbContextFactory<ColaboradorContext>(x => x.UseMySQL(connectionString));
            services.AddTransient<ColaboradorContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IApiClient, ApiClient.Infra.Impl.ApiClient>();
            services.AddTransient(typeof(IAspNetUser), typeof(AspNetUser));
            services.AddTransient<ILogCore, LogCore>();
            services.AddTransient(typeof(ILogRepository), typeof(LogRepository));
            services.AddTransient<ITokens, Tokens>();
            services.AddTransient(typeof(IUsuarioClient), typeof(UsuarioClient));

            services.AddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.AddTransient(typeof(ICCHClient), typeof(CCHClient));
            services.AddTransient(typeof(IMapaAlocacaoRepository), typeof(MapaAlocacaoRepository));
            services.AddTransient(typeof(IMockFoursys), typeof(MockFoursys));
            services.AddTransient(typeof(IMockShowCase), typeof(MockShowCase));
            services.AddTransient(typeof(IProjetoMapaDeAlocacaoRepository), typeof(ProjetoMapaDeAlocacaoRepository));
            services.AddTransient(typeof(IRotinaIntegracaoRepository), typeof(RotinaIntegracaoRepository));
            services.AddTransient(typeof(IRotinaMigracaoRepository), typeof(RotinaMigracaoRepository));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient<IUsuarioGrupoAcessoRepository, UsuarioGrupoAcessoRepository>();
            services.AddTransient<IAtividadeProjetoRepository, AtividadeProjetoRepository>();
            services.AddTransient<IBuscaColaboradorRepository, BuscaColaboradorRepository>();
            services.AddTransient<ICargaFourmakerService, CargaFourmakerService>();
            services.AddTransient<ICCHClient, CCHClient>();
            services.AddTransient<IClienteOrgRepository, ClienteOrgRepository>();
            services.AddTransient<IColaboradorClient, ColaboradorClient>();
            services.AddTransient<IColaboradorKeeperRepository, ColaboradorKeeperRepository>();
            services.AddTransient<ICompetenciaClient, CompetenciaClient>();
            services.AddTransient<ICompetenciaColaboradorRepository, Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository>();
            services.AddTransient<IEnvioEmail, EnvioEmail>();
            services.AddTransient<IFirebaseClient, FirebaseClient>();
            services.AddTransient<INotificacaoService, NotificacaoService>();
            services.AddTransient<INotificacaoRepository, NotificacaoRepository>();

            services.AddTransient<IMapaAlocacaoRepository, MapaAlocacaoRepository>();
            services.AddTransient<IProjetoMapaDeAlocacaoRepository, ProjetoMapaDeAlocacaoRepository>();
            services.AddTransient<IProjetoOrgRepository, ProjetoOrgRepository>();
            services.AddTransient<IStatusDtoRepository, StatusRepository>();
            services.AddTransient<ITokenDtoRepository, TokenRepository>();
            services.AddTransient<ITokenSistemaDtoRepository, TokenSistemaRepositoryDiscontinued>();
            services.AddTransient<ITokenUsuarioAcessoDtoRepository, TokenUsuarioAcessoRepository>();
            services.AddTransient<IRotinaIntegracaoRepository, RotinaIntegracaoRepository>();
            services.AddTransient<IRotinaMigracaoRepository, RotinaMigracaoRepository>();
            services.AddTransient<ISRSClient, SRSClient>();
            services.AddTransient<ISRSColaboracaoClient, SRSColaboracaoClient>();
            services.AddTransient<ISRSInfraClient, SRSInfraClient>();
            services.AddTransient<ISRSRepository, SRSRepository>();
            services.AddTransient<ISRSService, SRSService>();
            services.AddTransient<ISSOClient, SSOClient>();
            services.AddTransient<ISSORepository, SSORepository>();
            services.AddTransient<ITokenSSODtoRepository, TokenSSORepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUploadFilesClient, UploadFilesClient>();
            services.AddTransient<IUsuarioClient, UsuarioClient>();
            services.AddTransient<IUsuarioColaboradorRepository, UsuarioColaboradorDapperRepository>();
            services.AddTransient<IUsuarioExternoRepository, UsuarioExternoRepository>();
            services.AddTransient<IUsuarioDtoRepository, UsuarioRepository>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IVagaService, VagaService>();
            services.AddTransient<IVagasSRSServices, VagasSRSServices>();
            services.AddTransient<IBuscaColaboradorRepository, BuscaColaboradorRepository>();

            services.AddTransient<IConnectionStringCore, ConnectionStringCore>();
            services.AddTransient<IHistoricoCVRepository, HistoricoCVRepository>();
            services.AddTransient<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.AddTransient<IPerfilAlocacaoRepository, PerfilAlocacaoRepository>();
            services.AddTransient<ICandidaturaRepository, CandidaturaRepository>();
            services.AddTransient<IComentarioCandidaturaRepository, ComentarioCandidaturaRepository>();

            services.AddTransient<IAcessoUsuarioRepository, AcessoUsuarioRepository>();
            services.AddTransient<IOrgRepository, OrgRepository>();
            services.AddScoped<IDBConnection, DBConnection>();
            services.AddScoped<IDBConnectionUnitOfWork, DBConnectionUnitOfWork>();

            // RotinaLimpezaAws: S3 + token file temp (mesmo bucket/chave do UploadFiles)
            services.AddTransient(typeof(IAmazonS3), typeof(AmazonS3Client));
            services.AddTransient(typeof(IAmazonS3Uploader), typeof(AmazonS3Uploader));
            services.AddTransient<ITokenFileTempRepository, TokenFileTempRepository>();

            // Clients
            services.AddTransient<ISRSVagaClient, SRSVagaClient>();

            services.AddTransient<IVagaFourmakersRepository, VagaFourmakersRepository>();
            services.AddTransient<IVagaValidatorService, VagaValidatorService>();
            services.AddTransient<IGestaoAlocadosRepository, GestaoAlocadosRepository>();
            services.AddTransient<ICandidatoRepository, CandidatoRepository>();
            services.AddTransient<IMatchClient, MatchClient>();
            services.AddTransient<ICRMBridgeService, CRMBridgeService>();
            services.AddTransient<ICRMRepository, CRMRepository>();
            services.AddTransient<IComentarioVagaRepository, ComentarioVagaRepository>();
            services.AddTransient<IRotinaContratosVencidosRepository, RotinaContratosVencidosRepository>();
            services.AddTransient<IComunicacaoConfiguracaoRepository, ComunicacaoConfiguracaoRepository>();
            services.AddTransient<IComunicacaoPublicacaoRepository, ComunicacaoPublicacaoRepository>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();
            services.AddTransient(typeof(IClassificacaoService), typeof(ClassificacaoService));
            services.AddTransient(typeof(IClassificacaoRepository), typeof(ClassificacaoRepository));
            services.AddTransient(typeof(IClassificacaoClient), typeof(ClassificacaoClient));
            services.AddTransient(typeof(IElasticSearchClient), typeof(ElasticSearchClient));

            services.AddTransient(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));

            services.AddTransient<IApontamentoRepository, ApontamentoRepository>();
            services.AddTransient<IQuestionarioRespostaRepository, QuestionarioRespostaRepository>();
            services.AddLocalization();

            services.AddTemplateRepositoryDependencies(true);
            services.AddHistoricoCVDependencies();
            services.AddBuscaParametroConfiguracaoServicesDependencies(true);
            services.AddCargaClienteSincronizarCRMServiceDependencies(true);
            services.AddServicoLogDBDependencies(true);
            
            // Registrar como Transient antes de chamar AddQuestionarioServicesDependencies
            // para evitar problemas com HostedServices (Singleton) consumindo serviços Scoped
            services.AddTransient<Core.Domain.Questionario.IQuestionarioRespostaRepository, 
                Colaboracao.Infra.Repositories.Questionario.QuestionarioRespostaRepository>();
            services.AddTransient<Questionario.Domain.Interfaces.IQuestionarioService, 
                Questionario.Domain.Impl.QuestionarioService>();
            
            services.AddQuestionarioServicesDependencies();
        }
    }
}