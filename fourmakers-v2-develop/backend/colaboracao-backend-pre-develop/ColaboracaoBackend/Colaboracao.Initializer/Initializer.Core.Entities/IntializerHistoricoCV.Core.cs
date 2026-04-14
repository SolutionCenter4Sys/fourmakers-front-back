using Colaboracao.Infra.Repositories.Colaborador;
using Core.Domain.Colaborador;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddHistoricoCVDependencies(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IHistoricoCVRepository), typeof(HistoricoCVRepository));
        }
    }
}