using Aws.Infra.Impl;
using Aws.Infra.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddAwsSqsDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<IQueueProducer, AmazonSQSProducer>();
            }
            else
            {
                services.TryAddScoped<IQueueProducer, AmazonSQSProducer>();
            }
        }
    }
}