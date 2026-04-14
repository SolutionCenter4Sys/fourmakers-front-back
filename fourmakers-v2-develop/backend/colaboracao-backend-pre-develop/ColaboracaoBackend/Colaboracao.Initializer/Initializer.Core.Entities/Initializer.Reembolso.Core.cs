using Amazon.S3;
using ApiClient.Domain.Impl;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Impl.S3;
using Aws.Infra.Interfaces.S3;
using ApiClient.Infra.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.Financeiro.Reembolso.ControleDeSaldo;
using Colaboracao.Infra.Repositories.Financeiro.Reembolso.Parametro;
using Colaboracao.Infra.Repositories.Financeiro.Reembolso.Solicitacao;
using Colaboracao.Infra.Repositories.Financeiro.Reembolso.Verba;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Fourmakers;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Core.Domain.ParametroOrg;
using Core.DomainModel;
using Core.Domain.Reembolso.ControleDeSaldo;
using Core.Domain.Reembolso.Parametro;
using Core.Domain.Reembolso.Solicitacao;
using Core.Domain.Reembolso.Verba;
using Core.Domain.Usuario.Permissao;
using Financeiro.Domain.Interfaces.Reembolso.Parametro;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Financeiro.Domain.Services.Reembolso.Parametro;
using Financeiro.Domain.Services.Reembolso.Solicitacao;
using Financeiro.Domain.Services.Reembolso.Validadores;
using Financeiro.Domain.Services.Reembolso.Verba;
using Microsoft.Extensions.DependencyInjection;
using Financeiro.Domain.Services.Verba;
using Core.Domain.Usuario;
using Colaboracao.Infra.Repositories.Usuario;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Extensions;

namespace Colaboracao.Initializer;

