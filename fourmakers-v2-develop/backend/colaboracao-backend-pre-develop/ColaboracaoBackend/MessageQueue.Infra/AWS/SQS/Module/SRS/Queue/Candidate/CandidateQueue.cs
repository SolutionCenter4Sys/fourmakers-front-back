using Aws.Infra.Interfaces;
using Colaboracao.Helper;
using MessageQueue.Infra.Infrastructure.Interface;

namespace MessageQueue.Infra.AWS.SQS.Module.SRS.Queue.Candidate;

public class CandidateQueue : IQueueActions
{
    private readonly IQueueProducer _queueProducer;

    public CandidateQueue(IQueueProducer queueProducer)
    {
        _queueProducer = queueProducer;
    }

    public void EnviaSQS(string body)
    {
        _queueProducer.Produce(
            VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_URL_BANCOTALENTOS_SRS"),
            body
        );
    }

    public void EnviaBancoTalentoSQS(string body)
    {
        _queueProducer.Produce(
            VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(ApiClient.Domain.EnvironmentVariables.AWS_SQS_QUEUE_URL_CANDIDATO),
            body
        );
    }
}