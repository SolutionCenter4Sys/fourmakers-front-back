using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Colaboracao.Infra;
using Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.MapaAlocacao.MapaAlocacao.GestaoDeAlocados;
using Colaboracao.Infra.Repositories.SRS;
using Colaboracao.Infra.Repositories.Social;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain;
using Core.Domain.SRS;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Social;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.MapaAlocacao.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfil;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfilSkill;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.MinhaEquipeValidaAcesso;
using MapaDeAlocacao.Domain.Impl.Perfil;
using MapaDeAlocacao.Domain.Impl.Perfil.PerfilValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.MinhaEquipeValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.Perfil;
using MapaDeAlocacao.Domain.Interfaces.Perfil.PerfilValidaAcesso;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Projeto.Domain.Impl.Services;
using Projeto.Domain.Interfaces.Services;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddGestaoDeAlocadosServicesDependencies(this IServiceCollection services)
        {
            services.TryAddScopedDomainService<IGestorExternoService, GestorExternoService>();
            services.TryAddScoped<IGestorExternoValidatorService, GestorExternoValidatorService>();
            services.TryAddScoped<IGestorExternoRepository, GestorExternoRepository>();

            services.TryAddScopedDomainService<IGestorExternoPerfilService, GestorExternoPerfilService>();
            services.AddTransient<IComentarioCandidaturaRepository, ComentarioCandidaturaRepository>();
            services.TryAddScoped<IGestorExternoPerfilValidatorService, GestorExternoPerfilValidatorService>();
            services.TryAddScoped<IGestorExternoPerfilRepository, GestorExternoPerfilRepository>();
            services.TryAddScoped<IAdmissaoCargoRepository, AdmissaoCargoRepository>();

            services.TryAddScopedDomainService<IGestorExternoPerfilSkillService, GestorExternoPerfilSkillService>();
            services.TryAddScoped<IGestorExternoPerfilSkillValidatorService, GestorExternoPerfilSkillValidatorService>();
            services.TryAddScoped<IGestorExternoPerfilSkillRepository, GestorExternoPerfilSkillRepository>();

            services.TryAddScopedDomainService<IGestaoAlocadosService, GestaoAlocadosService>();
            services.TryAddScoped<IGestaoAlocadosValidarAcessoService, GestaoAlocadosValidarAcessoService>();
            services.TryAddScoped<IGestaoAlocadosRepository, GestaoAlocadosRepository>();
            services.TryAddScoped<IMinhaEquipeValidarAcessoService, MinhaEquipeValidarAcessoService>();
            services.TryAddScoped<IPricingClient, PricingClient>();

            services.TryAddScopedDomainService<IAreaAtuacaoService, AreaAtuacaoService>();
            services.TryAddScoped<IAreaAtuacaoRepository, AreaAtuacaoRepository>();

            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.TryAddScoped<IPerfilAlocacaoRepository, PerfilAlocacaoRepository>();

            services.TryAddScopedDomainService<IAderenciaService, AderenciaService>();
            services.TryAddScoped<IAderenciaRepository, AderenciaRepository>();

            services.TryAddScopedDomainService<IPerfilService, PerfilService>();
            services.TryAddScoped<IPerfilValidarAcessoService, PerfilValidarAcessoService>();
            services.TryAddScoped<IPerfilRepository, PerfilRepository>();
        }
    }
}