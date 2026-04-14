using Colaboracao.Infra.Repositories.Usuario.GestaoDeAcesso;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Usuario.Usuario.GestaoDeAcesso;
using Core.Domain.Usuario.GestaoDeAcesso;
using Core.Domain.Usuario.Permissao;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Usuario.Domain.Impl.Services.GestaoDeAcesso.Recurso;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
	{
        public static void AddGestaoDeAcessoServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScoped<IRecursoService, RecursoService>();
            services.TryAddScoped<IRecursoValidatorService, RecursoValidatorService>();
            services.TryAddScoped<IRecursoOrgDisponivelService, RecursoOrgDisponivelService>();

            services.TryAddScoped<IRecursoRepository, RecursoRepository>();
            services.TryAddScoped<IRecursoMenuRepository, RecursoMenuRepository>();
            services.TryAddScoped<IRecursoOrgDisponivelRepository, RecursoOrgDisponivelRepository>();

            services.TryAddScoped<IRecursoMenuFuncionalidadeSistemaRepository, RecursoMenuFuncionalidadeSistemaRepository>();
            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
        }
    }
}
