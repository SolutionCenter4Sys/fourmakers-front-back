using System.Threading.Tasks;

namespace Core.Domain.Fourmakers;

public interface IFourmakersRepository
{
    Task InserirAvaliacaoAsync(int orgId, string codigoInternoColaborador, int servicoRate, int recomendacaoRate, string? experienciaDescricao, string aspectoDescricao);
}