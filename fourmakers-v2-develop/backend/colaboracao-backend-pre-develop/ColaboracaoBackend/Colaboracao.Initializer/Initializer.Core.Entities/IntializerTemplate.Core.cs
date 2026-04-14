using Colaboracao.Infra.Repositories.TemplateEmail;
using Core.Domain.TemplateEmail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TemplateOrg.Impl;
using TemplateOrg.Interfaces;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddTemplateRepositoryDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<ITemplateRepository, TemplateRepository>();
                services.TryAddTransient<ITemplateOrgService, TemplateOrgService>();
            }
            else
            {
                services.TryAddScoped<ITemplateRepository, TemplateRepository>();
                services.TryAddScoped<ITemplateOrgService, TemplateOrgService>();
            }
        }
    }
}