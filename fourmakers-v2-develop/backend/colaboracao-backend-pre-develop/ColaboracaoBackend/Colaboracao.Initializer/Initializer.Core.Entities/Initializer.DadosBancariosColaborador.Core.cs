using Colaboracao.Infra.Repositories.Financeiro.Banco;
using Core.Domain.Financeiro.Banco;
using Financeiro.Domain.Impl.Banco;
using Financeiro.Domain.Interfaces.Banco;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Colaboracao.Initializer;

public static partial class InitializerExtension
{
    public static void AddDadosBancariosColaboradorServicesDependencies(this IServiceCollection services)
    {
        services.AddScoped<IDadosBancariosColaboradorRepository, DadosBancariosColaboradorRepository>();
        services.AddScopedDomainService<IDadosBancariosColaboradorService, DadosBancariosColaboradorService>();
        services.AddScoped<IDadosBancariosColaboradorValidadorService, DadosBancariosColaboradorValidadorService>();
    }
}