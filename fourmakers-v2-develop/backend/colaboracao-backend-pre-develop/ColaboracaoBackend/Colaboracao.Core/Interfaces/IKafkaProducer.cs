using System;
using System.Threading.Tasks;

namespace Colaboracao.Core.Interfaces
{
    public interface IKafkaProducer
    {
        Task<bool> Producer(Object requestInfo, string producerKey, string topicIn);
    }
}