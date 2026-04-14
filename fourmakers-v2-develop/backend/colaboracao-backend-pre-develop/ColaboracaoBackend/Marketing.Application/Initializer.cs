using Colaboracao.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddServicosComunsColaboracao();
            services.AddMarketingComunicacaoServicesDependencies();
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}
