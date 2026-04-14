using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aws.Infra.Interfaces.S3;
using Core.DomainModel;
using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace RotinasBackoffice.API;

/// <summary>
/// Expurgo diário de <c>tb_token_files_temp</c> com <c>data_criacao</c> anterior a 2 dias e objeto correspondente no S3.
/// Horário do cron é o do relógio do processo (geralmente UTC no servidor).
/// </summary>
public class RotinaLimpezaAws : BackgroundService
{
    private static readonly TimeSpan RetencaoMinima = TimeSpan.FromDays(2);

    private readonly CrontabSchedule _schedule;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RotinaLimpezaAws> _logger;
    private DateTime _nextRun;

    public RotinaLimpezaAws(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RotinaLimpezaAws> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        var cron = configuration["Schedule:CronLimpezaTokenFileTempAws"] ?? "0 0 22 * * *";
        _schedule = CrontabSchedule.Parse(cron, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(120000, cancellationToken);
        do
        {
            var now = DateTime.UtcNow;
            if (now > _nextRun)
            {
                await ExecutarLimpezaAsync(cancellationToken);
                _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
            }

            await Task.Delay(1000, cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    private async Task ExecutarLimpezaAsync(CancellationToken cancellationToken)
    {
        var limiteUtc = DateTime.UtcNow.Subtract(RetencaoMinima);

        using var scope = _serviceScopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ITokenFileTempRepository>();
        var s3 = scope.ServiceProvider.GetRequiredService<IAmazonS3Uploader>();

        IReadOnlyList<TokenFileTempDTO> expirados;
        try
        {
            expirados = await repo.ListarCriadosAntesDeAsync(limiteUtc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RotinaLimpezaAws: falha ao listar tb_token_files_temp anteriores a {Limite}", limiteUtc);
            return;
        }

        if (expirados.Count == 0)
        {
            _logger.LogInformation("RotinaLimpezaAws: nenhum registro expirado (limite {Limite}).", limiteUtc);
            return;
        }

        var tokensParaRemover = new List<string>();

        foreach (var item in expirados)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(item.NomeArquivo))
            {
                _logger.LogWarning("RotinaLimpezaAws: token {Token} sem nome_arquivo; removendo apenas do banco.", item.Token);
                tokensParaRemover.Add(item.Token);
                continue;
            }

            try
            {
                var key = item.NomeArquivo.Trim();
                var ok = await s3.DeleteFile(key);
                if (ok)
                    tokensParaRemover.Add(item.Token);
                else
                    _logger.LogWarning("RotinaLimpezaAws: DeleteFile retornou false para chave {Key} (token {Token}).", key, item.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RotinaLimpezaAws: erro S3 ao excluir {Key} (token {Token}).", item.NomeArquivo, item.Token);
            }
        }

        if (tokensParaRemover.Count == 0)
        {
            _logger.LogWarning("RotinaLimpezaAws: {Total} candidatos, nenhum elegível para remoção no banco nesta execução.", expirados.Count);
            return;
        }

        try
        {
            var deleted = await repo.ExcluirPorTokensAsync(tokensParaRemover);
            _logger.LogInformation(
                "RotinaLimpezaAws: removidos {Deleted} registro(s) no banco (candidatos {Total}, tokens com S3 OK ou sem chave).",
                deleted,
                expirados.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RotinaLimpezaAws: falha ao excluir tokens do banco.");
        }
    }
}
