using DataTransferObject.Domain.Arquivo;

namespace Financeiro.Domain.Interfaces.Reembolso.Solicitacao;

public interface ISolicitacaoReembolsoDocumentoService
{
    /// <summary>
    /// Resolve a chave S3 em <c>tb_token_files_temp</c> pelo token, persiste o documento da solicitação com essa mesma chave
    /// (sem mover objeto no S3) e devolve o token para <c>ExcluirPorTokensAsync</c> após o commit em <c>SolicitacaoReembolsoService.InserirAsync</c>.
    /// </summary>
    /// <returns>URL completa (<c>SERVICE_MIDIA</c> + chave) e o GUID/token enviado pelo cliente.</returns>
    /// <remarks>Fluxo detalhado: <c>docs/reembolso-arquivos-temporarios-s3.md</c>.</remarks>
    Task<(string Path, string Token)> InserirDocumentoAsync(Base64DTO base64, string cpf, int reembolsoId);
}