using Colaboracao.Infra.Repositories.Financeiro.Financeiro.Banco;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Financeiro.Banco;
using Core.Domain.Usuario.Permissao;
using Financeiro.Domain.Impl.Banco.CadastroBanco;
using Financeiro.Domain.Interfaces.Banco.CadastroBanco;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
	{
        public static void AddBancoServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScoped<ICadastroBancoService, CadastroBancoService>();
            services.TryAddScoped<ICadastroBancoValidatorService, CadastroBancoValidatorService>();
            services.TryAddScoped<ICadastroBancoRepository, CadastroBancoRepository>();

            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
        }
    }
}
