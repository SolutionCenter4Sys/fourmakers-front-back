using Logs.Infra.Helpers;
using Logs.Infra.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Logs.Infra.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class LogActionAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var logEntry = new LogEntry();
            Exception exception = null;

            try
            {
                // Obter trace ID do header ou gerar um novo
                var traceId = context.HttpContext.Request.Headers["X-Trace-Id"].FirstOrDefault() 
                    ?? context.HttpContext.TraceIdentifier 
                    ?? Guid.NewGuid().ToString();

                // Obter frontend trace ID do header
                var frontendTraceId = context.HttpContext.Request.Headers["FRONTEND_TRACE_ID"].FirstOrDefault();

                // Definir trace ID no response header
                context.HttpContext.Response.Headers["X-Trace-Id"] = traceId;

                // Definir frontend trace ID no response header se presente
                if (!string.IsNullOrEmpty(frontendTraceId))
                {
                    context.HttpContext.Response.Headers["FRONTEND_TRACE_ID"] = frontendTraceId;
                }

                // Definir trace IDs no contexto assíncrono para propagar para a camada Domain
                Logs.Infra.Helpers.TraceIdContext.Current = traceId;
                if (!string.IsNullOrEmpty(frontendTraceId))
                {
                    Logs.Infra.Helpers.TraceIdContext.FrontendTraceId = frontendTraceId;
                }

                // Capturar informações do controller e método
                var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
                var actionName = context.RouteData.Values["action"]?.ToString() ?? "Unknown";
                var controllerType = context.Controller.GetType();
                var methodInfo = (context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)?.MethodInfo;

                // Obter nome do projeto
                var projeto = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";

                // Construir path da classe
                var classePath = $"{controllerType.Namespace}.{controllerType.Name}";

                // Capturar parâmetros
                var parametros = new System.Collections.Generic.Dictionary<string, object>();
                foreach (var parameter in context.ActionArguments)
                {
                    try
                    {
                        // Mascarar dados sensíveis antes de armazenar
                        var maskedValue = LogMaskingHelper.MaskSensitiveData(parameter.Value);
                        parametros[parameter.Key] = maskedValue;
                    }
                    catch
                    {
                        parametros[parameter.Key] = "[Não serializável]";
                    }
                }

                // Capturar Claims do usuário autenticado
                var claims = new System.Collections.Generic.Dictionary<string, string>();
                if (context.HttpContext.User?.Claims != null)
                {
                    foreach (var claim in context.HttpContext.User.Claims)
                    {
                        claims[claim.Type] = claim.Value;
                    }
                }

                // Preencher log entry
                logEntry.TraceId = traceId;
                logEntry.FrontendTraceId = frontendTraceId;
                logEntry.Projeto = projeto;
                logEntry.ClassePath = classePath;
                logEntry.Metodo = methodInfo?.Name ?? actionName;
                
                // Serializar parâmetros como JSON válido (já com dados mascarados)
                logEntry.ParametrosJson = JsonSerializer.Serialize(parametros, new JsonSerializerOptions 
                { 
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                // Serializar Claims como JSON
                logEntry.ClaimsJson = JsonSerializer.Serialize(claims, new JsonSerializerOptions 
                { 
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                logEntry.DataHora = DateTime.UtcNow;
                logEntry.HttpMethod = context.HttpContext.Request.Method;
                logEntry.RequestPath = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

                // Executar a ação
                var executedContext = await next();

                // Capturar status code após execução
                if (executedContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
                {
                    logEntry.StatusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
                }
                else
                {
                    logEntry.StatusCode = context.HttpContext.Response.StatusCode;
                }

                // Capturar exceção se houver
                exception = executedContext.Exception;
                if (exception != null)
                {
                    logEntry.ExceptionMessage = exception.Message;
                    logEntry.ExceptionStackTrace = exception.StackTrace;
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
                stopwatch.Stop();
                logEntry.TempoExecucaoMs = stopwatch.ElapsedMilliseconds;

                // Escrever log estruturado no console para consumo pelo Promtail
                try
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    if (loggerFactory != null)
                    {
                        var logger = loggerFactory.CreateLogger(Logs.Infra.Constants.StructuredLogConstants.LogCategory);
                        StructuredLogger.LogStructured(logger, logEntry);
                    }
                }
                catch
                {
                    // Ignorar erros de log para não impactar a aplicação
                }
            }
        }
    }
}

