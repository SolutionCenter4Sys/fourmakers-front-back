using ApiClient.Domain;
using Colaboracao.Helper;
using DataTransferObject.Domain.Vaga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaVaga : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public RotinaVaga(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:Cron1"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            do
            {
                var now = DateTime.UtcNow;
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    var retorno = await VagaSRS();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(120000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);

            //await VagaSRS();
        }

        public async Task<string> VagaSRS()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var sRSService = scope.ServiceProvider.GetRequiredService<ISRSService>();
                    
                    var token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);
                    var vagas = await sRSService.BuscarVagaSRS(token);

                    foreach (var vaga in vagas)
                    {
                        var listaVagas = new List<VagaDTO> { vaga };
                        //await _vagaService.CriarOrdemDeTrabalhoInterna(listaVagas, token);
                        Console.WriteLine($"Ordem de trabalho criada para a vaga: {vaga.IdVaga}");
                    }
                }

                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                return "A operação falhou." + e.Message;
            }
        }
    }
}