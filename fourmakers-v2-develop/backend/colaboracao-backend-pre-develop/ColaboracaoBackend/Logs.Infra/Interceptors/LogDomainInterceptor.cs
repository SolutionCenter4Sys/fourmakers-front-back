using Logs.Infra.Helpers;
using Logs.Infra.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Logs.Infra.Interceptors
{
    /// <summary>
    /// Interceptor para métodos de serviços Domain usando DispatchProxy.
    /// Intercepta chamadas de métodos e gera logs estruturados automaticamente.
    /// </summary>
    public class LogDomainInterceptor<T> : System.Reflection.DispatchProxy where T : class
    {
        private T _target;
        private ILogger _logger;

        /// <summary>
        /// Cria uma instância proxy do tipo T que intercepta chamadas de métodos
        /// </summary>
        public static T Create(T target, ILoggerFactory loggerFactory)
        {
            var proxy = System.Reflection.DispatchProxy.Create<T, LogDomainInterceptor<T>>();
            var interceptor = proxy as LogDomainInterceptor<T>;
            if (interceptor != null)
            {
                interceptor._target = target;
                interceptor._logger = loggerFactory?.CreateLogger(Logs.Infra.Constants.StructuredLogConstants.LogCategory);
            }
            return proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            // Ignorar propriedades (getters/setters)
            if (targetMethod.IsSpecialName && (targetMethod.Name.StartsWith("get_") || targetMethod.Name.StartsWith("set_")))
            {
                return targetMethod.Invoke(_target, args);
            }

            var stopwatch = Stopwatch.StartNew();
            var logEntry = new LogEntry();
            Exception exception = null;
            object result = null;

            try
            {
                // Obter trace IDs do contexto assíncrono
                var traceId = TraceIdContext.Current ?? Guid.NewGuid().ToString();
                var frontendTraceId = TraceIdContext.FrontendTraceId;

                // Obter informações do serviço
                var serviceType = _target.GetType();
                var projeto = serviceType.Assembly.GetName().Name ?? "Unknown";
                var classePath = $"{serviceType.Namespace}.{serviceType.Name}";

                // Capturar parâmetros
                var parametros = new System.Collections.Generic.Dictionary<string, object>();
                var parameters = targetMethod.GetParameters();
                for (int i = 0; i < parameters.Length && i < args.Length; i++)
                {
                    try
                    {
                        // Mascarar dados sensíveis antes de armazenar
                        var maskedValue = LogMaskingHelper.MaskSensitiveData(args[i]);
                        parametros[parameters[i].Name] = maskedValue;
                    }
                    catch
                    {
                        parametros[parameters[i].Name] = "[Não serializável]";
                    }
                }

                // Preencher log entry
                logEntry.TraceId = traceId;
                logEntry.FrontendTraceId = frontendTraceId;
                logEntry.Projeto = projeto;
                logEntry.ClassePath = classePath;
                logEntry.Metodo = targetMethod.Name;

                // Serializar parâmetros como JSON válido (já com dados mascarados)
                logEntry.ParametrosJson = JsonSerializer.Serialize(parametros, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                logEntry.DataHora = DateTime.UtcNow;
                // http_method, request_path, status_code e claims são NULL para logs da camada Domain

                // Executar o método
                result = targetMethod.Invoke(_target, args);

                logEntry.StatusCode = null; // Domain não tem status code HTTP

                // Se for um Task, criar uma continuação para logar após conclusão
                if (result is Task task)
                {
                    return HandleAsyncMethod(task, logEntry, stopwatch);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                logEntry.ExceptionMessage = ex.Message;
                logEntry.ExceptionStackTrace = ex.StackTrace;
            }
            finally
            {
                // Para métodos síncronos, logar imediatamente
                if (!(result is Task))
                {
                    stopwatch.Stop();
                    logEntry.TempoExecucaoMs = stopwatch.ElapsedMilliseconds;

                    // Escrever log estruturado no console para consumo pelo Promtail
                    try
                    {
                        StructuredLogger.LogStructured(_logger, logEntry);
                    }
                    catch
                    {
                        // Ignorar erros de log para não impactar a aplicação
                    }
                }
            }

            // Se houve exceção em método síncrono, relançar
            if (exception != null && !(result is Task))
            {
                throw exception;
            }

            return result;
        }

        private object HandleAsyncMethod(Task task, LogEntry logEntry, Stopwatch stopwatch)
        {
            // Para métodos assíncronos, criar uma continuação para logar após conclusão
            task.ContinueWith(t =>
            {
                try
                {
                    if (t.IsFaulted)
                    {
                        var ex = t.Exception?.GetBaseException() ?? t.Exception;
                        logEntry.ExceptionMessage = ex?.Message;
                        logEntry.ExceptionStackTrace = ex?.StackTrace;
                    }
                    else if (t.IsCanceled)
                    {
                        logEntry.ExceptionMessage = "Task foi cancelada";
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    logEntry.TempoExecucaoMs = stopwatch.ElapsedMilliseconds;

                    // Escrever log estruturado
                    try
                    {
                        StructuredLogger.LogStructured(_logger, logEntry);
                    }
                    catch
                    {
                        // Ignorar erros de log para não impactar a aplicação
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }
    }
}

