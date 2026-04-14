using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Util.Enum;
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
    public class RotinaGeraCargaProjeto : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IHostApplicationLifetime _lifetime;
        
        public RotinaGeraCargaProjeto(IHostApplicationLifetime lifetime, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _lifetime = lifetime;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronCarga"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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
                        await CargaProjetoMock();
                    else
                        await CargaProjetoCCH();
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

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var mockFoursys = scope.ServiceProvider.GetRequiredService<IMockFoursys>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    
                    var retProjetos = mockFoursys.GeraCargaProjeto();
                    var retGerenteProjetosMock = mockFoursys.GeraCargaProjetoGerente();
                    var indexCarga = 0;
                    foreach (var itemProjeto in retProjetos)
                    {
                        cargaFourmakerService.InsereCargaProjeto(new List<ProjetoCargaDTO> { itemProjeto }, 2);
                    }
                    var gruposProjetoGerentes = retGerenteProjetosMock.GroupBy(x => x.IdentificadorProjeto);
                    var totalItens = gruposProjetoGerentes.Count();
                    foreach (var grupoProjetoGerente in gruposProjetoGerentes)
                    {
                        try
                        {
                            cargaFourmakerService.InsereCargaHierarquiaProjeto(grupoProjetoGerente.ToList(), 2);
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

        private async Task<string> CargaProjetoCCH()
        {
            try
            {
                Console.WriteLine("Iniciou a rotina CargaProjetoCCH as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cCHClient = scope.ServiceProvider.GetRequiredService<ICCHClient>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    
                    var token = await cCHClient.AutenticacaoMapaAlocacao();
                    var tokenValidacao = await cCHClient.Autenticacao();
                    var retProjetos = await cCHClient.ProjetoHorasFourMakers(token.token);
                    var indexCarga = 0;
                    var totalItens = retProjetos.Count();
                    
                    foreach (var projeto in retProjetos.OrderByDescending(c => c.cdProjeto))
                    {
                        try
                        {
                            if ((indexCarga % 100) == 0)
                            {
                                Console.WriteLine("Atualizando token em projetos");
                                token = await cCHClient.AutenticacaoMapaAlocacao();
                                tokenValidacao = await cCHClient.Autenticacao();
                            }
                            var projetoValidacao = (await cCHClient.ValidacaoProjeto(projeto.cdProjeto.ToString(), tokenValidacao.token)).FirstOrDefault();

                            if (projetoValidacao is null)
                            {
                                Console.WriteLine("Erro ao tentar obter os dados do projeto: " + projeto.nmProjeto
                                                  + "/código: " + projeto.cdProjeto
                                                  + "/Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                                  + "/Info: " + "Nenhum recurso alocado no projeto");
                                continue;
                            }

                            cargaFourmakerService.InsereCargaProjeto(new List<ProjetoCargaDTO>{
                                new ProjetoCargaDTO{
                                    Cliente = projetoValidacao.nmCliente,
                                    DataFim = GetDataFinalProjeto(projetoValidacao.dtFimDesenvolvimento, projetoValidacao.dtReplanejamento),
                                    DataInicio = projeto.dtInicioProjeto,
                                    Filial = projetoValidacao.nmDivisao,
                                    IdentificadorCliente = projetoValidacao.cdCliente.ToString(),
                                    IdentificadorFilial = projetoValidacao.cdDivisao.ToString(),
                                    IdentificadorProjeto = projeto.cdProjeto.ToString(),
                                    IdentificadorProposta = projeto.nmPropostas,
                                    Projeto = projeto.nmProjeto,
                                    QtdHorasExecutadas = projeto.qtHorasTrabalhadas,
                                    QtdHorasPlanejada = projeto.qtHorasComerciais,
                                    IndentificadorStatus = projeto.cdStatusProjeto,
                                    Status = projeto.dsStatusProjeto ?? "Sem Status",
                                    Propostas = projetoValidacao.propostas
                                }
                            }, 2);
                            cargaFourmakerService.InsereCargaHierarquiaProjeto(new List<ProjetoGerenteCargaDTO>{
                                new ProjetoGerenteCargaDTO{
                                    IdentificadorColaboradorGerente = projetoValidacao.cdGerenteProjeto.ToString(),
                                    IdentificadorProjeto = projeto.cdProjeto.ToString(),
                                    IdentificadorTipoGerente = "GerenteProjeto"
                                }
                            }, EnumORG.FOURSYS_2.ToInt());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao tentar obter os dados do projeto: " + projeto.nmProjeto
                                            + "/código: " + projeto.cdProjeto
                                            + "/Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                            + "/Info: " + ex.Message);
                            continue;
                        }
                        indexCarga++;
                        Console.WriteLine("Iteração de projeto " + indexCarga + " ----- Progresso: " + indexCarga + " de " + totalItens);
                    }
                }

                Console.WriteLine("Finalizou a rotina CargaProjetoCCH as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina CargaProjetoCCH as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "A operação falhou." + e.Message;
            }
        }

        private DateTime? GetDataFinalProjeto(DateTime? dtFim, DateTime? dtReplanejamento)
        {
            if (dtReplanejamento > dtFim)
                return dtReplanejamento;

            return dtFim;
        }
    }
}