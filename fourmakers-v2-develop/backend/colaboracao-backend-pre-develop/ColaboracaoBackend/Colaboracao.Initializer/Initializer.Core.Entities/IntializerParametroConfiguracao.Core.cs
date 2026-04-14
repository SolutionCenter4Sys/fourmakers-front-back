using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Fourmakers;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Colaborador;
using Core.Domain.ParametroOrg;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Org;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddParametroConfiguracaoServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScoped<IParametroConfiguracaoService, ParametroConfiguracaoService>();
            services.TryAddScoped<IParametroConfiguracaoValidatorService, ParametroConfiguracaoValidatorService>();
            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.TryAddScoped<IUsuarioColaboradorRepository, UsuarioColaboradorRepository>();
            services.TryAddScoped<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
            services.TryAddScoped<IBuscaColaboradorRepository, BuscaColaboradorRepository>();
            services.TryAddScoped<IGrupoAcessoRepository, GrupoAcessoRepository>();
            services.TryAddScoped<IOrgRepository, OrgRepository>();
            services.TryAddScoped<IParametroService, ParametroService>();
            services.TryAddScoped<IParametroValidatorService, ParametroValidatorService>();
            services.TryAddScoped<IParametroRepository, ParametroRepository>();
        }
    }
}