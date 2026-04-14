using Logs.Infra.Constants;
using Logs.Infra.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace Logs.Infra.Helpers
{
    /// <summary>
    /// Helper para escrever logs estruturados em JSON no console para consumo pelo Promtail
    /// </summary>
    public static class StructuredLogger
    {

        /// <summary>
        /// Escreve um log estruturado no console em formato JSON
        /// </summary>
        public static void LogStructured(ILogger logger, LogEntry logEntry)
        {
            if (logger == null)
                return;

            try
            {
                // Criar objeto estruturado para o log
                var logObject = new
                {
                    trace_id = logEntry.TraceId,
                    frontend_trace_id = logEntry.FrontendTraceId,
                    projeto = logEntry.Projeto,
                    classe_path = logEntry.ClassePath,
                    metodo = logEntry.Metodo,
                    parametros_json = logEntry.ParametrosJson != null 
                        ? JsonSerializer.Deserialize<object>(logEntry.ParametrosJson) 
                        : null,
                    claims = logEntry.ClaimsJson != null 
                        ? JsonSerializer.Deserialize<object>(logEntry.ClaimsJson) 
                        : null,
                    data_hora = logEntry.DataHora.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    http_method = logEntry.HttpMethod,
                    request_path = logEntry.RequestPath,
                    status_code = logEntry.StatusCode,
                    exception_message = logEntry.ExceptionMessage,
                    exception_stack_trace = logEntry.ExceptionStackTrace,
                    tempo_execucao_ms = logEntry.TempoExecucaoMs
                };

                // Serializar para JSON
                var jsonLog = JsonSerializer.Serialize(logObject, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                // Escrever diretamente no console para evitar formatação dupla do logger
                Console.Out.WriteLine($"{jsonLog}");
            }
            catch (Exception ex)
            {
                // Em caso de erro ao formatar o log, logar o erro sem quebrar a aplicação
                logger.LogError(ex, "Erro ao formatar log estruturado");
            }
        }
    }
}

