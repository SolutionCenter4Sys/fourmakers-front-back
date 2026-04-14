using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Aws.Infra.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Apontamento;
using Apontamento.Domain.Interfaces.Service;
using Newtonsoft.Json;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using Financeiro.Domain.Interfaces.Holerite;
using Financeiro.Domain.Interfaces.Conciliacao;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using Colaboracao.Core.Interfaces;
using Colaboracao.Core.Exceptions;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;

namespace FolhaColaboradorConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _queueFolhaUrl;
        private readonly string _queueHoleriteUrl;
        private readonly string _queueRubricaCargaUrl;
        private readonly string _queueHoleriteAnalisado;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _queueFolhaUrl = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_FOLHAPONTO")
                ?? _configuration["AWS:QueueUrl"];
            _queueHoleriteUrl = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_HOLERITE")
                ?? _configuration["AWS:QueueHoleriteUrl"];
            _queueHoleriteAnalisado = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_HOLERITEANALISADO")
                ?? _configuration["AWS:QueueHoleriteAnalisadoUrl"];
            _queueRubricaCargaUrl = Environment.GetEnvironmentVariable("AWS_SQS_QUEUE_RUBRICA")
                                    ?? _configuration["AWS:QueueRubricaUrl"];
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tarefaFolhaPonto = Task.Run(() => ConsumerFolhaPonto(stoppingToken), stoppingToken);
            var tarefaHolerite = Task.Run(() => ConsumerHolerite(stoppingToken), stoppingToken);
            var tarefaRubricaCarga = Task.Run(() => ConsumerRubricaCarga(stoppingToken), stoppingToken);
            var tarefaHoleriteAnalisado = Task.Run(() => ConsumerHoleriteAnalisado(stoppingToken), stoppingToken);

            await Task.WhenAll(tarefaFolhaPonto, tarefaHolerite,tarefaRubricaCarga, tarefaHoleriteAnalisado);
        }

        private async Task ConsumerHoleriteAnalisado(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HoleriteConsumerAnalisado iniciado. Consumindo fila: {QueueUrl}", _queueHoleriteAnalisado);
            var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            var conciliacaoService = scope.ServiceProvider.GetRequiredService<IConciliacaoService>();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;
                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueHoleriteAnalisado, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                message.MessageAttributes.TryGetValue("itemLoteId", out var itemLoteId);
                                message.MessageAttributes.TryGetValue("orgId", out var orgId);
                                message.MessageAttributes.TryGetValue("codigoInternoColaborador", out var codigoInternoColaborador);
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;

                                if (itemLoteId == null || orgId == null || codigoInternoColaborador == null)
                                {
                                    throw new InvalidOperationException("Atributos obrigatórios da mensagem não encontrados");
                                }
                                var objMensagem = System.Text.Json.JsonSerializer.Deserialize<RetornoAnaliseHoleriteDTO>(body, new System.Text.Json.JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if(objMensagem == null)
                                {
                                    throw new InvalidOperationException("Mensagem não encontrada");
                                }
                                var connectionDB = scope.ServiceProvider.GetRequiredService<IDBConnection>();
                                connectionDB.NewConnection();
                                await conciliacaoService.ProcessarItemConciliacaoAsync(objMensagem, int.Parse(orgId.StringValue), itemLoteId.StringValue, codigoInternoColaborador.StringValue);
                                await producer.DeleteMessageAsync(_queueHoleriteAnalisado, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por sucesso: {MessageId}", message.MessageId);
                            }
                            catch (ValidationException ex)
                            {
                                _logger.LogError(ex, "Erro de validação ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                                await producer.DeleteMessageAsync(_queueHoleriteAnalisado, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por erro de validação: {MessageId}", message.MessageId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                            }

                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ConsumerHolerite(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HoleriteConsumer iniciado. Consumindo fila: {QueueUrl}", _queueHoleriteUrl);
            var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            var holeriteService = scope.ServiceProvider.GetRequiredService<IHoleriteService>();
            while (!stoppingToken.IsCancellationRequested)
            {
                
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;

                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueHoleriteUrl, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                message.MessageAttributes.TryGetValue("itemLoteId", out var itemLoteId);
                                message.MessageAttributes.TryGetValue("loteId", out var loteId);
                                message.MessageAttributes.TryGetValue("orgId", out var orgId);
                                message.MessageAttributes.TryGetValue("pagina", out var pagina);
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;

                                if (itemLoteId == null || loteId == null || orgId == null)
                                {
                                    throw new InvalidOperationException("Atributos obrigatórios da mensagem não encontrados");
                                }
                                var objMensagem = JsonConvert.DeserializeObject<FilaMessageDTO>(body);
                                if(objMensagem == null)
                                {
                                    throw new InvalidOperationException("Mensagem não encontrada");
                                }
                                var connectionDB = scope.ServiceProvider.GetRequiredService<IDBConnection>();
                                connectionDB.NewConnection();
                                await holeriteService.ProcessarItemHoleriteAsync(objMensagem, objMensagem.CodigoColaboradorSolicitante, int.Parse(orgId.StringValue), itemLoteId.StringValue, loteId.StringValue);
                                await producer.DeleteMessageAsync(_queueHoleriteUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por sucesso: {MessageId}", message.MessageId);
                            }
                            catch (ValidationException ex)
                            {
                                _logger.LogError(ex, "Erro de validação ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                                await producer.DeleteMessageAsync(_queueHoleriteUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por erro de validação: {MessageId}", message.MessageId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        private async Task ConsumerFolhaPonto(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FolhaColaboradorConsumer iniciado. Consumindo fila: {QueueUrl}", _queueFolhaUrl);
            var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            var folhaPontoService = scope.ServiceProvider.GetRequiredService<IFolhaPontoService>();
            while (!stoppingToken.IsCancellationRequested)
            {
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;

                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueFolhaUrl, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                message.MessageAttributes.TryGetValue("itemLoteId", out var itemLoteId);
                                message.MessageAttributes.TryGetValue("loteId", out var loteId);
                                message.MessageAttributes.TryGetValue("orgId", out var orgId);
                                message.MessageAttributes.TryGetValue("pagina", out var pagina);
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;

                                if (itemLoteId == null || loteId == null || orgId == null)
                                {
                                    throw new InvalidOperationException("Atributos obrigatórios da mensagem não encontrados");
                                }
                                var objMensagem = JsonConvert.DeserializeObject<FilaMessageDTO>(body);
                                if(objMensagem == null)
                                {
                                    throw new InvalidOperationException("Mensagem não encontrada");
                                }
                                var connectionDB = scope.ServiceProvider.GetRequiredService<IDBConnection>();
                                connectionDB.NewConnection();
                                await folhaPontoService.ProcessarItemFolhaPontoAsync(objMensagem, objMensagem.CodigoColaboradorSolicitante, int.Parse(orgId.StringValue), itemLoteId.StringValue, loteId.StringValue);
                                await producer.DeleteMessageAsync(_queueFolhaUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por sucesso: {MessageId}", message.MessageId);
                            }
                            catch (ValidationException ex)
                            {
                                _logger.LogError(ex, "Erro de validação ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                                await producer.DeleteMessageAsync(_queueFolhaUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por erro de validação: {MessageId}", message.MessageId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ConsumerRubricaCarga(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConsumerRubricaCarga iniciado. Consumindo fila: {QueueUrl}", _queueRubricaCargaUrl);

            var scope = _scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<IQueueProducer>();
            var rubricaCargaService = scope.ServiceProvider.GetRequiredService<IRubricaCargaService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var messageReceiptHandle = string.Empty;
                var body = string.Empty;

                try
                {
                    var response = await producer.ReceiveMessageAsync(_queueRubricaCargaUrl, stoppingToken);

                    if (response.Messages is not null)
                    {
                        foreach (var message in response.Messages)
                        {
                            try
                            {
                                message.MessageAttributes.TryGetValue("itemLoteId", out var itemLoteId);
                                message.MessageAttributes.TryGetValue("loteId", out var loteId);
                                message.MessageAttributes.TryGetValue("orgId", out var orgId);
                                message.MessageAttributes.TryGetValue("pagina", out var pagina);
                                messageReceiptHandle = message.ReceiptHandle;
                                body = message.Body;
                                
                                var objMensagem = JsonConvert.DeserializeObject<FilaMessageRubricaCargaDTO>(body);
                                if(objMensagem == null)
                                {
                                    throw new InvalidOperationException("Mensagem não encontrada");
                                }
                                var connectionDB = scope.ServiceProvider.GetRequiredService<IDBConnection>();
                                connectionDB.NewConnection();
                                await rubricaCargaService.ProcessarItemRubricaCargaAsync(objMensagem, objMensagem.CodigoColaboradorSolicitante, int.Parse(orgId.StringValue), itemLoteId.StringValue, loteId.StringValue);
                                await producer.DeleteMessageAsync(_queueRubricaCargaUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por sucesso: {MessageId}", message.MessageId);
                            }
                            catch (ValidationException ex)
                            {
                                _logger.LogError(ex, "Erro de validação ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                                await producer.DeleteMessageAsync(_queueRubricaCargaUrl, messageReceiptHandle);
                                _logger.LogInformation("Mensagem deletada da fila por erro de validação: {MessageId}", message.MessageId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}\nMensagem Erro: {MessageError}\nStack Trace: {StackTrace}", message.MessageId, ex.Message, ex.StackTrace);
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir fila SQS");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
} 