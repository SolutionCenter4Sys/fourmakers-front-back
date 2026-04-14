using ApiClient.Domain;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotina.Aws.Core.Interfaces;
using Rotina.Aws.Domain.Impl;

namespace Rotina.CuradoriaSkill.Application;

public class Initializer : IInitializer
{
    public void Configure(IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<CuradoriaSkillJobService>();
        ClientConfig.SetConfiguration(ref config);
        services.AddTransient<IApiClient, ApiClient.Infra.Impl.ApiClient>();
        services.AddTransient<ICuradoriaClient, CuradoriaClient>();
        services.AddTransient<ICompetenciaClient, CompetenciaClient>();
    }
}