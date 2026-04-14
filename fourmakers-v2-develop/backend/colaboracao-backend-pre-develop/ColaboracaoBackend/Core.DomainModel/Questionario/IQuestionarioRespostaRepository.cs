using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Questionario;

namespace Core.Domain.Questionario;

public interface IQuestionarioRespostaRepository
{
    Task InserirRespostaAsync(string codigoQuestionario, string textoJson, string codigoInternoColaborador, int orgId, bool processado);
    Task<GenericQuestionarioResult<T>> ObterRespostaPorCodigoAsync<T>(int codigoResposta);
    Task<bool> ExisteCodigoQuestionarioAsync(string codigoQuestionario, int orgId);
    Task<List<string>> ObterCodigosQuestionarioColaboradorAsync(int orgId, string codigoInternoColaborador);
    Task<List<string>> ObterCodigosQuestionariosAtivosAsync(int orgId);
}