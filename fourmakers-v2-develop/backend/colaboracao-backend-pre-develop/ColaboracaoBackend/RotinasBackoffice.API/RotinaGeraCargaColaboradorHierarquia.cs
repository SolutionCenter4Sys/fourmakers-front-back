using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using DataTransferObject.Domain.Carga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RotinasBackoffice.API.Mock.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class RotinaGeraCargaColaboradorHierarquia : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IHostApplicationLifetime _lifetime;

        public RotinaGeraCargaColaboradorHierarquia(IHostApplicationLifetime lifetime, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _lifetime = lifetime;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            var cronExpression = _configuration["Schedule:CronCarga"] ?? "* */1 * * * *";
            _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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
                    if (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "HML")
                        await CargaColaboradorHierarquiaMock();
                    else
                        await CargaColaboradorHierarquiaCCH();
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
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var mockFoursys = scope.ServiceProvider.GetRequiredService<IMockFoursys>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    
                    var lstHierarquia = mockFoursys.GeraCargaHierarquia();
                    cargaFourmakerService.InsereCargaHierarquiaColaborador(lstHierarquia, 2);
                }
                
                Console.WriteLine("Finalizou a rotina RotinaGeraCargaColaboradorHierarquia Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina RotinaGeraCargaColaboradorHierarquia Mock as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\nDetalhes: " + e.Message + "\nStack Trace: " + e.StackTrace.ToString());
                return "A operação falhou." + e.Message;
            }
        }
        private async Task<string> CargaColaboradorHierarquiaCCH()
        {
            var messageLog = string.Empty;

            try
            {
                messageLog = "Iniciou a rotina RotinaGeraCargaColaboradorHierarquia as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                Console.WriteLine(messageLog);
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cCHClient = scope.ServiceProvider.GetRequiredService<ICCHClient>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    var logDBCore = scope.ServiceProvider.GetRequiredService<ILogDBCore>();
                    
                    logDBCore.SaveLogDefaultInDatabase(messageLog, string.Empty, DataTransferObject.Domain.Log.ProcessIdentifierEnum.CargaColaboradorHierarquiaCCH);

                    var tokenValidacao = await cCHClient.Autenticacao();
                    var retHierarquia = await cCHClient.Hierarquia(tokenValidacao.token);
                    retHierarquia.ForEach(x => {
                        x.Recurso = x.Recurso.Where(y => y.CodigoProfissional != 0).ToList();
                    });
                    var lstHierarquia = new List<ColaboradorHierarquiaCargaDTO>();

                    foreach (var hierarquia in retHierarquia)
                    {
                        foreach (var recurso in hierarquia.Recurso)
                        {
                            lstHierarquia.Add(new ColaboradorHierarquiaCargaDTO
                            {
                                IdentificadorSuperior = hierarquia.CodigoProfissional.ToString(),
                                IdentificadorColaborador = recurso.CodigoProfissional.ToString()
                            });
                        }
                    }

                    logDBCore.SaveLogDefaultInDatabase("Número de Hierarquias a inserir: " + lstHierarquia.Count, string.Empty, DataTransferObject.Domain.Log.ProcessIdentifierEnum.CargaColaboradorHierarquiaCCH);

                    cargaFourmakerService.InsereCargaHierarquiaColaborador(lstHierarquia, 2);

                    messageLog = "Finalizou a rotina RotinaGeraCargaColaboradorHierarquia as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    Console.WriteLine(messageLog);
                    logDBCore.SaveLogDefaultInDatabase(messageLog, string.Empty, DataTransferObject.Domain.Log.ProcessIdentifierEnum.CargaColaboradorHierarquiaCCH);
                }

                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var logDBCore = scope.ServiceProvider.GetRequiredService<ILogDBCore>();
                    messageLog = "Erro na rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    Console.WriteLine(messageLog);
                    logDBCore.SaveExceptionLogInDatabase(messageLog, e, DataTransferObject.Domain.Log.ProcessIdentifierEnum.CargaColaboradorHierarquiaCCH);
                }
                return "A operação falhou." + e.Message;
            }
        }
    }
}