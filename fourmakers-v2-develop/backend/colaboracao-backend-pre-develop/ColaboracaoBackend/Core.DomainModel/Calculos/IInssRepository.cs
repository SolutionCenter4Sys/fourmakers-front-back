using DataTransferObject.Domain.Calculos;
using System.Threading.Tasks;

namespace Core.DomainModel.Calculos
{
    public interface IInssRepository
    {
        Task<InssTabelaDTO> ObterTabelaInssPorAnoAsync(int ano);
        Task<InssTabelaDTO> ObterTabelaInssVigenteAsync();
    }
}
