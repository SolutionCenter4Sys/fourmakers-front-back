using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Aws.Infra.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Colaborador.Domain.Interfaces.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using SRS.Domain.Interfaces.Service;
using DataTransferObject.Domain.SRS.Candidate;
using Colaboracao.Core.Interfaces;

namespace BancoTalentoSRSConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _queueBancoTalentosUrl = string.Empty;
        private readonly string _queueBancoTalentosAuxUrl = string.Empty;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _queueBancoTalentosUrl = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_URL_BANCOTALENTOS_SRS")
                ?? _configuration["AWS:QueueBancoTalentosUrl"] 
                ?? string.Empty;

            _queueBancoTalentosAuxUrl = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_URL_BANCOTALENTOS_AUX")
                ?? _configuration["AWS:QueueBancoTalentosAuxUrl"]
                ?? string.Empty;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tarefaPrincipal = ConsumerBancoTalentosSRS(stoppingToken);
            var tarefaAuxiliar = ConsumerBancoTalentosSRSAux(stoppingToken);
            await Task.WhenAll(tarefaPrincipal, tarefaAuxiliar);
        }

        private async Task ConsumerBancoTalentosSRS(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BancoTalentoSRSConsumer iniciado. Consumindo fila: {QueueUrl}", _queueBancoTalentosUrl);
            

            while (!stoppingToken.IsCancellationRequested)
            {
                
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;
                using var scope = _scopeFactory.CreateScope();
                var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();

                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueBancoTalentosUrl, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            var messageScope = _scopeFactory.CreateScope();
                            var importacaoColaboradorService = messageScope.ServiceProvider.GetRequiredService<IImportacaoColaboradorService>();
                            try
                            {
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;

                                _logger.LogInformation("Mensagem recebida: {MessageBody}", body);

                                var objMensagem = JsonConvert.DeserializeObject<BancoTalentosSRSMessage>(body);
                                if (objMensagem == null || string.IsNullOrWhiteSpace(objMensagem.PerfilIN))
                                {
                                    throw new InvalidOperationException("Mensagem inválida - PerfilIN não encontrado");
                                }

                                var orgId = 2;
                                var token = Environment.GetEnvironmentVariable("TOKEN_SISTEMA_COLABORACAO");
                                var resultado = await importacaoColaboradorService.SincronizarColaboradorBancoTalentos(
                                    objMensagem.PerfilIN, 
                                    orgId, 
                                    token,
                                    objMensagem.Email,
                                    objMensagem.Telefone,
                                    objMensagem.CodVaga);

                                _logger.LogInformation("Sincronização concluída para perfil: {PerfilIN}", objMensagem.PerfilIN);

                                // Registrar log de sucesso
                                await importacaoColaboradorService.RegistrarLogBancoTalentoSRSAsync(
                                    objMensagem.PerfilIN, 
                                    true, 
                                    "Sincronização realizada com sucesso");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", 
                                    message.MessageId, ex.Message, ex.StackTrace);

                                // Registrar log de erro
                                try
                                {
                                    var objMensagem = JsonConvert.DeserializeObject<BancoTalentosSRSMessage>(body);
                                    var urlLinkedin = objMensagem?.PerfilIN ?? "URL não identificada";
                                    
                                    await importacaoColaboradorService.RegistrarLogBancoTalentoSRSAsync(
                                        urlLinkedin, 
                                        false, 
                                        ex.Message, 
                                        ex.StackTrace);
                                }
                                catch (Exception logEx)
                                {
                                    _logger.LogError(logEx, "Erro ao registrar log de erro para Banco de Talentos SRS");
                                }
                            }
                            finally
                            {
                                // Sempre deletar a mensagem da fila, mesmo em caso de erro
                                await producer.DeleteMessageAsync(_queueBancoTalentosUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila: {MessageId}", message.MessageId);
                                messageScope.Dispose();
                            }
                        }
                    }
                    scope.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ConsumerBancoTalentosSRSAux(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BancoTalentoSRSConsumer AUX iniciado. Consumindo fila: {QueueUrl}", _queueBancoTalentosAuxUrl);
            using var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            var candidateService = scope.ServiceProvider.GetRequiredService<ISRSCandidateService>();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;

                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueBancoTalentosAuxUrl, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;

                                _logger.LogInformation("Mensagem AUX recebida: {MessageBody}", body);

                                var objMensagem = JsonConvert.DeserializeObject<BancoTalentosSRSMessage>(body);
                                if (objMensagem == null || string.IsNullOrWhiteSpace(objMensagem.PerfilIN))
                                {
                                    throw new InvalidOperationException("Mensagem inválida - PerfilIN não encontrado");
                                }

                                var input = new CadastrarAtualizarPreAprovarCandidadoLinkedinInput
                                {
                                    UrlLinkedin = objMensagem.PerfilIN,
                                    CodigoDaVaga = null,
                                    UserId = 1
                                };

                                var connectionDB = scope.ServiceProvider.GetRequiredService<IDBConnection>();
                                connectionDB.NewConnection();
                                var resultado = await candidateService.CadastrarAtualizarPreAprovarCandidadoService(input);

                                _logger.LogInformation("Processamento AUX concluído para perfil: {PerfilIN}. Mensagem: {Mensagem}", objMensagem.PerfilIN, resultado?.Mensagem);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem AUX: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}",
                                    message.MessageId, ex.Message, ex.StackTrace);
                            }
                            finally
                            {
                                await producer.DeleteMessageAsync(_queueBancoTalentosAuxUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem AUX deletada da fila: {MessageId}", message.MessageId);
                            }
                        }
                    }
                    scope.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS AUX");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

    }

    public class BancoTalentosSRSMessage
    {
        public string PerfilIN { get; set; } = string.Empty;
        public string? Email { get; set; } = null;
        public string? Telefone { get; set; } = null;
        public string? CodVaga { get; set; } = null;
    }
}
