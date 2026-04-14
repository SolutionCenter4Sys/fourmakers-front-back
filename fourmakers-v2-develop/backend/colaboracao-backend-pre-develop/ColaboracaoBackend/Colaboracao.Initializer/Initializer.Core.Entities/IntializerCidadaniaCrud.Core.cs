using Colaboracao.Infra.Repositories;
using Colaborador.Domain.Impl.Services;
using Colaborador.Domain.Interfaces.Services;
using Core.DomainModel;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddCidadaniaServicesDependencies(this IServiceCollection services)
        {
            services.AddScopedDomainService<ICidadaniaService, CidadaniaService>();
            services.AddScoped<ICidadaniaRepository, CidadaniaRepository>();

            services.AddScoped<ICidadaniaValidadorService, CidadaniaValidadorService>();
        }
    }
}