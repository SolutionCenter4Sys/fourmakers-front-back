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
    public class RotinaGeraCargaProjeto : BackgroundService
    {
        private readonly ICCHClient _cCHClient;
        private readonly IConfiguration _configuration;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly ICargaFourmakerService _cargaFourmakerService;
        private readonly IMockShowCase _mockFoursys;
        private readonly IHostApplicationLifetime _lifetime;
        public RotinaGeraCargaProjeto(IHostApplicationLifetime lifetime, IConfiguration configuration, ICCHClient cCHClient, ICargaFourmakerService cargaFourmakerService, IMockShowCase mockFoursys)
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
                    await CargaProjetoMock();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
                }
                await Task.Delay(60000, cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private async Task<string> CargaProjetoMock()
        {
            try
            {
                Console.WriteLine("Iniciou a rotina CargaProjeto Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                var retProjetos = _mockFoursys.GeraCargaProjeto();
                var retGerenteProjetosMock = _mockFoursys.GeraCargaProjetoGerente();
                var indexCarga = 0;
                foreach (var itemProjeto in retProjetos)
                {
                    _cargaFourmakerService.InsereCargaProjeto(new List<ProjetoCargaDTO> { itemProjeto }, 5);
                }
                var gruposProjetoGerentes = retGerenteProjetosMock.GroupBy(x => x.IdentificadorProjeto);
                var totalItens = gruposProjetoGerentes.Count();
                foreach (var grupoProjetoGerente in gruposProjetoGerentes)
                {
                    try
                    {
                        _cargaFourmakerService.InsereCargaHierarquiaProjeto(grupoProjetoGerente.ToList(), 5);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao tentar obter os dados do projeto: "
                            + "/código: " + grupoProjetoGerente.Key
                            + "/Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                            + "/Info: " + ex.Message);
                        continue;
                    }
                    indexCarga++;
                    Console.WriteLine("Iteração de grupo de projeto " + indexCarga + " ----- Progresso: " + indexCarga + " de " + totalItens);
                }

                Console.WriteLine("Finalizou a rotina CargaProjeto Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina CargaProjeto Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\nDetalhes: " + e.Message + "\nStack Trace: " + e.StackTrace.ToString());
                return "A operação falhou." + e.Message;
            }
        }
    }
}