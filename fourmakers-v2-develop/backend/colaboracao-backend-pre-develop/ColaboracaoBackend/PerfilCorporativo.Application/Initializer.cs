using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Initializer;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PerfilCorporativo.Domain.Impl;
using PerfilCorporativo.Domain.Interfaces;

namespace PerfilCorporativo.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicoLogDBDependencies();

        services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
        services.AddScopedDomainService<IPerfilCorporativoService, PerfilCorporativoService>();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}
