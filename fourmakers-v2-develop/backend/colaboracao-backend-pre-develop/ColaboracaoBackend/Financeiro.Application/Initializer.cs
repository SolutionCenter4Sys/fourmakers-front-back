using Colaboracao.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Financeiro.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicosComunsColaboracao();
        services.AddReembolsoDependencies();
        services.AddRubricaServicesDependencies();
        services.AddHoleriteServicesDependencies();
        services.AddTokenSistemaDependencies();
        services.AddNotaFiscalServicesDependencies();
        services.AddDadosBancariosColaboradorServicesDependencies();
        services.AddBancoServicesDependencies();
        services.AddIntegracaoBancariaServicesDependencies();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}