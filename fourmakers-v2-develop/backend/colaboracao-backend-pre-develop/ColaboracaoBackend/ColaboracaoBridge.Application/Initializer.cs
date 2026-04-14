using ApiClient.Domain;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Initializer;
using ColaboracaoBridge.Domain.Impl.Services;
using ColaboracaoBridge.Domain.Interfaces.Services;
using Core.Domain;
using CRM.Infra;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ColaboracaoBridge.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);

            services.AddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.AddScoped(typeof(IUsuarioClient), typeof(UsuarioClient));
            services.AddScopedDomainService<ICRMBridgeService, CRMBridgeService>();
            services.AddScoped(typeof(ICRMRepository), typeof(CRMRepository));

            services.AddHealthChecks();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}