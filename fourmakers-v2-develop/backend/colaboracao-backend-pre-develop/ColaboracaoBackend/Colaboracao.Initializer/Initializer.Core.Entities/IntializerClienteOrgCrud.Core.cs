using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Projeto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Projeto.Domain.Impl.Services;
using Projeto.Domain.Interfaces.Services;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddClienteOrgCrudServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScoped<IClienteOrgCrudRepository, ClienteOrgCrudRepository>();
            services.TryAddScoped<IClienteOrgCrudService, ClienteOrgCrudService>();
            services.TryAddScoped<IClienteOrgCrudValidatorService, ClienteOrgCrudValidatorService>();

            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
        }
    }
}