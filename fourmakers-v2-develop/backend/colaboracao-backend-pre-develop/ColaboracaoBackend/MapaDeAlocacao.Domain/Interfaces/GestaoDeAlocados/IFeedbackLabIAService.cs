using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    
    public interface IFeedbackLabIAService
    {
        Task<ApiGenericResult<FeedbackLabIADTO>> InserirFeedbackLabIA(FeedbackLabIAParam param, string codigoColaborador);
        
        Task<ApiGenericResult<List<FeedbackLabIADTO>>> ListarFeedbacksLabIa();
    }
}

