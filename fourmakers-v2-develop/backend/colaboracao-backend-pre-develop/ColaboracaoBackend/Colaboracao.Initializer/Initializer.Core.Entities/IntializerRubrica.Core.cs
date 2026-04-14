using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica;
using Colaboracao.Infra.Repositories.Financeiro.Rubrica;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.Usuario.Permissao;
using Financeiro.Domain.Impl.NotaFiscal;
using Financeiro.Domain.Impl.Rubrica.Rubrica;
using Financeiro.Domain.Impl.Rubrica.RubricaCarga;
using Financeiro.Domain.Impl.Rubrica.RubricaColaborador;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Financeiro.Domain.Interfaces.Rubrica.Rubrica;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
	{
        public static void AddRubricaServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScopedDomainService<IRubricaService, RubricaService>();
            services.TryAddScoped<IRubricaValidatorService, RubricaValidatorService>();
            services.TryAddScoped<IRubricaRepository, RubricaRepository>();

            services.TryAddScopedDomainService<IRubricaColaboradorService, RubricaColaboradorService>();
            services.TryAddScoped<IRubricaColaboradorValidatorService, RubricaColaboradorValidatorService>();
            services.TryAddScoped<IRubricaColaboradorRepository, RubricaColaboradorRepository>();

            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.TryAddScoped<ICurriculoClient, CurriculoClient>();
            services.TryAddScopedDomainService<IRubricaCargaService, RubricaCargaService>();
            services.TryAddScoped<IRubricaCargaRepository, RubricaCargaRepository>();

            services.TryAddScopedDomainService<IRubricaCargaExternoService, RubricaCargaExternoService>();
            services.TryAddScoped<IBuscaColaboradorOrgRepository, BuscaColaboradorOrgRepository>();
            services.TryAddScoped<IRubricaCargaMassivaLogRepository, RubricaCargaMassivaLogRepository>();


        }
    }
}
