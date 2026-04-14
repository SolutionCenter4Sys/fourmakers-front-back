using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RotinasBackoffice.API.Mock.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API.ShowCase
{
    public class RotinaGeraCargaColaboradorHierarquia : BackgroundService
    {
        private readonly ICCHClient _cCHClient;
        private readonly IConfiguration _configuration;
        private readonly ICargaFourmakerService _cargaFourmakerService;
        private readonly IMockShowCase _mockFoursys;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IHostApplicationLifetime _lifetime;
        public RotinaGeraCargaColaboradorHierarquia(IHostApplicationLifetime lifetime, IConfiguration configuration, ICCHClient cCHClient, ICargaFourmakerService cargaFourmakerService, IMockShowCase mockFoursys)
        {
            _lifetime = lifetime;
            _cargaFourmakerService = cargaFourmakerService;
            _configuration = configuration;
            _cCHClient = cCHClient;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronCarga"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
            _mockFoursys = mockFoursys;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(120000, cancellationToken);
            do
            {
                var now = DateTime.UtcNow;
                if (now > _nextRun)
                {
                    await CargaColaboradorHierarquiaMock();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(60000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }
        private async Task<string> CargaColaboradorHierarquiaMock()
        {
            try
            {
                Console.WriteLine("Iniciou a rotina RotinaGeraCargaColaboradorHierarquia Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                var lstHierarquia = _mockFoursys.GeraCargaHierarquia();
                Console.WriteLine("Finalizou a rotina RotinaGeraCargaColaboradorHierarquia Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                _cargaFourmakerService.InsereCargaHierarquiaColaborador(lstHierarquia, 5);
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina RotinaGeraCargaColaboradorHierarquia Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\nDetalhes: " + e.Message + "\nStack Trace: " + e.StackTrace.ToString());
                return "A operação falhou." + e.Message;
            }
        }
    }
}