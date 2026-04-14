using ApiClient.Domain.Interfaces;
using ApiClient.Domain.Impl;
using ApiClient.Infra.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Initializer;
using Colaboracao.Infra.Repositories.Labs;
using Core.Domain.Labs;
using Labs.Domain.Impl;
using Labs.Domain.Interfaces;
using Labs.Infra;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Labs.Application
{
    /// <summary>
    /// Configuração de DI para Labs (Domain + Infra).
    /// </summary>
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddServicoLogDBDependencies();

            services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));

            services.AddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.AddScoped<IMatchSemanticoClient, MatchSemanticoClient>();
            services.TryAddScoped<ILabsLogMatchSemanticoRepository, LabsLogMatchSemanticoRepository>();
            services.TryAddScoped<ILabsFeedbackMatchSemanticoRepository, LabsFeedbackMatchSemanticoRepository>();
            services.TryAddScopedDomainService<IMatchSemanticoService, MatchSemanticoService>();
            services.TryAddScopedDomainService<ILabsFeedbackMatchSemanticoService, LabsFeedbackMatchSemanticoService>();

            services.TryAddScoped(typeof(IMatchClient), typeof(MatchClient));
            services.TryAddScoped<ILabsLogRankCandidatesIdsRepository, LabsLogRankCandidatesIdsRepository>();
            services.TryAddScoped<ILabsLogScoreSingleCandidatesRepository, LabsLogScoreSingleCandidatesRepository>();
            services.TryAddScopedDomainService<IMatchService, MatchService>();
            services.TryAddScoped<ILabsLogExtractorExtractVagaRepository, LabsLogExtractorExtractVagaRepository>();
            services.TryAddScopedDomainService<IExtractorService, ExtractorService>();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projectName)
        {
            app.ConfigureAppStandard(env, projectName);
        }
    }
}
