using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Publicacao;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Comunidade;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Grupo;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Configuracao;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Profissionais;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Label;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Tag;
using Colaboracao.Infra.Repositories.Marketing.Comunicacao.Analytics;
using Core.Domain.Marketing.Comunicacao.Publicacao;
using Core.Domain.Marketing.Comunicacao.Comunidade;
using Core.Domain.Marketing.Comunicacao.Grupo;
using Core.Domain.Marketing.Comunicacao.Configuracao;
using Core.Domain.Marketing.Comunicacao.Profissionais;
using Core.Domain.Marketing.Comunicacao.Label;
using Core.Domain.Marketing.Comunicacao.Tag;
using Core.Domain.Marketing.Comunicacao.Analytics;
using Marketing.Domain.Impl.Comunicacao.Publicacao;
using Marketing.Domain.Impl.Comunicacao.Comunidade;
using Marketing.Domain.Impl.Comunicacao.Grupo;
using Marketing.Domain.Impl.Comunicacao.PublicacaoGerencial;
using Marketing.Domain.Impl.Comunicacao.Configuracao;
using Marketing.Domain.Impl.Comunicacao.Profissionais;
using Marketing.Domain.Impl.Comunicacao.Label;
using Marketing.Domain.Impl.Comunicacao.Tag;
using Marketing.Domain.Impl.Comunicacao.IA;
using Marketing.Domain.Impl.Comunicacao.Analytics;
using Marketing.Domain.Interfaces.Comunicacao.Publicacao;
using Marketing.Domain.Interfaces.Comunicacao.Comunidade;
using Marketing.Domain.Interfaces.Comunicacao.Grupo;
using Marketing.Domain.Interfaces.Comunicacao.PublicacaoGerencial;
using Marketing.Domain.Interfaces.Comunicacao.Configuracao;
using Marketing.Domain.Interfaces.Comunicacao.Profissionais;
using Marketing.Domain.Interfaces.Comunicacao.Label;
using Marketing.Domain.Interfaces.Comunicacao.Tag;
using Marketing.Domain.Interfaces.Comunicacao.IA;
using Marketing.Domain.Interfaces.Comunicacao.Analytics;
using ApiClient.Domain.Interfaces;
using ApiClient.Domain.Impl;
using Logs.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddMarketingComunicacaoServicesDependencies(this IServiceCollection services)
        {
            // Upload de arquivos (AWS S3)
            services.AddScoped<IUploadFilesClient, UploadFilesClient>();

            // Publicacao
            services.AddScoped<IComunicacaoPublicacaoRepository, ComunicacaoPublicacaoRepository>();
            services.AddScopedDomainService<IComunicacaoPublicacaoService, ComunicacaoPublicacaoService>();

            // Comunidade
            services.AddScoped<IComunicacaoComunidadeRepository, ComunicacaoComunidadeRepository>();
            services.AddScopedDomainService<IComunicacaoComunidadeService, ComunicacaoComunidadeService>();

            // Grupo (requer IFuncionalidadeSistemaRepository para validar GESTAO_COMUNICADOS ao criar grupo)
            services.TryAddScoped<IConnectionStringCore, ConnectionStringCore>();
            services.TryAddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.AddScoped<IComunicacaoGrupoRepository, ComunicacaoGrupoRepository>();
            services.AddScopedDomainService<IComunicacaoGrupoService, ComunicacaoGrupoService>();

            // Publicacao Gerencial (usa o mesmo repositorio de Publicacao)
            services.AddScopedDomainService<IComunicacaoPublicacaoGerencialService, ComunicacaoPublicacaoGerencialService>();

            // Configuracao
            services.AddScoped<IComunicacaoConfiguracaoRepository, ComunicacaoConfiguracaoRepository>();
            services.AddScopedDomainService<IComunicacaoConfiguracaoService, ComunicacaoConfiguracaoService>();

            // Profissionais
            services.AddScoped<IComunicacaoProfissionaisRepository, ComunicacaoProfissionaisRepository>();
            services.AddScopedDomainService<IComunicacaoProfissionaisService, ComunicacaoProfissionaisService>();

            // Label
            services.AddScoped<IComunicacaoLabelRepository, ComunicacaoLabelRepository>();
            services.AddScopedDomainService<IComunicacaoLabelService, ComunicacaoLabelService>();

            // Tag (somente publicações tipo documento)
            services.AddScoped<IComunicacaoTagRepository, ComunicacaoTagRepository>();
            services.AddScopedDomainService<IComunicacaoTagService, ComunicacaoTagService>();

            // IA (assistente texto - usa CurriculoClient / API analise-documental)
            services.TryAddScoped<ICurriculoClient, CurriculoClient>();
            services.AddScopedDomainService<IComunicacaoIAService, ComunicacaoIAService>();

            // Analytics
            services.AddScoped<IComunicacaoAnalyticsRepository, ComunicacaoAnalyticsRepository>();
            services.AddScopedDomainService<IComunicacaoAnalyticsService, ComunicacaoAnalyticsService>();
        }
    }
}
