using Aws.Infra.Interfaces;
using MessageQueue.Infra.AWS.SQS.Module.SRS.Queue.Candidate;
using MessageQueue.Infra.Infrastructure.Interface;

namespace MessageQueue.Infra.AWS.SQS.Module.SRS.Builder;

public class SRSBuilder
{
    private readonly IQueueProducer _queueProducer;

    public SRSBuilder(IQueueProducer queueProducer)
    {
        _queueProducer = queueProducer;
    }

    public IQueueActions Candidate => new CandidateQueue(_queueProducer);
}