using Prometheus.Infra.Interfaces;

namespace Prometheus.Infra.Impl
{
    public class MetricService : IMetricService
    {
        private readonly Counter ItemConsumed = Metrics.CreateCounter("item_consumed", "Itens consumidos pelo tópico");
        private readonly Counter ItemConsumedResult = Metrics.CreateCounter("item_consumed_result", "Resultado dos itens consumidos",
        new CounterConfiguration
        {
            LabelNames = new[] { "code" }
        });
        private readonly Histogram ConsumingDuration = Metrics.CreateHistogram("item_consumed_duration", "Duração do consumo de cada item");
        private MetricServer metricServer;

        //public MetricService()
        //{
        //    StartServer(30150);
        //}

        public void StartServer(int port)
        {
            metricServer = new MetricServer(port: port);
            metricServer.Start();
        }

        public void IncItemConsumed()
        {
            ItemConsumed.Inc();
        }

        public void IncItemConsumedResult(string code)
        {
            ItemConsumedResult.WithLabels(code).Inc();
        }

        public ITimer NewDuration()
        {
            return ConsumingDuration.NewTimer();
        }
    }
}