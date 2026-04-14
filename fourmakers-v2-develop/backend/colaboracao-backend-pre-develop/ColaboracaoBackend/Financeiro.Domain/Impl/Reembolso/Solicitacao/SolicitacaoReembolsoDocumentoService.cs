using ApiClient.Domain;
using Aws.Infra.Interfaces.S3;
using Colaboracao.Helper;
using Core.Domain.Reembolso.Solicitacao;
using Core.DomainModel;
using DataTransferObject.Domain.Arquivo;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Solicitacao;

[LogDomainClass]
public class SolicitacaoReembolsoDocumentoService : ISolicitacaoReembolsoDocumentoService
{
    private readonly ITokenFileTempRepository _tokenFileTempRepository;
    private readonly ISolicitacaoReembolsoDocumentoRepository _solicitacaoReembolsoDocumentoRepository;
    private readonly IAmazonS3Uploader _amazonS3Uploader;

    public SolicitacaoReembolsoDocumentoService(
        ITokenFileTempRepository tokenFileTempRepository,
        ISolicitacaoReembolsoDocumentoRepository solicitacaoReembolsoDocumentoRepository,
        IAmazonS3Uploader amazonS3Uploader)
    {
        _tokenFileTempRepository = tokenFileTempRepository;
        _solicitacaoReembolsoDocumentoRepository = solicitacaoReembolsoDocumentoRepository;
        _amazonS3Uploader = amazonS3Uploader;
    }

    public async Task<(string Path, string Token)> InserirDocumentoAsync(Base64DTO base64, string cpf, int reembolsoId)
    {
        ArgumentNullException.ThrowIfNull(base64);
        var token = base64.TokenArquivoTemp?.Trim();

        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token do arquivo temporário é obrigatório para anexos de reembolso.");

        if (!Guid.TryParse(token, out _))
            throw new ArgumentException("Token do arquivo temporário deve ser um GUID válido.");

        var registro = await _tokenFileTempRepository.ObterPorTokenAsync(token);

        if (registro == null || string.IsNullOrWhiteSpace(registro.NomeArquivo))
            throw new ArgumentException("Arquivo temporário não encontrado ou expirado. Envie o comprovante novamente pela API de upload.");

        var s3Key = registro.NomeArquivo.Trim();
        var tipo = ObterTipoDocumentoPorChaveS3(s3Key);
        var basePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
        var path = basePath + s3Key;

        try
        {
            await _solicitacaoReembolsoDocumentoRepository.InserirAsync(path, tipo, s3Key, reembolsoId);
        }
        catch
        {
            try
            {
                await _amazonS3Uploader.DeleteFile(s3Key);
            }
            catch
            {
                // best-effort: evita órfão silencioso sem log estruturado aqui
            }
            throw;
        }
        return (path, token);
    }

    private static string ObterTipoDocumentoPorChaveS3(string relativePath)
    {
        var ext = Path.GetExtension(relativePath).TrimStart('.').ToLowerInvariant();
        return ext switch
        {
            "pdf" => "pdf",
            "png" => "png",
            "jpg" or "jpeg" => "jpg",
            _ => throw new ArgumentException($"Extensão de arquivo não suportada para comprovante: {(string.IsNullOrEmpty(ext) ? "(vazia)" : ext)}. Use pdf, png ou jpg.")
        };
    }
}

