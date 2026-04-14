using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.SRS;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Initializer;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Admissao.Domain.Impl.Service;
using Admissao.Domain.Interfaces.Service;
using Core.Domain.SRS;

namespace Admissao.Application;

/// <summary>
/// DI para Admissao API: CBO, Remuneração CLT, Cargo, Status, Pipeline, Admissão e Histórico de Status.
/// </summary>
public class Initializer : IInitializerConfigurator
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBaseGeralServicosColaboracao(config);
        services.AddServicoLogDBDependencies();

        services.AddScoped<IConnectionStringCore, ConnectionStringCore>();

        services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
        services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));

        services.AddScoped<ICboRepository, CboRepository>();
        services.AddScopedDomainService<ICboService, CboService>();

        services.AddScoped<IRemuneracaoCltRepository, RemuneracaoCltRepository>();
        services.AddScopedDomainService<IRemuneracaoCltService, RemuneracaoCltService>();

        services.AddScoped<IAdmissaoCargoRepository, AdmissaoCargoRepository>();
        services.AddScopedDomainService<IAdmissaoCargoService, AdmissaoCargoService>();

        services.AddScoped<IAdmissaoStatusRepository, AdmissaoStatusRepository>();
        services.AddScopedDomainService<IAdmissaoStatusService, AdmissaoStatusService>();

        services.AddScoped<IAdmissaoPipelineRepository, AdmissaoPipelineRepository>();
        services.AddScoped<IAdmissaoPipelineStatusRepository, AdmissaoPipelineStatusRepository>();
        services.AddScopedDomainService<IAdmissaoPipelineValidatorService, AdmissaoPipelineValidatorService>();
        services.AddScopedDomainService<IAdmissaoPipelineService, AdmissaoPipelineService>();

        services.AddScoped<IAdmissaoRepository, AdmissaoRepository>();
        services.AddScoped<IAdmissaoHistoricoStatusRepository, AdmissaoHistoricoStatusRepository>();
        services.AddScoped<IAdmissaoOrigemRepository, AdmissaoOrigemRepository>();
        services.AddScoped<IAdmissaoColaboradorRepository, AdmissaoColaboradorRepository>();
        services.AddScopedDomainService<IAdmissaoValidatorService, AdmissaoValidatorService>();
        services.AddScopedDomainService<IAdmissaoService, AdmissaoService>();
        services.AddScopedDomainService<IAdmissaoHistoricoStatusService, AdmissaoHistoricoStatusService>();
        services.AddScopedDomainService<IAdmissaoOrigemService, AdmissaoOrigemService>();
    }

    public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projectName)
    {
        app.ConfigureAppStandard(env, projectName);
    }
}
