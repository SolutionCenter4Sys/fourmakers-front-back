using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using DataTransferObject.Domain.Carga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RotinasBackoffice.API.Mock.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API.ShowCase
{
    public class RotinaGeraCargaColaborador : BackgroundService
    {
        private readonly ICCHClient _cCHClient;
        private readonly IConfiguration _configuration;
        private readonly IMockShowCase _mockFoursys;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly ICargaFourmakerService _cargaFourmakerService;
        private readonly IHostApplicationLifetime _lifetime;
        public RotinaGeraCargaColaborador(IHostApplicationLifetime lifetime, IConfiguration configuration, ICCHClient cCHClient, ICargaFourmakerService cargaFourmakerService, IMockShowCase mockFoursys)
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
                    await CargaColaboradorMock();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(60000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task<string> CargaColaboradorMock()
        {
            try
            {
                Console.WriteLine("Iniciou a rotina RotinaGeraCargaColaborador Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                var retColaboradores = _mockFoursys.GeraCargaColaborador();
                var indexCarga = 0;
                var totalItens = retColaboradores.Count();
                foreach (var itemColaborador in retColaboradores)
                {
                    itemColaborador.Ativo = true;
                    itemColaborador.ColaboradorAtivo = true;
                    _cargaFourmakerService.InsereCargaColaborador(new List<ColaboradorCargaDTO> { itemColaborador }, 5);
                }
                Console.WriteLine("Finalizou a rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\nDetalhes: " + e.Message + "\nStack Trace: " + e.StackTrace.ToString());
                return "A operação falhou." + e.Message;
            }
        }
    }
}