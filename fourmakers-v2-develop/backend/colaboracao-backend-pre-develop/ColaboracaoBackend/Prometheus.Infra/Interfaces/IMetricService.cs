namespace Prometheus.Infra.Interfaces
{
    public interface IMetricService
    {
        void StartServer(int port);
        void IncItemConsumed();
        void IncItemConsumedResult(string code);
        ITimer NewDuration();
    }
}