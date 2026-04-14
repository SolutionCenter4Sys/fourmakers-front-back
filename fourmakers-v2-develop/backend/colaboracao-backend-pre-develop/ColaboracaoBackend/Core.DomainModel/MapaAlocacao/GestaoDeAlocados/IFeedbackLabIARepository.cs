using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{

    public interface IFeedbackLabIARepository
    {
        Task<FeedbackLabIADTO> InserirFeedbackLabIA(FeedbackLabIAParam param, string codigoColaborador);
        Task<List<FeedbackLabIADTO>> ListarFeedbacksLabIa();
    }
}