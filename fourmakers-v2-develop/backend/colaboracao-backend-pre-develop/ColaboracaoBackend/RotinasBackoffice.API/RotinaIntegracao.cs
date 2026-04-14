using ApiClient.Domain.Interfaces;
using Core.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaIntegracao : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public RotinaIntegracao(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:Cron"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.UtcNow;
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await IntegrarDados();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(120000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        public async Task<string> IntegrarDados()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cchClient = scope.ServiceProvider.GetRequiredService<ICCHClient>();
                    var repository = scope.ServiceProvider.GetRequiredService<IRotinaIntegracaoRepository>();
                    
                    var tokenCCH = (await cchClient.Autenticacao()).token;
                    var emails = repository.ListaEmailColaboradoresFoursys();

                    foreach (var email in emails)
                    {
                        var validacaoCch = await cchClient.Validacao(email, tokenCCH);
                        if (validacaoCch.Count() > 0)
                        {
                            var ativo = validacaoCch.FirstOrDefault().flFuncionarioAtivo;
                            repository.AtualizaStatusColaboradorPorEmail(ativo, email);
                        }
                    }
                }
                return "Operação concluida!";
            }
            catch (Exception e)
            {
                return "Operação falhou!" + e.Message;
            }
        }
    }
}