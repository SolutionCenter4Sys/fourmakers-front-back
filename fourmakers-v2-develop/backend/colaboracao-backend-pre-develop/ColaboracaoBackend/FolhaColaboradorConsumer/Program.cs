using FolhaColaboradorConsumer;
using Amazon.SQS;
//using CurriculoBatchConsumer.Services;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Impl.Services;
using Colaboracao.Initializer;
using Aws.Infra.Interfaces;
using Aws.Infra.Impl;
using Microsoft.AspNetCore.Builder;
using Core.Domain.Apontamento;
using Core.Domain.Usuario;
using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Infra.Repositories.Apontamento;
using Colaboracao.Infra.Repositories.Lote;
using Colaboracao.Infra.Repositories.Usuario;
using Apontamento.Domain.Impl.Service;
using Core.Domain.Projeto;
using Colaboracao.Infra.Repositories.Projeto;
using Core.Domain.Usuario.Permissao;
using Colaboracao.Infra.Repositories.Usuario.Permissao;
using Firebase.Domain.Interfaces.Services;
using Firebase.Domain.Impl.Services;
using Core.Domain.Notificacao;
using Colaboracao.Infra.Repositories.Notificacao;
using Apontamento.Domain.Interfaces;
using Apontamento.Domain.Impl.Service.Validador;
using Microsoft.Extensions.Caching.Memory;

var builder = Host.CreateApplicationBuilder(args);

var region = builder.Configuration["AWS:Region"];
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IAmazonSQS>(sp =>
            new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region)));
//builder.Services.AddScoped<IService, Service>();
builder.Services.AddScoped<IQueueProducer, AmazonSQSProducer>();
builder.Services.AddScoped(typeof(IFolhaPontoRepository), typeof(FolhaPontoRepository));
builder.Services.AddScoped(typeof(ILoteRepository), typeof(LoteRepository));
builder.Services.AddScoped(typeof(IUsuarioColaboradorRepository), typeof(UsuarioColaboradorDapperRepository));
builder.Services.AddScoped(typeof(IAtividadeProjetoRepository), typeof(AtividadeProjetoRepository));
builder.Services.AddScoped(typeof(IFolhaPontoService), typeof(FolhaPontoService));
builder.Services.AddScoped(typeof(IApontamentoService), typeof(ApontamentoService));
builder.Services.AddScoped(typeof(IApontamentoRepository), typeof(ApontamentoRepository));
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton(typeof(IApontamentoCacheService), typeof(ApontamentoCacheService));
builder.Services.AddScoped(typeof(IFuncionalidadeSistemaRepository), typeof(FuncionalidadeSistemaRepository));
builder.Services.AddScoped(typeof(IPeriodoFechadoRepository), typeof(PeriodoFechadoRepository));
builder.Services.AddScoped(typeof(IPeriodoFechadoService), typeof(PeriodoFechadoService));
builder.Services.AddScoped(typeof(INotificacaoService), typeof(NotificacaoService));
builder.Services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
builder.Services.AddScoped<IApontamentoValidadorService, ApontamentoValidadorService>();

builder.Services.AddColaboradorServicesDependencies();
builder.Services.AddTemplateRepositoryDependencies();
builder.Services.AddHistoricoCVDependencies();
builder.Services.AddCidadaniaServicesDependencies();
builder.Services.AddTemplateRepositoryDependencies();
builder.Services.AddBuscaParametroConfiguracaoServicesDependencies();
builder.Services.AddReembolsoDependencies();
builder.Services.AddRubricaServicesDependencies();
builder.Services.AddNotaFiscalServicesDependencies();
builder.Services.AddHoleriteServicesDependencies();

builder.Services.AddBaseGeralServicosColaboracao(builder.Configuration);

var host = builder.Build();

Task.Run(() =>
{
    var builder = WebApplication.CreateBuilder();
    builder.Services.AddHealthChecks();

    var app = builder.Build();
    app.MapHealthChecks("/health");

    app.Run();

});

host.Run();
