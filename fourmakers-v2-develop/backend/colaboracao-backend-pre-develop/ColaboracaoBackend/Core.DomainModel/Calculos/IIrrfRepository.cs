using DataTransferObject.Domain.Calculos;
using System.Threading.Tasks;

namespace Core.DomainModel.Calculos
{
    public interface IIrrfRepository
    {
        Task<IrrfTabelaDTO> ObterTabelaIrrfPorAnoAsync(int ano);
        Task<IrrfTabelaDTO> ObterTabelaIrrfVigenteAsync();
    }
}
