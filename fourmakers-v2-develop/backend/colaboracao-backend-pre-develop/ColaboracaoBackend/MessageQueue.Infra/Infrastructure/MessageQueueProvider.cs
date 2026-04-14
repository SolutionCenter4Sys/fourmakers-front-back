using MessageQueue.Infra.AWS.SQS.Builder;

namespace MessageQueue.Infra.Infrastructure;

public class MessageQueueProvider
{
    #region providers

    public AmazonSQSBuilder AmazonSQS => AmazonSQSBuilder.GetInstance();

    #endregion providers
}