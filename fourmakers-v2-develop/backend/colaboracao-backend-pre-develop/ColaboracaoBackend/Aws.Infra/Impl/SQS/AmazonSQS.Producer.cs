using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Aws.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aws.Infra.Impl
{
    public class AmazonSQSProducer : IQueueProducer
    {
        private readonly AmazonSQSClient _sqsClient;

        public AmazonSQSProducer()
        {
            var regionName = Environment.GetEnvironmentVariable("AWS_REGION")
                ?? Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");

            _sqsClient = new AmazonSQSClient(RegionEndpoint.GetBySystemName(regionName));
        }

        public bool Produce(string queueUrl, string body, Dictionary<string, string> messageAttributes)
        {
            try
            {
                var attributes = new Dictionary<string, MessageAttributeValue>();

                if (messageAttributes != null)
                {
                    foreach (var attribute in messageAttributes.Keys)
                    {
                        var attributeValue = messageAttributes[attribute];

                        attributes.Add(attribute, new MessageAttributeValue() { StringValue = attributeValue, DataType = "String" });
                    }
                }

                var request = new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = body,
                    MessageAttributes = attributes ?? new Dictionary<string, MessageAttributeValue>()
                };

                var response = _sqsClient.SendMessageAsync(request).Result;
                Console.Write(response);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Neura ao enviar mensagem.: " + ex.Message);
                return false;
            }
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageAsync(string _queueUrl, CancellationToken stoppingToken)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = 10,
                MessageAttributeNames = ["All"]
            };

            return await _sqsClient.ReceiveMessageAsync(request, stoppingToken);
        }

        public async Task DeleteMessageAsync(string _queueUrl, string receiptHandle)
        {
            await _sqsClient.DeleteMessageAsync(_queueUrl, receiptHandle);
        }

        public async Task<SendMessageResponse> SendMessageAsync(string queueUrl, string body, Dictionary<string, string> messageAttributes)
        {
            var attributes = new Dictionary<string, MessageAttributeValue>();

            if (messageAttributes != null)
            {
                foreach (var attribute in messageAttributes.Keys)
                {
                    var attributeValue = messageAttributes[attribute];

                    attributes.Add(attribute, new MessageAttributeValue() { StringValue = attributeValue, DataType = "String" });
                }
            }

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = body,
                MessageAttributes = attributes ?? new Dictionary<string, MessageAttributeValue>()
            };

            return await _sqsClient.SendMessageAsync(request);
        }
    }
}