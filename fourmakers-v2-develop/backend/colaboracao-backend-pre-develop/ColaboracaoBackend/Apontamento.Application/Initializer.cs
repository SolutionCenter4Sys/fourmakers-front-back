using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Apontamento.Domain.Impl.Service;
using Apontamento.Domain.Impl.Service.Validador;
using Apontamento.Domain.Interfaces.Service;
using Apontamento.Domain.Interfaces;
using Apontamento.Infra.Impl;
using Apontamento.Infra.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.Lote;
using Colaboracao.Infra.Repositories.Notificacao;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Initializer;
using Core.Domain;
using Core.Domain.Apontamento;
using Core.Domain.Notificacao;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aws.Infra.Impl;
using Aws.Infra.Interfaces;
using Core.Domain.Projeto;
using Colaboracao.Infra.Repositories.Projeto;
using Logs.Infra.Extensions;

namespace Apontamento.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddBuscaParametroConfiguracaoServicesDependencies();

            services.AddScoped(typeof(IApontamentoInfraClient), typeof(ApontamentoInfraClient));
            services.AddScoped(typeof(IApontamentoRepository), typeof(ApontamentoRepository));
            services.AddScoped(typeof(IFolhaPontoRepository), typeof(FolhaPontoRepository));
            services.AddScoped(typeof(ILoteRepository), typeof(LoteRepository));
            //services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(BuscaColaboradorRepository));
            //services.AddScoped(typeof(IBuscaColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));

            //UsuarioColaboradorDapperRepository : IUsuarioColaboradorRepository
            //UsuarioColaboradorRepository

            services.AddScopedDomainService<IApontamentoService, ApontamentoService>();
            services.AddScopedDomainService<IFolhaPontoService, FolhaPontoService>();
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
            services.AddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.AddScoped(typeof(ICurriculoClient), typeof(CurriculoClient));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.AddScoped(typeof(IQueueProducer), typeof(AmazonSQSProducer));

            services.AddSingleton(typeof(IApontamentoCacheService), typeof(ApontamentoCacheService));
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));

            services.AddScoped(typeof(IPeriodoFechadoRepository), typeof(PeriodoFechadoRepository));
            services.AddScopedDomainService<IPeriodoFechadoService, PeriodoFechadoService>();
            services.AddScoped(typeof(INotificacaoService), typeof(NotificacaoService));
            services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddScoped<IApontamentoValidadorService, ApontamentoValidadorService>();

            services.AddTemplateRepositoryDependencies();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}