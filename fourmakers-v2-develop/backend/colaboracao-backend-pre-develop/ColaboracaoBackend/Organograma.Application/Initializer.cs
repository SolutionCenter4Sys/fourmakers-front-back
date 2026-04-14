using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.Organograma;
using Colaboracao.Initializer;
using Core.Domain.Organograma;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organograma.Domain.Impl;
using Organograma.Domain.Interfaces;

namespace Organograma.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicoLogDBDependencies();

        services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
        services.AddScoped(typeof(IOrganogramaRepository), typeof(OrganogramaRepository));
        services.AddScopedDomainService<IOrganogramaService, OrganogramaService>();
        services.AddScoped(typeof(IOrganogramaLogRepository), typeof(OrganogramaLogRepository));
        services.AddScopedDomainService<IOrganogramaLogService, OrganogramaLogService>();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}