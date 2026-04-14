using ApiClient.Domain;
using Colaboracao.Helper;
using Logs.Infra.Attributes;
using Logs.Infra.Constants;
using Logs.Infra.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Logs.Infra.Extensions
{
    public static class LogsServiceExtensions
    {
        /// <summary>
        /// Registra os serviços de logging estruturado para console (Promtail)
        /// Configura o logging para exibir apenas logs estruturados em produção/homologação
        /// </summary>
        public static IServiceCollection AddLogsInfra(this IServiceCollection services)
        {
            // Configurar logging para permitir apenas logs estruturados em produção/homologação
            services.AddLogging(builder =>
            {
                // Verificar ambiente através da configuração ou variável de ambiente
                var environment = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE);

                var isProductionOrHomolog = environment.Equals("PRD", System.StringComparison.OrdinalIgnoreCase) || 
                                            environment.Equals("HML", System.StringComparison.OrdinalIgnoreCase);

                if (isProductionOrHomolog)
                {
                    // Em produção/homologação: configurar para mostrar apenas logs estruturados
                    // Permitir logs estruturados no nível Trace (mais baixo possível para capturar todos)
                    builder.AddFilter(StructuredLogConstants.LogCategory, StructuredLogConstants.StructuredLogLevel);
                    
                    // Configurar outros logs para níveis mais altos (não exibir)
                    // Isso garante que apenas logs estruturados sejam exibidos
                    builder.AddFilter("Default", LogLevel.Error);
                    builder.AddFilter("Microsoft", LogLevel.Critical);
                    builder.AddFilter("System", LogLevel.Critical);
                }
                else
                {
                    // Em desenvolvimento: permitir todos os logs estruturados e outros logs normais
                    builder.AddFilter(StructuredLogConstants.LogCategory, StructuredLogConstants.StructuredLogLevel);
                }
            });

            return services;
        }

        /// <summary>
        /// Registra um serviço Domain com interceptação automática de logging se a classe tiver o atributo [LogDomainClass].
        /// Se a classe tiver o atributo, um proxy será criado para interceptar chamadas de métodos e gerar logs automaticamente.
        /// </summary>
        /// <typeparam name="TInterface">Interface do serviço</typeparam>
        /// <typeparam name="TImplementation">Implementação do serviço</typeparam>
        public static IServiceCollection AddScopedDomainService<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            // Verificar se a classe de implementação tem o atributo [LogDomainClass]
            var hasLogAttribute = typeof(TImplementation).GetCustomAttribute<LogDomainClassAttribute>() != null;

            if (hasLogAttribute)
            {
                // Registrar com factory que cria o proxy
                services.AddScoped<TInterface>(sp =>
                {
                    var implementation = ActivatorUtilities.CreateInstance<TImplementation>(sp);
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                    return LogDomainInterceptor<TInterface>.Create(implementation, loggerFactory);
                });
            }
            else
            {
                // Registrar normalmente sem interceptação
                services.AddScoped<TInterface, TImplementation>();
            }

            return services;
        }

        /// <summary>
        /// Registra um serviço Domain com interceptação automática de logging se a classe tiver o atributo [LogDomainClass].
        /// Usa TryAddScoped para evitar duplicação de registros.
        /// </summary>
        /// <typeparam name="TInterface">Interface do serviço</typeparam>
        /// <typeparam name="TImplementation">Implementação do serviço</typeparam>
        public static IServiceCollection TryAddScopedDomainService<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            // Verificar se a classe de implementação tem o atributo [LogDomainClass]
            var hasLogAttribute = typeof(TImplementation).GetCustomAttribute<LogDomainClassAttribute>() != null;

            if (hasLogAttribute)
            {
                // Registrar com factory que cria o proxy usando TryAddScoped
                services.TryAddScoped<TInterface>(sp =>
                {
                    var implementation = ActivatorUtilities.CreateInstance<TImplementation>(sp);
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                    return LogDomainInterceptor<TInterface>.Create(implementation, loggerFactory);
                });
            }
            else
            {
                // Registrar normalmente sem interceptação usando TryAddScoped
                services.TryAddScoped<TInterface, TImplementation>();
            }

            return services;
        }
    }
}

