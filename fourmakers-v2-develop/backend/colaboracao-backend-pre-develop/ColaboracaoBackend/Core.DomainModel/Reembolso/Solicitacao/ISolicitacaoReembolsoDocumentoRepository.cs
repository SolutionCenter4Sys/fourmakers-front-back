using System.Threading.Tasks;

namespace Core.Domain.Reembolso.Solicitacao;

public interface ISolicitacaoReembolsoDocumentoRepository
{
    Task InserirAsync(string url, string tipo, string relativePath, int reembolsoId);
}