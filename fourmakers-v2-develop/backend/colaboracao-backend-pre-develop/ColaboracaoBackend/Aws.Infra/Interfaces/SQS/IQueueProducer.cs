using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace Aws.Infra.Interfaces
{
    public interface IQueueProducer
    {
        bool Produce(string queueUrl, string body, Dictionary<string, string> messageAttributes = null);
        Task<SendMessageResponse> SendMessageAsync(string queueUrl, string body, Dictionary<string, string> messageAttributes);
        Task<ReceiveMessageResponse> ReceiveMessageAsync(string _queueUrl, CancellationToken stoppingToken);
        Task DeleteMessageAsync(string _queueUrl, string receiptHandle);
    }
}