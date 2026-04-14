using ApiClient.Domain.Interfaces;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using RotinasBackoffice.Domain.Interfaces.Services;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RotinasBackoffice.API
{
    public class VagasSRSSchedule : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public VagasSRSSchedule(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(_configuration["Schedule:CronVagasSRS"], new CrontabSchedule.ParseOptions { IncludingSeconds = true });
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

        // Última modificação por: [Nome do Desenvolvedor] em [Data]
        public async Task<string> VagaSRS()
        {
            try
            {
                Console.WriteLine("Iniciou a sincronização de vagas do srs");
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var vagasSrsClient = scope.ServiceProvider.GetRequiredService<ISRSClient>();
                    var vagasSRSServices = scope.ServiceProvider.GetRequiredService<IVagasSRSServices>();
                    
                    var retorno = new VagasSRSResult();
                    var token = await vagasSrsClient.Autenticacao();
                    var retJobOrders = await vagasSrsClient.Requisicao(token.token);
                    retorno.VagasSRS = retJobOrders.Select(x => new VagaFourmakersSRSDTO()
                    {
                        Id_vaga = x.JobOrderId,
                        Titulo = x.Title,
                        Vagas_abertas = x.Openings,
                        Nivel = x.Senioridade,
                        Data_abertura = x.JoDtabVaga,
                        Status_vaga = x.JoStVaga,
                        Descricao = x.Description,
                        Cargo = x.Cargo,
                        Data_criacao = x.DateCreated,
                        Data_alteracao = x.DateModified,
                        LocTrabalho = x.joLocTrab,
                        Estado = x.state,
                        TextoLinkedin = x.textForLinkedin,
                        Confidencial = x.confidentialJob,
                        TipoVaga = x.tipoVaga,
                        Termometro = x.termometro,
                        Ativo = 1,
                        Skills = x.Skills.Select(x => new SkillsVagasDTO() { SkillId = x.SkillId, TypeSkills = x.TypeSkills, SkillNivelId = x.SkillNivelId }).ToList(),
                        Frequencia = x.Frequencia,
                        MaquinaFour = x.MaquinaFour.HasValue ? x.MaquinaFour.ToString() : "0",
                        MaquinaCliente = x.MaquinaCliente.HasValue ? x.MaquinaCliente.ToString() : "0",
                        Notes = x.Notes
                    }).ToList();

                    var existeVagas = vagasSRSServices.ObterTodasAsVagas();
                    var vagasParaAtualizar = new List<VagaFourmakersSRSDTO>();
                    var vagasParaInserir = new List<VagaFourmakersSRSDTO>();
                    foreach (var item in retorno.VagasSRS)
                    {
                        var vagaExistente = existeVagas.FirstOrDefault(x => x.Id_vaga == item.Id_vaga);
                        if (vagaExistente != null)
                        {
                            AtualizarVagaExistente(vagaExistente, item);
                            vagasParaAtualizar.Add(item);
                        }
                        else
                        {
                            vagasParaInserir.Add(item);
                        }
                    }
                    Console.WriteLine("Atualizando " + vagasParaAtualizar.Count() + " vagas");
                    vagasSRSServices.AtualizarVagas(vagasParaAtualizar);
                    Console.WriteLine("Inserindo " + vagasParaInserir.Count() + " novas vagas");
                    vagasSRSServices.InserirVagas(vagasParaInserir);
                }
                
                Console.WriteLine("Finalizou a sincronização de vagas do srs");
                return "Operação Concluida.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Falha na sincronização de vagas do srs. Message: " + e.Message + "\nStacktrace: " + e.StackTrace);
                return "A operação falhou." + e.Message;
            }
        }

        private void AtualizarVagaExistente(VagaFourmakersSRSDTO vagaExistente, VagaFourmakersSRSDTO novaVaga)
        {
            vagaExistente.Titulo = novaVaga.Titulo;
            vagaExistente.Vagas_abertas = novaVaga.Vagas_abertas;
            vagaExistente.Nivel = novaVaga.Nivel;
            vagaExistente.Data_abertura = novaVaga.Data_abertura;
            vagaExistente.Status_vaga = novaVaga.Status_vaga;
            vagaExistente.Descricao = novaVaga.Descricao;
            vagaExistente.Cargo = novaVaga.Cargo;
            vagaExistente.Data_criacao = novaVaga.Data_criacao;
            vagaExistente.Data_alteracao = novaVaga.Data_alteracao;
            vagaExistente.LocTrabalho = novaVaga.LocTrabalho;
            vagaExistente.Estado = novaVaga.Estado;
            vagaExistente.TextoLinkedin = novaVaga.TextoLinkedin;
            vagaExistente.Confidencial = novaVaga.Confidencial;
            vagaExistente.TipoVaga = novaVaga.TipoVaga;
            vagaExistente.Ativo = novaVaga.Ativo;
            vagaExistente.Skills = novaVaga.Skills;
            vagaExistente.Termometro = novaVaga.Termometro;
            vagaExistente.Frequencia = novaVaga.Frequencia;
            vagaExistente.MaquinaCliente = novaVaga.MaquinaCliente;
            vagaExistente.MaquinaFour = novaVaga.MaquinaFour;
            vagaExistente.Notes = novaVaga.Notes;
        }
    }
}