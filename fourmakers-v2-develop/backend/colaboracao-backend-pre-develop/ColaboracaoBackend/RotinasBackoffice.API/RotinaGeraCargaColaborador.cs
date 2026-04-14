using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
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
using Microsoft.IdentityModel.Tokens;

namespace RotinasBackoffice.API
{
    public class RotinaGeraCargaColaborador : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IHostApplicationLifetime _lifetime;
        
        public RotinaGeraCargaColaborador(
            IHostApplicationLifetime lifetime, 
            IConfiguration configuration, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _lifetime = lifetime;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            var cronExpression = _configuration["Schedule:CronCargaColaboradorCCH"] ?? "* */1 * * * *";
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
                        await CargaColaboradorMock();
                    else
                        await CargaColaboradorCCH();
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
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var mockFoursys = scope.ServiceProvider.GetRequiredService<IMockFoursys>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    
                    var retColaboradores = mockFoursys.GeraCargaColaborador();
                    var indexCarga = 0;
                    var totalItens = retColaboradores.Count();
                    var colabradoresMockAcesso = !String.IsNullOrEmpty(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MOCK_ACESSO)) ?
                        VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MOCK_ACESSO).Split(",")
                            .Select(x => new KeyValuePair<string, string>(x.Split("|")[1], x.Split("|")[0])).ToList()
                        : new List<KeyValuePair<string, string>>();
                    var indexSubstituicaoMock = 0;
                    foreach (var mockAcesso in colabradoresMockAcesso)
                    {
                        retColaboradores[indexSubstituicaoMock].Cpf = mockAcesso.Key;
                        retColaboradores[indexSubstituicaoMock].Email = mockAcesso.Value;
                        indexSubstituicaoMock++;
                    }
                    foreach (var itemColaborador in retColaboradores)
                    {
                        cargaFourmakerService.InsereCargaColaborador(new List<ColaboradorCargaDTO> { itemColaborador }, 2);
                    }
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

        private async Task<string> CargaColaboradorCCH()
        {
            try
            {
                Console.WriteLine("Iniciou a rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cCHClient = scope.ServiceProvider.GetRequiredService<ICCHClient>();
                    var cargaFourmakerService = scope.ServiceProvider.GetRequiredService<ICargaFourmakerService>();
                    
                    var token = await cCHClient.AutenticacaoMapaAlocacao();
                    var tokenValidacao = await cCHClient.Autenticacao();
                    var retColaboradores = await cCHClient.ColaboradoresFourMakers(token.token);
                    var indexCarga = 0;
                    retColaboradores = retColaboradores.GroupBy(x => x.cdCpf).Select(x => x.Where(y => y.flFuncionarioAtivo == true).LastOrDefault() ?? x.LastOrDefault()).ToList();
                    var totalItens = retColaboradores.Count();
                    
                    foreach (var colabBasic in retColaboradores)
                    {
                        try
                        {
                            if ((indexCarga % 100) == 0)
                            {
                                Console.WriteLine("Atualizando token");
                                token = await cCHClient.AutenticacaoMapaAlocacao();
                                tokenValidacao = await cCHClient.Autenticacao();
                            }
                            var colaboradorValidacao = await cCHClient.Recurso(colabBasic.cdProfissional.ToString(), tokenValidacao.token);
                            var projetoRecurso = await cCHClient.ProjetoRecurso(colaboradorValidacao.cdProfissional.ToString(), tokenValidacao.token);
                            if (colaboradorValidacao.nmEnderecoEletronico != null)
                                cargaFourmakerService.InsereCargaColaborador(new List<ColaboradorCargaDTO> {
                                    new ColaboradorCargaDTO{
                                        NomeCompleto = colabBasic.nmProfissional,
                                        DataNascimento = colabBasic.dtNascimento,
                                        Cargo = colaboradorValidacao.nmCargo ?? "",
                                        ColaboradorAtivo = colaboradorValidacao.flFuncionarioAtivo,
                                        ModeloContratacao = colabBasic.nmTipoContratacao.IsNullOrEmpty() ? null : colabBasic.nmTipoContratacao,
                                        DataInativacao = colabBasic.dtDesligamento,
                                        Cpf = colabBasic.cdCpf,
                                        DataAdmissao = colaboradorValidacao.dtContratacao,
                                        Departamento = "",
                                        Email = colaboradorValidacao.nmEnderecoEletronico,
                                        Filial = colaboradorValidacao.nmDivisao,
                                        IdentificadorCargo = colaboradorValidacao.cdCargo.ToString() ?? "",
                                        IdentificadorColaborador = colaboradorValidacao.cdProfissional.ToString(),
                                        IdentificadorDepartamento = "0",
                                        IdentificadorFilial = colaboradorValidacao.cdDivisao.ToString(),
                                        Ativo = colaboradorValidacao.flFuncionarioAtivo,
                                        Projetos = projetoRecurso.Select(x => new ColaboradorProjetoCargaDTO {
                                            CodigoColaborador = colaboradorValidacao.cdProfissional.ToString(),
                                            CodigoProjeto = x.cdProjeto.ToString(),
                                            NomeProjeto = x.nmProjeto
                                        }).ToList()
                                    }
                                }, 2);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao tentar obter os dados do colaborador: " + colabBasic.nmProfissional
                                + "/código: " + colabBasic.cdProfissional
                                + "/Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                + "/Info: " + ex.Message);
                            continue;
                        }
                        finally
                        {
                            indexCarga++;
                            Console.WriteLine("Iteração " + indexCarga + " ----- Progresso: " + indexCarga + " de " + totalItens);
                        }
                    }
                }
                
                Console.WriteLine("Finalizou a rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na rotina RotinaGeraCargaColaborador as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return "A operação falhou." + e.Message;
            }
        }
    }
}