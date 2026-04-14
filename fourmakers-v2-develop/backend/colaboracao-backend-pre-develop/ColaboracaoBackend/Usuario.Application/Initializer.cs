using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Context;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.LogRepo;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Questionario;
using Colaboracao.Infra.Repositories.SSO;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Initializer;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.DomainModel;
using Core.Domain.LogRepo;
using Core.Domain.Projeto;
using Core.Domain.Questionario;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Competencia;
using Core.DomainModel.Org;
using Core.DomainModel.Projeto;
using Core.DomainModel.SSO;
using Core.DomainModel.Usuario;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;
using Core.DomainModel;
using Usuario.Domain.Impl.Services;
using Usuario.Domain.Impl.Services.Permissao;
using Usuario.Domain.Interfaces.Services;
using Usuario.Domain.Interfaces.Services.Permissao;
using Logs.Infra.Extensions;

namespace Usuario.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddGestaoDeAcessoServicesDependencies();
            services.AddServicoLogDBDependencies();
            services.AddBuscaParametroConfiguracaoServicesDependencies();
            
            // Registrar serviços de logging
            services.AddLogsInfra();

            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
            services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(IColaboradorKeeperRepository), typeof(ColaboradorKeeperRepository));
            services.AddScoped(typeof(IComentarioClient), typeof(ComentarioClient));
            services.AddScoped(typeof(ICompetenciaClient), typeof(CompetenciaClient));
            services.AddScoped(typeof(ICompetenciaColaboradorRepository), typeof(Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository));
            services.AddScoped(typeof(IEnvioEmail), typeof(EnvioEmail));
            services.AddScoped(typeof(ILogRepository), typeof(LogRepository));
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
            services.AddScoped(typeof(IStatusDtoRepository), typeof(StatusRepository));
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.AddScoped(typeof(ISSOClient), typeof(SSOClient));
            services.AddScoped(typeof(ISSORepository), typeof(SSORepository));
            
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.AddScoped(typeof(IUsuarioDtoRepository), typeof(UsuarioRepository));
            services.AddScoped(typeof(IUsuarioExternoRepository), typeof(UsuarioExternoRepository));
            services.AddScoped(typeof(ColaboradorContext), typeof(ColaboradorContext));
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISSOClient), typeof(SSOClient));
            services.AddScoped(typeof(IComentarioClient), typeof(ComentarioClient));
            services.AddScoped(typeof(IUsuarioClient), typeof(UsuarioClient));
            services.AddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.AddScoped(typeof(IEnvioEmail), typeof(EnvioEmail));
            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(IColaboradorKeeperRepository), typeof(ColaboradorKeeperRepository));
            services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
            services.AddScoped(typeof(ICompetenciaColaboradorRepository), typeof(Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository));
            services.AddScoped(typeof(ISSORepository), typeof(SSORepository));
            services.AddScoped(typeof(ICompetenciaClient), typeof(CompetenciaClient));
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            
            services.AddScopedDomainService<IUsuarioService, UsuarioService>();
            services.AddScoped(typeof(IUsuarioRelatorioRepository), typeof(UsuarioRelatorioRepository));
            services.AddScopedDomainService<IExtracaoUsuarioService, ExtracaoUsuarioService>();

            services.AddScopedDomainService<IGrupoAcessoService, GrupoAcessoService>();
            services.AddScopedDomainService<IUsuarioGrupoAcessoService, UsuarioGrupoAcessoService>();
            services.AddScopedDomainService<IFuncionalidadeSistemaService, FuncionalidadeSistemaService>();

            services.AddScoped(typeof(IPermissaoLogRepository), typeof(PermissaoLogRepository));
            services.AddScoped(typeof(IGrupoAcessoRepository), typeof(GrupoAcessoRepository));
            services.AddScoped(typeof(IUsuarioGrupoAcessoRepository), typeof(UsuarioGrupoAcessoRepository));
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));

            services.AddScopedDomainService<IExtracaoUsuarioService, ExtracaoUsuarioService>();
            services.AddScoped<IConnectionStringCore, ConnectionStringCore>();

            services.AddTemplateRepositoryDependencies();
            services.AddHistoricoCVDependencies();

            services.AddScoped(typeof(IAcessoUsuarioRepository), typeof(AcessoUsuarioRepository));
            services.AddScopedDomainService<IAcessoUsuarioService, AcessoUsuarioService>();
            services.AddTokenSistemaDependencies();

            services.AddScoped(typeof(IOrgRepository), typeof(OrgRepository));

            services.AddSingleton<IUsuarioCacheService>(sp =>
            {
                var implementation = ActivatorUtilities.CreateInstance<UsuarioCacheService>(sp);
                var loggerFactory = sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
                return Logs.Infra.Interceptors.LogDomainInterceptor<IUsuarioCacheService>.Create(implementation, loggerFactory);
            });

            services.AddScoped<IQuestionarioRespostaRepository, QuestionarioRespostaRepository>();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}