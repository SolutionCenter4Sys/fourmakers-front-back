using Colaboracao.Core;
using Colaboracao.Infra.Repositories.LogRepo;
using Core.Domain.LogRepo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddServicoLogDBDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<ILogDBCore, LogDBCore>();
                services.TryAddTransient<ILogRepository, LogRepository>();
                services.TryAddTransient<IEnvioEmail, EnvioEmail>();
            }
            else
            {
                services.TryAddScoped<ILogDBCore, LogDBCore>();
                services.TryAddScoped<ILogRepository, LogRepository>();
                services.TryAddScoped<IEnvioEmail, EnvioEmail>();
            }
        }
    }
}