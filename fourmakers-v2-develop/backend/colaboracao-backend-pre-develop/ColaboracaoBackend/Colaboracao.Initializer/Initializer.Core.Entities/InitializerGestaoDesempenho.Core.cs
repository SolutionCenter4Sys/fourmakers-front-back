using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.Colaborador;
using Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.Gestor;
using Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.RH;
using Colaboracao.Infra.Repositories.GestaoPessoa.Pdi;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Colaborador;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using Core.Domain.GestaoPessoa.GestaoDesempenho.RH;
using Core.Domain.GestaoPessoa.Pdi;
using Core.Domain.Usuario.Permissao;
using GestaoPessoa.Domain.Impl.GestaoDesempenho.Colaborador;
using GestaoPessoa.Domain.Impl.GestaoDesempenho.Gestor;
using GestaoPessoa.Domain.Impl.GestaoDesempenho.RH;
using GestaoPessoa.Domain.Impl.Services.Pdi;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Colaborador;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Gestor;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.RH;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddGestaoDesempenhoServicesDependencies(this IServiceCollection services)
        {

            // Validacao
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));

            // Gestor
            services.AddScoped<IGestaoDesempenhoGestorRepository, GestaoDesempenhoGestorRepository>();
            services.AddScoped<IGestaoDesempenhoParametrizacaoRepository, GestaoDesempenhoParametrizacaoRepository>();
            services.AddScopedDomainService<IGestaoDesempenhoGestorService, GestaoDesempenhoGestorService>();

            // Colaborador
            services.AddScoped<IGestaoDesempenhoColaboradorRepository, GestaoDesempenhoColaboradorRepository>();
            services.AddScopedDomainService<IGestaoDesempenhoColaboradorService, GestaoDesempenhoColaboradorService>();

            // RH
            services.AddScoped<IGestaoDesempenhoRHRepository, GestaoDesempenhoRHRepository>();
            services.AddScopedDomainService<IGestaoDesempenhoRHService, GestaoDesempenhoRHService>();

            // PDI (Plano de Desenvolvimento Individual) - movido de Colaborador para GestaoPessoa
            services.TryAddScoped<IBuscaColaboradorRepository, BuscaColaboradorRepository>();
            services.TryAddScoped<IPdiRepository, PdiRepository>();
            services.TryAddScopedDomainService<IMeusPdisService, MeusPdisService>();
            services.TryAddScopedDomainService<IPdisDoTimeService, PdisDoTimeService>();
            services.TryAddScopedDomainService<IPdiMetricasService, PdiMetricasService>();
            services.TryAddScopedDomainService<IPdisRhService, PdisRhService>();
        }
    }
}
