using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Colaborador;
using Colaboracao.Infra.Repositories.Competencia;
using Colaboracao.Infra.Repositories.Competencia.DashboardMinhaJornada;
using Colaboracao.Infra.Repositories.Competencia.Dominio;
using Colaboracao.Infra.Repositories.Competencia.MapaCompetencia;
using Colaboracao.Infra.Repositories.Formacao;
using Colaboracao.Infra.Repositories.Idioma;
using Colaboracao.Infra.Repositories.Projeto;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Colaboracao.Initializer;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Impl.Services.DashboardMinhaJornada;
using Competencia.Domain.Impl.Services.Metodologia;
using Competencia.Domain.Interfaces.Services;
using Competencia.Domain.Interfaces.Services.DashboardMinhaJornada;
using Competencia.Domain.Interfaces.Services.Metodologia;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Competencia;
using Core.Domain.Competencia.DashboardMinhaJornada;
using Core.Domain.Competencia.Metodologia;
using Core.Domain.Dominio;
using Core.Domain.Formacao;
using Core.Domain.IIdioma;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using Core.DomainModel.Competencia;
using Core.DomainModel.Projeto;
using Core.DomainModel.Softskill;
using Formacao.Domain.Impl.Services;
using Formacao.Domain.Interfaces.Services;
using Logs.Infra.Extensions;
using MapaDeAlocacao.Domain.Impl.GestaoDeHabilidades;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;

namespace Competencia.Application
{
    public class Initializer : IInitializerConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddBaseGeralServicosColaboracao(config);
            services.AddHistoricoCVDependencies();

            services.AddScoped(typeof(ICompetenciaDtoRepository), typeof(Colaboracao.Infra.Repositories.CompetenciaRepository));
            services.AddScopedDomainService<ICompetenciaService, CompetenciaService>();
            services.AddScopedDomainService<ICompetenciaHistoricoService, CompetenciaHistoricoService>();
            services.AddScoped(typeof(ICompetenciaHistoricoRepository), typeof(CompetenciaHistoricoRepository));
            services.AddScopedDomainService<IHardSkillService, HardSkillService>();
            services.AddScoped(typeof(IHardSkillRepository), typeof(HardSkillRepository));
            // CompetenciaDomainFactory removed - business logic moved to services
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IColaboradorClient), typeof(ColaboradorClient));
            services.AddScoped(typeof(IFirebaseClient), typeof(FirebaseClient));
            services.AddScoped(typeof(IUploadFilesClient), typeof(UploadFilesClient));
            services.AddScoped(typeof(IFoursysClient), typeof(FoursysClient));
            services.AddScoped(typeof(IVerificaSeCpfESistemico), typeof(VerificaSeCpfESistemico));
            services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
            services.AddScoped(typeof(IConnectionStringCore), typeof(ConnectionStringCore));
            services.AddScopedDomainService<IMapaCompetenciaService, MapaCompetenciaService>();
            services.AddScoped(typeof(IFormacaoRepository), typeof(FormacaoRepository));
            services.AddScoped(typeof(IFormacaoGenericoRepository), typeof(FormacaoGenericoRepository));
            services.AddScoped(typeof(IEstatisticasRepository), typeof(EstatisticasRepository));
            services.AddScoped(typeof(IClienteOrgRepository), typeof(ClienteOrgRepository));
            services.AddScoped(typeof(IMapaCompetenciaRepository), typeof(MapaCompetenciaRepository));
            //Soft skill
            services.AddScoped(typeof(ISoftskillRepository), typeof(SoftskillRepository));
            services.AddScoped(typeof(ISoftskillColaboradorRepository), typeof(SoftskillColaboradorRepository));
            services.AddScopedDomainService<ISoftskillService, SoftskillService>();
            services.AddScoped(typeof(ISoftskillNivelRepository), typeof(SoftskillNivelRepository));
            // Dominio
            services.AddScoped(typeof(IDominioRepository), typeof(DominioRepository));
            services.AddScopedDomainService<IDominioService, DominioService>();
            //Hobby
            //services.AddScoped(typeof(IHobbyDomainFactory), typeof(HobbyDomainFactory));
            //services.AddScoped(typeof(IHobbyService), typeof(HobbyService));
            //services.AddScoped(typeof(IRepository<IHobbyColaboradorModel, IHobbyDomainFactory>), typeof(HobbyColaboradorRepository));
            //services.AddScoped(typeof(IRepository<IHobbyModel, IHobbyDomainFactory>), typeof(HobbyRepository));
            //Interesse
            services.AddScopedDomainService<IInteresseService, InteresseService>();
            services.AddScoped(typeof(IInteresseDtoRepository), typeof(InteresseRepository));
            services.AddScoped(typeof(IInteresseColaboradorDtoRepository), typeof(InteresseColaboradorRepository));
            // Idioma
            services.AddScoped(typeof(IIdiomaColaboradorRepository), typeof(IdiomaColaboradorRepository));
            services.AddScoped(typeof(IIdiomaNivelRepository), typeof(IdiomaNivelRepository));
            services.AddScoped(typeof(IIdiomaRepository), typeof(IdiomaRepository));
            services.AddScoped(typeof(IIdiomaRepositoryGenerico), typeof(IdiomaRepositoryGenerico));
            services.AddScopedDomainService<IIdiomaService, IdiomaService>();
            //Metodologia
            services.AddScopedDomainService<IMetodologiasService, MetodologiasService>();
            services.AddScoped(typeof(IMetodologiasRepository), typeof(Colaboracao.Infra.Repositories.Competencia.MetodologiasRepository));

            services.AddScopedDomainService<IFormacaoService, FormacaoService>();
            services.AddScoped(typeof(IFormacaoRepository), typeof(FormacaoRepository));
            services.AddScoped(typeof(IFormacaoGenericoRepository), typeof(FormacaoGenericoRepository));
            services.AddScoped(typeof(ICompetenciaColaboradorRepository), typeof(Colaboracao.Infra.Repositories.Competencia.CompetenciaColaboradorRepository));
            services.AddScoped(typeof(IPerfilAlocacaoRepository), typeof(PerfilAlocacaoRepository));

            //GestaoDeCompetencia
            services.AddScoped(typeof(IGestaoDeCompetenciaRepository), typeof(GestaoDeCompetenciaRepository));
            services.AddScopedDomainService<IGestaoDeCompetenciaService, GestaoDeCompetenciaService>();
            services.AddScoped(typeof(ISRSClient), typeof(SRSClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));

            services.AddScopedDomainService<ISkillDesconhecidaService, SkillDesconhecidaService>();
            services.AddScoped(typeof(ISkillDesconhecidaColaboradorRepository), typeof(SkillDesconhecidaColaboradorRepository));
            services.AddScoped(typeof(ISkillDesconhecidaRepository), typeof(SkillDesconhecidaRepository));

            services.AddScopedDomainService<ISkillGenericService, SkillGenericService>();
            services.AddScoped(typeof(IColaboradorPerfilRepository), typeof(ColaboradorPerfilRepository));

            services.AddScoped(typeof(IDashboardMinhaJornadaService), typeof(DashboardMinhaJornadaService));
            services.AddScoped(typeof(IDashboardMinhaJornadaRepository), typeof(DashboardMinhaJornadaRepository));
        }

        public void ConfigureAppStandard(IApplicationBuilder app, IWebHostEnvironment env, string projetcName)
        {
            app.ConfigureAppStandard(env, projetcName);
        }
    }
}