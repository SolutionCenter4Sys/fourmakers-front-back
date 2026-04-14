using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Core.Domain.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Financeiro.Financeiro.IntegracaoBancaria;
using Colaboracao.Infra.Repositories.Financeiro.IntegracaoBancaria;
using Financeiro.Domain.Impl.IntegracaoBancaria.CnabOrg;
using Financeiro.Domain.Impl.IntegracaoBancaria.RemessaBancaria;
using Financeiro.Domain.Impl.IntegracaoBancaria.RetornoBancaria;
using Financeiro.Domain.Impl.IntegracaoBancaria.Externo;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RemessaBancaria;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RetornoBancaria;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.Externo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
	{
        public static void AddIntegracaoBancariaServicesDependencies(this IServiceCollection services)
        {
            // CNAB Org
            services.TryAddScoped<ICnabOrgService, CnabOrgService>();
            services.TryAddScoped<ICnabOrgValidatorService, CnabOrgValidatorService>();
            services.TryAddScoped<ICnabOrgRepository, CnabOrgRepository>();

            // Remessa Bancária
            services.TryAddScoped<IRemessaBancariaService, RemessaBancariaService>();
            services.TryAddScoped<IPagamentoCnabRepository, PagamentoCnabRepository>();
            services.TryAddScoped<ICnabRemessaRepository, CnabRemessaRepository>();

            // Retorno Bancária
            services.TryAddScoped<IRetornoBancariaService, RetornoBancariaService>();
            services.TryAddScoped<ICnabRetornoRepository, CnabRetornoRepository>();

            // Integração Externa
            services.TryAddScoped<IIntegracaoBancariaExternaService, IntegracaoBancariaExternaService>();

            // Permissões
            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
        }
    }
}
