using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using BotFourmakers.Domain.Impl;
using BotFourmakers.Domain.Interfaces;
using Colaboracao.Infra.Repositories.BotFourmakers;
using Colaboracao.Initializer;
using Core.Domain.BotFourmakers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotFourmakers.Application;

public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicosComunsColaboracao();
        services.AddBotFourmakersDependencies();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
    {
        app.ConfigureAppStandard(env, projetcName);
    }
}