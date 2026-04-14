using DataTransferObject.Domain;
using System.Threading.Tasks;

namespace Core.Domain.Organograma
{
    public interface IOrganogramaLogRepository
    {
        Task OrganogramaInserirLog(OrganogramaLogDTO param, string tabLog);
     }
}
