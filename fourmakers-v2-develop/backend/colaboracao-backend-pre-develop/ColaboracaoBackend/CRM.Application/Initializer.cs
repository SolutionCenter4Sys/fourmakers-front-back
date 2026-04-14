using ApiClient.Domain;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Initializer;
using CRM.Domain.Impl.Services;
using CRM.Domain.Interfaces.Services;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);
            JWTAuth.ConfigureJWT(services);
            //CRM não utiliza ColaboradorContext (não faz conexão no banco)

            services.AddServicosComunsColaboracao();

            services.AddScoped(typeof(IColaboracaoBridgeClient), typeof(ColaboracaoBridgeClient));
            services.AddScopedDomainService<ICRMService, CRMService>();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}