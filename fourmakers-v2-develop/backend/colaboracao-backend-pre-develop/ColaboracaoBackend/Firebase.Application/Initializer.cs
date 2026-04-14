using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories.Notificacao;
using Colaboracao.Initializer;
using Core.Domain.Notificacao;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firebase.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);

            services.AddSingleton(typeof(ILogCore), typeof(LogCore));
            services.AddScopedDomainService<IFirebaseService, FirebaseService>();
            services.AddScopedDomainService<INotificacaoService, NotificacaoService>();
            services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddSingleton(typeof(IFirebaseSDK), typeof(FirebaseSDK));
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}