using ApiClient.Domain;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Aws.Infra.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util;
using Colaboracao.Infra.Context;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Usuario.Restricao;
using Core.Domain;
using Core.Domain.Usuario.Restricao;
using MessageQueue.Infra.AWS.SQS.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Colaboracao.Initializer
{
    public static class InitializerCoreExtension
    {
        public static void AddBaseGeralServicosColaboracao(this IServiceCollection services, IConfiguration config)
        {
            ClientConfig.SetConfiguration(ref config);

            JWTAuth.ConfigureJWT(services);

            services.AddDBGColbColaboracaoContext();

            services.AddServicosComunsColaboracao();
            
            services.AddMemoryCache();

            services.AddLocalization();

            services.AddHealthChecks();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = CultureUtil.GetSupportedLanguages();

                options.DefaultRequestCulture = new RequestCulture("pt-BR");

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        public static void AddServicosComunsColaboracao(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILogCore), typeof(LogCore));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.AddScoped(typeof(IAspNetUser), typeof(AspNetUser));
            services.AddScoped(typeof(ITokens), typeof(Tokens));
            services.AddScoped(typeof(IUsuarioClient), typeof(UsuarioClient));
            services.AddScoped(typeof(IDBConnection), typeof(DBConnection));
            services.AddTransient(typeof(IDBConnectionUnitOfWork), typeof(DBConnectionUnitOfWork));
            services.AddScoped(typeof(IMemoryCacheService), typeof(MemoryCacheService));
            services.AddScoped(typeof(IRestricaoDeAcessoService), typeof(RestricaoDeAcessoService));
            services.AddScoped(typeof(IRestricaoDeAcessoRepository), typeof(RestricaoDeAcessoRepository));
            services.AddScoped(typeof(IClassificacaoService), typeof(ClassificacaoService));
            services.AddScoped(typeof(IClassificacaoRepository), typeof(ClassificacaoRepository));
            services.AddScoped(typeof(IClassificacaoClient), typeof(ClassificacaoClient));
            services.AddScoped(typeof(IElasticSearchClient), typeof(ElasticSearchClient));
            
        }

        public static void AddDBGColbColaboracaoContext(this IServiceCollection services, string connectionStringOptional = null)
        {
            string connectionString = connectionStringOptional ?? ConnectionStringCore.DBGColbConnectionString;

            services.AddDbContext<ColaboradorContext>(x => x.UseLazyLoadingProxies().UseMySQL(connectionString));
            services.AddScoped(typeof(ColaboradorContext), typeof(ColaboradorContext));
        }

        public static void ConfigureAppStandard(this IApplicationBuilder app, IWebHostEnvironment env, string projectName)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", projectName + " v1"));

            //prometheus
            app.UseMetricServer("/metrics");
            app.UseHttpMetrics();

            app.UseCors("CorsPolicy");

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            ConfigureAppStandardByAppName(app, projectName);
        }

        public static void UseFluentMessageQueue(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var queueProducer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            AmazonSQSBuilder.ConfigureQueueProducer(queueProducer);
        }

        private static void ConfigureAppStandardByAppName(IApplicationBuilder app, string projectName)
        {
            if (projectName == "SRS.API")
            {
                app.UseFluentMessageQueue();
            }
        }
    }
}