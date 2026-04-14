using Aws.Infra.Interfaces;
using MessageQueue.Infra.AWS.SQS.Module.SRS.Builder;

namespace MessageQueue.Infra.AWS.SQS.Builder;

public class AmazonSQSBuilder
{
    private static IQueueProducer _queueProducer;
    private static AmazonSQSBuilder _instance;

    public AmazonSQSBuilder()
    { }

    public static void ConfigureQueueProducer(IQueueProducer queueProducer)
    {
        _queueProducer = queueProducer;
    }

    public static AmazonSQSBuilder GetInstance() => _instance ??= new AmazonSQSBuilder();

    #region modules

    public SRSBuilder SRS => new SRSBuilder(_queueProducer);

    #endregion modules
}