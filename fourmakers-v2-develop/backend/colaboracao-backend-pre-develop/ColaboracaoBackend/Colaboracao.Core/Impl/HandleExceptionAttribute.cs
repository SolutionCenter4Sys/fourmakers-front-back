using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Colaboracao.Core
{
    public class HandleExceptionAttribute : Attribute, IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var statusCode = context.Exception switch
            {
                ApplicationException => HttpStatusCode.BadRequest, // erro na ao tentar executar a ação (ex.: usuário já existe)
                ArgumentException => HttpStatusCode.BadRequest, // erro ao enviar parâmetro errado
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                AccessViolationException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            var result = new ApiGenericResult
            {
                Sucesso = false,
                Mensagem = context.Exception.Message
            };

            var env = context.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

            try
            {
                var erros = ObterDetalhesExcecao(context.Exception);
                var errosString = JsonConvert.SerializeObject(erros);
                Console.WriteLine(errosString);
                if (env?.IsDevelopment() == true)
                {
                    result.Erros = erros;
                }
            }
            catch (Exception)
            {
                result.Erros = new List<string> { "Erro ao obter detalhes da exceção" };
            }

            context.Result = new ObjectResult(result)
            {
                StatusCode = (int)statusCode
            };

            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }

        private static List<string> ObterDetalhesExcecao(Exception ex)
        {
            var detalhes = new List<string>();
            var atual = ex;
            while (atual != null)
            {
                var bloco = $"[{atual.GetType().Name}] {atual.Message}";
                if (!string.IsNullOrWhiteSpace(atual.StackTrace))
                    bloco += "\n" + atual.StackTrace;
                detalhes.Add(NormalizarQuebrasDeLinha(bloco));
                atual = atual.InnerException;
            }
            return detalhes;
        }

        /// <summary>Substitui \r\n e \r por \n para o JSON exibir quebras de linha corretamente.</summary>
        private static string NormalizarQuebrasDeLinha(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto.Replace("\r\n", "\n").Replace('\r', '\n');
        }
    }
}