public static partial class InitializerExtension
{
    public static void AddReembolsoDependencies(this IServiceCollection services, bool transient = false)
    {
        // Reembolso: comprovante permanece na chave S3 do upload temp; uploader usado p/ rollback e rotinas S3
        services.AddTransient(typeof(IAmazonS3), typeof(AmazonS3Client));
        services.AddTransient(typeof(IAmazonS3Uploader), typeof(AmazonS3Uploader));

        if (transient)
        {
            services.AddTransient<IVerbaService, VerbaService>();
            services.AddTransient<IVerbaRepository, VerbaRepository>();
            services.AddTransient<IVerbaLogService, VerbaLogService>();
            services.AddTransient<IVerbaLogRepository, VerbaLogRepository>();
            services.AddTransient<IVerbaTipoRepository, VerbaTipoRepository>();
            services.AddTransient<IVerbaTipoService, VerbaTipoService>();
            services.AddTransient<IParametroReembolsoService, ParametroReembolsoService>();
            services.AddTransient<IParametroReembolsoRepository, ParametroReembolsoRepository>();
            services.AddTransient<IVerbaValidadorService, VerbaValidadorService>();
            services.AddTransient<IParametroReembolsoValidadorService, ParametroReembolsoValidadorService>();
            services.AddTransient<ISolicitacaoReembolsoRepository, SolicitacaoReembolsoRepository>();
            services.AddTransient<ISolicitacaoReembolsoService, SolicitacaoReembolsoService>();
            services.AddTransient<ISolicitacaoReembolsoValidadorService, SolicitacaoReembolsoValidadorService>();
            services.AddTransient<ISolicitacaoStatusRepository, SolicitacaoStatusRepository>();
            services.AddTransient<ISolicitacaoStatusService, SolicitacaoStatusService>();
            services.AddTransient<ISolicitacaoExternoService, SolicitacaoExternoService>();
            services.AddTransient<ISolicitacaoReembolsoDocumentoRepository, SolicitacaoReembolsoDocumentoRepository>();
            services.AddTransient<ISolicitacaoReembolsoDocumentoService, SolicitacaoReembolsoDocumentoService>();
            services.AddTransient<ITokenFileTempRepository, TokenFileTempRepository>();
            services.AddTransient<IUploadFilesClient, UploadFilesClient>();
            services.AddTransient<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.AddTransient<IConnectionStringCore, ConnectionStringCore>();
            services.AddTransient<IIAClient, IAClient>();
            services.AddTransient<IApiClient, ApiClient.Infra.Impl.ApiClient>();
            services.AddTransient<IVerbaPersonalizadaService, VerbaPersonalizadaService>();
            services.AddTransient<IVerbaPersonalizadaRepository, VerbaPersonalizadaRepository>();
            services.AddTransient<IVerbaPersonalizadaValidadorService, VerbaPersonalizadaValidadorService>();
            services.AddTransient<IControleDeSaldoRepository, ControleDeSaldoRepository>();
            services.AddTransient<ITokenSistemaService, TokenSistemaService>();
            services.AddTransient<ITokenSistemaRepository, TokenSistemaRepository>();
            services.AddTransient<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
            services.AddTransient<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();
        }
        else
        {
            services.AddScopedDomainService<IVerbaService, VerbaService>();
            services.AddScoped<IVerbaRepository, VerbaRepository>();
            services.AddScopedDomainService<IVerbaLogService, VerbaLogService>();
            services.AddScoped<IVerbaLogRepository, VerbaLogRepository>();
            services.AddScoped<IVerbaTipoRepository, VerbaTipoRepository>();
            services.AddScopedDomainService<IVerbaTipoService, VerbaTipoService>();
            services.AddScopedDomainService<IParametroReembolsoService, ParametroReembolsoService>();
            services.AddScoped<IParametroReembolsoRepository, ParametroReembolsoRepository>();
            services.AddScoped<IVerbaValidadorService, VerbaValidadorService>();
            services.AddScoped<IParametroReembolsoValidadorService, ParametroReembolsoValidadorService>();
            services.AddScoped<ISolicitacaoReembolsoRepository, SolicitacaoReembolsoRepository>();
            services.AddScopedDomainService<ISolicitacaoReembolsoService, SolicitacaoReembolsoService>();
            services.AddScoped<ISolicitacaoReembolsoValidadorService, SolicitacaoReembolsoValidadorService>();
            services.AddScoped<ISolicitacaoStatusRepository, SolicitacaoStatusRepository>();
            services.AddScopedDomainService<ISolicitacaoStatusService, SolicitacaoStatusService>();
            services.AddScopedDomainService<ISolicitacaoExternoService, SolicitacaoExternoService>();
            services.AddScoped<ISolicitacaoReembolsoDocumentoRepository, SolicitacaoReembolsoDocumentoRepository>();
            services.AddScopedDomainService<ISolicitacaoReembolsoDocumentoService, SolicitacaoReembolsoDocumentoService>();
            services.AddScoped<ITokenFileTempRepository, TokenFileTempRepository>();
            services.AddScoped<IUploadFilesClient, UploadFilesClient>();
            services.AddScoped<IFuncionalidadeSistemaRepository, FuncionalidadeSistemaRepository>();
            services.AddScoped<IConnectionStringCore, ConnectionStringCore>();
            services.AddScoped<IIAClient, IAClient>();
            services.AddScoped<IApiClient, ApiClient.Infra.Impl.ApiClient>();
            services.AddScopedDomainService<IVerbaPersonalizadaService, VerbaPersonalizadaService>();
            services.AddScoped<IVerbaPersonalizadaRepository, VerbaPersonalizadaRepository>();
            services.AddScoped<IVerbaPersonalizadaValidadorService, VerbaPersonalizadaValidadorService>();
            services.AddScoped<IControleDeSaldoRepository, ControleDeSaldoRepository>();
            services.AddScoped<ITokenSistemaService, TokenSistemaService>();
            services.AddScoped<ITokenSistemaRepository, TokenSistemaRepository>();
            services.AddScoped<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
            services.AddScoped<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();
        }
    }
}


