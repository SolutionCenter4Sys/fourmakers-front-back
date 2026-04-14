using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.DadosPerfil;
using Colaboracao.Infra.Repositories.Fourmakers;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Initializer;
using Colaborador.Domain.Impl.Services;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.DomainModel;
using Core.Domain.DadosPerfil;
using Core.Domain.Fourmakers;
using Core.Domain.FourmakersLead;
using Core.DomainModel.Usuario;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foursys.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddBuscaParametroConfiguracaoServicesDependencies();
            services.AddParametroConfiguracaoServicesDependencies();

            services.AddScoped(typeof(ICCHClient), typeof(CCHClient));
            services.AddScoped(typeof(ICepClient), typeof(CepClient));
            services.AddScoped(typeof(IDadosPerfilRepository), typeof(DadosPerfilRepository));
            services.AddScopedDomainService<IDadosPerfilService, DadosPerfilService>();
            services.AddScoped(typeof(IEnvioEmail), typeof(EnvioEmail));
            services.AddScoped(typeof(IFoursysDtoRepository), typeof(FoursysRepository));
            services.AddScopedDomainService<IFoursysService, FoursysService>();
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUsuarioRelatorioRepository), typeof(UsuarioRelatorioRepository));
            services.AddScoped(typeof(IFourmakersLeadsRepository), typeof(FourmakersLeadsRepository));
            services.AddScopedDomainService<IFourmakersLeadsService, FourmakersLeadsService>();
            services.AddScoped<IConnectionStringCore, ConnectionStringCore>();
            services.AddScoped<IFourmakersRepository, FourmakersRepository>();
            services.AddScoped<IFourmakersService, FourmakersService>();
            services.AddQuestionarioServicesDependencies();
           
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}