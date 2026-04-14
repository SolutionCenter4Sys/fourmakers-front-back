using System.ComponentModel.DataAnnotations;
using Aws.Infra.Interfaces;
using CurriculoBatchConsumer.Services;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Lote;
using Core.Domain.Colaborador;
using Newtonsoft.Json;
using Colaboracao.Helper;
using ApiClient.Domain;

namespace CurriculoBatchConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _queueUrl;
        private readonly string _queueLinkedinUrl;
        private readonly IServiceScopeFactory _scopeFactory;
        private DateTime _horaQueIniciou;
        private DateTime _ultimoLogCurriculos;
        private DateTime _ultimoLogLinkedin;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_SQS_QUEUE_BATH_CURRICULO);
            _queueLinkedinUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_SQS_QUEUE_BATH_LINKEDIN);
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado - Iniciando consumers de CV e LinkedIn");
            _horaQueIniciou = DateTime.UtcNow;
            _ultimoLogCurriculos = DateTime.UtcNow;
            _ultimoLogLinkedin = DateTime.UtcNow;
            
            _logger.LogInformation($"Worker - Hora de início: {_horaQueIniciou:yyyy-MM-dd HH:mm:ss} UTC");
            _logger.LogInformation($"Worker - URL fila CV: {_queueUrl}");
            _logger.LogInformation($"Worker - URL fila LinkedIn: {_queueLinkedinUrl}");
            
            var taskCv = ConsumerCv(stoppingToken);
            var taskLinkedin = ConsumerLinkedin(stoppingToken);

            _logger.LogInformation("Worker - Aguardando finalização dos consumers...");
            await Task.WhenAll(taskCv, taskLinkedin);
            
            _logger.LogInformation("Worker - Todos os consumers finalizados");
        }

        private async Task ConsumerLinkedin(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consumer Linkedin iniciado.");
            var cicloCount = 0;
            var mensagensProcessadas = 0;
            var ultimaMensagemProcessada = DateTime.UtcNow;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                cicloCount++;
                _logger.LogDebug($"Consumer Linkedin - Iniciando ciclo {cicloCount}");
                
                try
                {
                    var scope = _scopeFactory.CreateScope();
                    _logger.LogDebug("Consumer Linkedin - Escopo criado com sucesso");
                    
                    var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
                    _logger.LogDebug("Consumer Linkedin - IQueueProducer obtido com sucesso");
                    
                    var messageReceiptHandle = String.Empty;
                    var idLote = String.Empty;
                    var body = String.Empty;

                    _logger.LogDebug($"Consumer Linkedin - Buscando mensagens da fila: {_queueLinkedinUrl}");
                    var response = await producer.ReceiveMessageAsync(_queueLinkedinUrl, stoppingToken);
                    _logger.LogDebug($"Consumer Linkedin - Recebidas {response.Messages?.Count ?? 0} mensagens");
                    
                    if (response.Messages == null || !response.Messages.Any())
                    {
                        _logger.LogDebug("Consumer Linkedin - Nenhuma mensagem encontrada, aguardando...");
                    }
                    else
                    {
                        foreach (var message in response.Messages)
                        {
                            _logger.LogDebug($"Consumer Linkedin - Processando mensagem: {message.MessageId}");
                            
                            var messageScope = _scopeFactory.CreateScope();
                            var service = messageScope.ServiceProvider.GetRequiredService<IService>();

                            try
                            {
                                messageReceiptHandle = message.ReceiptHandle;
                                idLote = (message.MessageAttributes["idLote"]).StringValue;
                                body = message.Body;
                                
                                _logger.LogInformation($"Consumer Linkedin - Iniciando processamento do lote: {idLote}");
                                
                                if (await service.ProcessarImportacaoLinkedin(body, idLote, messageReceiptHandle))
                                {
                                    _logger.LogInformation($"Consumer Linkedin - Lote {idLote} processado com sucesso, removendo da fila");
                                    await producer.DeleteMessageAsync(_queueLinkedinUrl, messageReceiptHandle);
                                    mensagensProcessadas++;
                                    ultimaMensagemProcessada = DateTime.UtcNow;
                                }
                                else
                                {
                                    _logger.LogWarning($"Consumer Linkedin - Lote {idLote} não foi processado com sucesso");
                                }
                            }
                            catch (Colaboracao.Core.Exceptions.ValidationException e)
                            {
                                _logger.LogError(e, $"Consumer Linkedin - Erro de validação no lote: {idLote}");
                            
                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    e.Message,
                                    e.StackTrace,
                                    body
                                );
                            
                                await producer.DeleteMessageAsync(_queueLinkedinUrl, messageReceiptHandle);
                            }
                            catch (ApplicationException e)
                            {
                                _logger.LogError(e, $"Consumer Linkedin - Erro de aplicação no lote: {idLote}");

                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    e.Message,
                                    e.StackTrace,
                                    body
                                );

                                await producer.DeleteMessageAsync(_queueLinkedinUrl, messageReceiptHandle);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Consumer Linkedin - Erro inesperado ao processar mensagem do lote: {idLote}");
                            
                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    ex.Message,
                                    ex.StackTrace,
                                    body
                                );

                                await producer.DeleteMessageAsync(_queueLinkedinUrl, messageReceiptHandle);
                            }
                            finally
                            {
                                await HealthLogLinkedin(service);
                                messageScope.Dispose();
                            }
                        }
                    }
                    scope.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Consumer Linkedin - Erro no ciclo principal {cicloCount}");
                }
            
                _logger.LogDebug($"Consumer Linkedin - Ciclo {cicloCount} finalizado. Mensagens processadas hoje: {mensagensProcessadas}. Última mensagem: {ultimaMensagemProcessada:yyyy-MM-dd HH:mm:ss}");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            
            _logger.LogInformation("Consumer Linkedin - Loop principal finalizado");
        }

        private ProcessarImportacaoLinkedinDTO Mapear(string body)
        {
            return JsonConvert.DeserializeObject<ProcessarImportacaoLinkedinDTO>(body);
        }

        private async Task HealthLogLinkedin(IService service)
        {
            if (DateTime.UtcNow.Subtract(_ultimoLogLinkedin) < TimeSpan.FromMinutes(10))
                return;

            var resumoHojeLotes = await service.ResumoLotesLinkedins();

            var log = $"IMPORTACAO LINKEDIN HORARIO DE BRASILIA {_horaQueIniciou.AddHours(-3).ToString()} - " +
                $"{resumoHojeLotes}";

            _logger.LogInformation(log);

            _ultimoLogLinkedin = DateTime.UtcNow;
        }

        private async Task ConsumerCv(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consumer CV iniciado.");
            var cicloCount = 0;
            var mensagensProcessadas = 0;
            var ultimaMensagemProcessada = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                cicloCount++;
                _logger.LogDebug($"Consumer CV - Iniciando ciclo {cicloCount}");
                
                try
                {
                    var scope = _scopeFactory.CreateScope();
                    _logger.LogDebug("Consumer CV - Escopo criado com sucesso");
                    
                    var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
                    _logger.LogDebug("Consumer CV - IQueueProducer obtido com sucesso");
                    
                    var messageReceiptHandle = String.Empty;
                    var idLote = String.Empty;
                    var body = String.Empty;

                    _logger.LogDebug($"Consumer CV - Buscando mensagens da fila: {_queueUrl}");
                    var response = await producer.ReceiveMessageAsync(_queueUrl, stoppingToken);
                    _logger.LogDebug($"Consumer CV - Recebidas {response.Messages?.Count ?? 0} mensagens");
                    
                    if (response.Messages == null || !response.Messages.Any())
                    {
                        _logger.LogDebug("Consumer CV - Nenhuma mensagem encontrada, aguardando...");
                    }
                    else
                    {
                        foreach (var message in response.Messages)
                        {
                            _logger.LogDebug($"Consumer CV - Processando mensagem: {message.MessageId}");
                            
                            var messageScope = _scopeFactory.CreateScope();
                            var service = messageScope.ServiceProvider.GetRequiredService<IService>();
                            
                            try
                            {
                                messageReceiptHandle = message.ReceiptHandle;
                                idLote = (message.MessageAttributes["idLote"]).StringValue;
                                body = message.Body;
                                
                                _logger.LogInformation($"Consumer CV - Iniciando processamento do lote: {idLote}");
                                
                                var dto = JsonConvert.DeserializeObject<DataTransferObject.Domain.Colaborador.ImportacaoColaboradorFilaDTO>(body);
                                _logger.LogDebug($"Consumer CV - DTO deserializado com sucesso para lote: {idLote}");
                                
                                if (await service.Processar(dto, idLote, messageReceiptHandle))
                                {
                                    _logger.LogInformation($"Consumer CV - Lote {idLote} processado com sucesso, removendo da fila");
                                    await producer.DeleteMessageAsync(_queueUrl, messageReceiptHandle);
                                    mensagensProcessadas++;
                                    ultimaMensagemProcessada = DateTime.UtcNow;
                                }
                                else
                                {
                                    _logger.LogWarning($"Consumer CV - Lote {idLote} não foi processado com sucesso");
                                }
                            }
                            catch (ArgumentException e)
                            {
                                _logger.LogError(e, $"Consumer CV - Erro de argumento no lote: {idLote}");
                            
                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    e.Message,
                                    e.StackTrace,
                                    body
                                );
                            
                                await producer.DeleteMessageAsync(_queueUrl, messageReceiptHandle);
                            }
                            catch (ApplicationException e)
                            {
                                _logger.LogError(e, $"Consumer CV - Erro de aplicação no lote: {idLote}");

                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    e.Message,
                                    e.StackTrace,
                                    body
                                );

                                await producer.DeleteMessageAsync(_queueUrl, messageReceiptHandle);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Consumer CV - Erro inesperado ao processar mensagem do lote: {idLote}");
                            
                                service.RegistrarOuAtualizarProcessamento(
                                    idLote,
                                    messageReceiptHandle,
                                    ex.Message,
                                    ex.StackTrace,
                                    body
                                );

                                await producer.DeleteMessageAsync(_queueUrl, messageReceiptHandle);
                            }
                            finally
                            {
                                await HealthLogAsync(service);
                                messageScope.Dispose();
                            }
                        }
                    }
                    scope.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Consumer CV - Erro no ciclo principal {cicloCount}");
                }

                _logger.LogDebug($"Consumer CV - Ciclo {cicloCount} finalizado. Mensagens processadas hoje: {mensagensProcessadas}. Última mensagem: {ultimaMensagemProcessada:yyyy-MM-dd HH:mm:ss}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            
            _logger.LogInformation("Consumer CV - Loop principal finalizado");
        }

        private async Task HealthLogAsync(IService service)
        {
            if (DateTime.UtcNow.Subtract(_ultimoLogCurriculos) < TimeSpan.FromMinutes(10))
                return;

            var resumoHojeLotes = await service.ResumoLotesCurriculos();

            var log = $"IMPORTACAO CV HORARIO DE BRASILIA {_horaQueIniciou.AddHours(-3).ToString()} - " +
                $"{resumoHojeLotes}";

            _logger.LogInformation(log);

            _ultimoLogCurriculos = DateTime.UtcNow;
        }
    }
}
