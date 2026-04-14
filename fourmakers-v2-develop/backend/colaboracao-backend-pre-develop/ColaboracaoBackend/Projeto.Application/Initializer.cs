using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Initializer;
using Core.Domain;
using Core.Domain.Projeto;
using Core.Domain.Usuario;
using Core.DomainModel.Projeto;
using Logs.Infra.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Projeto.Domain.Impl.Services;
using Projeto.Domain.Interfaces.Services;

namespace Projeto.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddClienteOrgCrudServicesDependencies();
            services.AddBuscaParametroConfiguracaoServicesDependencies();

            services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
            services.AddScopedDomainService<IAtividadeProjetoService, AtividadeProjetoService>();
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScopedDomainService<IClienteOrgService, ClienteOrgService>();
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(IEmpresaRepository), typeof(EmpresaRepository));
            services.AddScoped(typeof(IEnvioEmail), typeof(EnvioEmail));
            services.AddScoped(typeof(IProjetoOrgRepository), typeof(ProjetoOrgRepository));
            services.AddScopedDomainService<IProjetoOrgService, ProjetoOrgService>();
            services.AddScoped(typeof(IStatusProjetoRepository), typeof(StatusProjetoRepository));
            services.AddScopedDomainService<IStatusProjetoService, StatusProjetoService>();
            services.AddScoped(typeof(ITokenUsuarioRepository), typeof(TokenUsuarioRepository));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorRepository));
            services.AddScoped(typeof(IUsuarioFourmakerRepository), typeof(UsuarioFourmakerRepository));
            services.AddScoped(typeof(IVerificaSeCpfESistemico), typeof(VerificaSeCpfESistemico));
            services.AddScoped(typeof(IProjetoRelatorioRepository), typeof(ProjetoRelatorioRepository));
            services.AddScopedDomainService<IExtracaoProjetoService, ExtracaoProjetoService>();
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}