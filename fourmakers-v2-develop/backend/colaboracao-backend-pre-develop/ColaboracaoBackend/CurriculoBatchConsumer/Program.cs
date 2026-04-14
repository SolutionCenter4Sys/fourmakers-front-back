using CurriculoBatchConsumer;
using Amazon.SQS;
using Amazon.S3;
using CurriculoBatchConsumer.Services;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Impl.Services;
using Colaboracao.Initializer;
using Aws.Infra.Interfaces;
using Aws.Infra.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Core.Impl;
using Colaboracao.Infra.Repositories;
using Colaboracao.Infra.Repositories.Curriculo;
using Core.DomainModel;
using Core.Domain.Curriculo;
using Microsoft.AspNetCore.Builder;

var builder = Host.CreateApplicationBuilder(args);

var region = builder.Configuration["AWS:Region"];
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IAmazonSQS>(sp =>
            new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region)));
builder.Services.AddSingleton<IAmazonS3>(sp =>
            new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region)));
builder.Services.AddScoped<IService, Service>();
builder.Services.AddScoped<IQueueProducer, AmazonSQSProducer>();
builder.Services.AddScoped<ITokenFileRepository, TokenFileRepository>();
builder.Services.AddScoped<IUploadFiles, UploadFiles>();
builder.Services.AddScoped<ICurriculoColaboradorRespository, CurriculoColaboradorRespository>();
builder.Services.AddScoped<IImportacaoColaboradorService, ImportacaoColaboradorService>();
builder.Services.AddColaboradorServicesDependencies();
builder.Services.AddTemplateRepositoryDependencies();
builder.Services.AddHistoricoCVDependencies();
builder.Services.AddCidadaniaServicesDependencies();

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