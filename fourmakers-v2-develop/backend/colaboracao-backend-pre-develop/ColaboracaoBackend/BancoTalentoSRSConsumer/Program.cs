using BancoTalentoSRSConsumer;
using Amazon.SQS;
using Amazon.S3;
using Colaboracao.Initializer;
using Aws.Infra.Interfaces;
using Aws.Infra.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Infra.Repositories;
using Core.DomainModel;
using Microsoft.AspNetCore.Builder;
using Core.Domain;
using Colaboracao.Infra.Repositories.Log;
using SRS.Application;

var builder = Host.CreateApplicationBuilder(args);

var region = builder.Configuration["AWS:Region"];
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IAmazonSQS>(sp =>
            new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region)));
builder.Services.AddSingleton<IAmazonS3>(sp =>
            new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region)));

builder.Services.AddScoped<IQueueProducer, AmazonSQSProducer>();
builder.Services.AddScoped<ITokenFileRepository, TokenFileRepository>();
builder.Services.AddScoped<IUploadFiles, UploadFiles>();

// Registrar o repository específico do Banco de Talentos SRS
builder.Services.AddScoped<ILogBancoTalentoSRSRepository, LogBancoTalentoSRSRepository>();

// Adicionar as mesmas injeções de dependência do Colaborador.Application/Initializer.cs
builder.Services.AddBaseGeralServicosColaboracao(builder.Configuration);
builder.Services.AddColaboradorServicesDependencies();
builder.Services.AddTemplateRepositoryDependencies();
builder.Services.AddHistoricoCVDependencies();
builder.Services.AddCidadaniaServicesDependencies();

// Adiciona dependências do SRS (inclui ISRSCandidateService, ISRSInfraClient, etc.)
new Initializer().ConfigureServices(builder.Services, builder.Configuration);

var host = builder.Build();

// Health check endpoint similar ao FolhaColaboradorConsumer
Task.Run(() =>
{
    var webBuilder = WebApplication.CreateBuilder();
    webBuilder.Services.AddHealthChecks();

    var app = webBuilder.Build();
    app.MapHealthChecks("/health");

    app.Run();
});

host.Run();
