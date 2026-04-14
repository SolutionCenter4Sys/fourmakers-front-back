namespace MessageQueue.Infra.Infrastructure;

public static class MessageDispatcher
{
    public static MessageQueueProvider To { get; } = new();
}