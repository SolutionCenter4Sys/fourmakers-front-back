using Colaboracao.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Colaborador.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);

            services.AddColaboradorServicesDependencies();
            services.AddTemplateRepositoryDependencies();
            services.AddHistoricoCVDependencies();
            services.AddCidadaniaServicesDependencies();     
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string appName)
        {
            app.ConfigureAppStandard(env, appName);
        }
    }
}