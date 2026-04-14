using CargaCore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RotinasBackoffice.API.Mock.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaGeraCargaSincronizaCRM : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IHostApplicationLifetime _lifetime;

        public RotinaGeraCargaSincronizaCRM(IHostApplicationLifetime lifetime, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _lifetime = lifetime;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronCRM"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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
                    await SincronizarCRM();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(60000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task SincronizarCRM()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sincronizaCRMService = scope.ServiceProvider.GetRequiredService<ISincronizaCRMService>();
                await sincronizaCRMService.SincronizarCRM();
            }
        }
    }
}