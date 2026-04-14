using ApiClient.Domain;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Initializer;
using Core.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Domain.Impl;
using MockServer.Domain.Interfaces.Services;

namespace MockServer.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);

            // BI não utiliza autenticação

            var connectionString = config.GetConnectionString("ColaboradorConnection");
            services.AddDBGColbColaboracaoContext(connectionString);

            ClientConfig.SetConfiguration(ref config);

            JWTAuth.ConfigureJWT(services);

            services.AddServicosComunsColaboracao();

            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.AddScoped(typeof(IMockServerService), typeof(MockServerService));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IVerificaSeCpfESistemico), typeof(VerificaSeCpfESistemico));
        }
        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}