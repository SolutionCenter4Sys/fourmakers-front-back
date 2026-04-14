using ApiClient.Domain;
using Colaboracao.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using RotinasBackoffice.Application;

namespace RotinasBackoffice.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Initializer.Configure(services, Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddHostedService<VagasSRSSchedule>();
            // services.AddHostedService<RotinaVaga>();
            services.AddHostedService<RotinaGeraCargaSincronizaCRM>();
            services.AddHostedService<RotinaNotificacaoContratosVencidos>();
            services.AddHostedService<RotinasMarketingComunicacao>();

            if (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "PRD")
            {
                services.AddHostedService<RotinaEmailApontamentoExcedente>();
                services.AddHostedService<RotinaEmailColaborador>();
                services.AddHostedService<RotinaGeraCargaColaboradorHierarquia>();
                services.AddHostedService<RotinaGeraCargaColaborador>();
                services.AddHostedService<RotinaGeraCargaProjeto>();
                services.AddHostedService<RotinaLimpezaAws>();
            }

            // services.AddHostedService<CargaDeColaboradoresLGSchedule>();
            // services.AddHostedService<SolicitacaoDeArquivosDeHoleritesSchedule>();
            // services.AddHostedService<DownloadDeHoleritesSchedule>();
            // services.AddHostedService<ExpurgoDeSolicitacaoDeHoleritesRejeitadasSchedule>();

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMetricServer();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}