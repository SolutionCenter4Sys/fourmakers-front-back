using Colaboracao.Core;
using Colaboracao.Infra.Repositories.Financeiro.NotaFiscal;
using Colaboracao.Infra.Repositories.Financeiro.Rubrica;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.TemplateEmail;
using Core.Domain.Financeiro.NotaFiscal;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.TemplateEmail;
using Core.DomainModel.Org;
using Financeiro.Domain.Impl.NotaFiscal;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer;

public static partial class InitializerExtension
{
    public static void AddNotaFiscalServicesDependencies(this IServiceCollection services)
    {
        services.TryAddScopedDomainService<INotaFiscalService, NotaFiscalService>();
        services.TryAddScoped<INotaFiscalRepository, NotaFiscalRepository>();
        services.TryAddScoped<INotaFiscalLogRepository, NotaFiscalLogRepository>();
        services.TryAddScoped<INotaFiscalRubricaRepository, NotaFiscalRubricaRepository>();
        services.TryAddScopedDomainService<INotaFiscalExternoService, NotaFiscalExternoService>();
        services.TryAddScoped<ITemplateRepository, TemplateRepository>();
        services.TryAddScoped<IEnvioEmail, EnvioEmail>();
        services.TryAddScoped<IOrgRepository, OrgRepository>();
        services.TryAddScoped<IRubricaColaboradorLiberacaoNfRepository, RubricaColaboradorLiberacaoNfRepository>();
    }
}