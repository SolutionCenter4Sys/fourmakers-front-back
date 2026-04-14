using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Aws.Infra.Impl;
using Aws.Infra.Interfaces;
using BI.Domain.Impl.Services;
using BI.Domain.Interfaces.Services;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.BI;
using Colaboracao.Infra.Repositories.Competencia;
using Colaboracao.Infra.Repositories.Competencia.Dominio;
using Colaboracao.Infra.Repositories.Idioma;
using Colaboracao.Infra.Repositories.MapaAlocacao;
using Colaboracao.Infra.Repositories.Org;
using Colaboracao.Infra.Repositories.Usuario;
using Colaboracao.Initializer;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Logs.Infra.Extensions;
using Core.Domain.Apontamento;
using Core.Domain.BI;
using Core.Domain.Competencia.Metodologia;
using Core.Domain.Dominio;
using Core.Domain.IIdioma;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario;
using Core.DomainModel.Org;
using Core.DomainModel.Softskill;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using SRS.Infra.Impl;
using SRS.Infra.Interfaces;

namespace BI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBaseGeralServicosColaboracao(Configuration);
            
            services.AddTokenSistemaDependencies();

            services.AddScoped<IConnectionStringCore, ConnectionStringCore>();
            services.AddScoped<IApontamentoRepository, ApontamentoRepository>();
            services.AddScoped<IExtracaoAlocacaoRepository, ExtracaoAlocacaoRepository>();
            services.AddScoped<IColaboradorBIRepository, ColaboradorBIRepository>();
            services.AddScoped<IEscolaridadeColaboradorBIRepository, EscolaridadeColaboradorBIRepository>();
            services.AddScoped<IExperienciaProfissionalBIRepository, ExperienciaProfissionalBIRepository>();
            services.AddScoped<IOrgRepository, OrgRepository>();

            services.AddScoped(typeof(IApiClient), typeof(ApiClient.Infra.Impl.ApiClient));
            services.AddScoped(typeof(ISRSInfraClient), typeof(SRSInfraClient));
            services.AddScoped(typeof(ISRSColaboracaoClient), typeof(SRSColaboracaoClient));

            services.AddScoped(typeof(IHardSkillRepository), typeof(HardSkillRepository));
            services.AddScoped(typeof(ISoftskillRepository), typeof(SoftskillRepository));
            services.AddScoped(typeof(ISoftskillNivelRepository), typeof(SoftskillNivelRepository));
            services.AddScoped(typeof(IMetodologiasRepository), typeof(MetodologiasRepository));
            services.AddScoped(typeof(IDominioRepository), typeof(DominioRepository));
            services.AddScoped(typeof(IIdiomaRepository), typeof(IdiomaRepository));
            services.AddScoped(typeof(IIdiomaNivelRepository), typeof(IdiomaNivelRepository));
            services.AddSingleton<IAwsCacheService, AwsCacheServiceImpl>();

            services.AddScopedDomainService<IBIService, BIService>();
            services.AddScoped<IMapaDeAlocacaoClient, MapaDeAlocacaoClient>();
            services.AddScoped<IMapaAlocacaoBIService, MapaAlocacaoBIService>();
            services.AddScoped<IMapaAlocacaoRepository, MapaAlocacaoRepository>();
            services.AddScoped<IBICompetenciaRepository, BICompentenciaRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BI.API", Version = "v1" });

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "bearer"
                              }
                          },
                         new string[] {}
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BI.API v1"));
            }

            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}