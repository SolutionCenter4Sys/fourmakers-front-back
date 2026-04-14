using DataTransferObject.Domain.Calculos;
using System.Threading.Tasks;

namespace Core.DomainModel.Calculos
{
    public interface IIrrfReducaoRepository
    {
        Task<IrrfReducaoDTO> ObterReducaoPorAnoAsync(int ano);
        Task<IrrfReducaoDTO> ObterReducaoVigenteAsync();
    }
}

