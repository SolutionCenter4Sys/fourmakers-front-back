using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.MapaDeRelacionamento;
using Colaboracao.Infra.Repositories.Organograma;
using Colaboracao.Infra.Repositories.VCX;
using Colaboracao.Initializer;
using Core.Domain.MapaDeRelacionamento;
using Core.Domain.Organograma;
using Core.Domain.VCX;
using Logs.Infra.Extensions;
using MapaDeRelacionamento.Domain.Impl;
using MapaDeRelacionamento.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapaDeRelacionamento.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicoLogDBDependencies();

        services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));

        //Services
        services.AddScopedDomainService<IMapaDeRelacionamentoService, MapaDeRelacionamentoService>();
        services.AddScopedDomainService<IVCXService, VCXService>();

        //Repositoryes
        services.AddScoped(typeof(IOrganogramaRepository), typeof(OrganogramaRepository));
        services.AddScopedDomainService<IVCXRepository, VCXRepository>();
        services.AddScopedDomainService<IMapaDeRelacionamentoRepository, MapaDeRelacionamentoRepository>();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}
