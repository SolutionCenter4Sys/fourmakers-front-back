namespace MessageQueue.Infra.Infrastructure.Interface;

public interface IQueueActions
{
    void EnviaSQS(string body);
}