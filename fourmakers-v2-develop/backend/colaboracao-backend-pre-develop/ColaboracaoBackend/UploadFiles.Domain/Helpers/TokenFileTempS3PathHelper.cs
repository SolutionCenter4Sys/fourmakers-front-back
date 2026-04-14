using System;

namespace UploadFiles.Domain.Helpers;

/// <summary>
/// Monta chaves S3 no padrão <c>arquivo/{base}/{guid}_{timestamp}{ext}</c>, validando apenas o segmento <c>base</c> enviado pelo cliente.
/// </summary>
public static class TokenFileTempS3PathHelper
{
    public const int MaxNomeArquivoLength = 500;

    /// <summary>Primeiro nível fixo da chave (literal <c>arquivo</c>).</summary>
    public const string ArquivoRootFolder = "arquivo";

    /// <summary>Segmento <c>base</c> usado quando o cliente não envia valor.</summary>
    public const string DefaultBaseSegment = "temp";

    /// <summary>Tamanho máximo do segmento <c>base</c>.</summary>
    public const int MaxBaseSegmentLength = 200;

    /// <summary>
    /// Valida o segmento <paramref name="baseFromClient"/> e retorna o prefixo <c>arquivo/{segmento}/</c> ou erro.
    /// </summary>
    public static (string? Prefix, string? ErrorMessage) ResolveArquivoPrefix(string? baseFromClient)
    {
        var segment = string.IsNullOrWhiteSpace(baseFromClient)
            ? DefaultBaseSegment
            : baseFromClient.Trim();

        var validationError = ValidateBaseSegment(segment);
        if (validationError != null)
            return (null, validationError);

        var prefix = $"{ArquivoRootFolder}/{segment}/";
        return (prefix, null);
    }

    /// <summary>
    /// Monta a chave S3 completa <c>{prefix}{guid}_{yyyyMMddHHmmssfff}{ext}</c> e valida o tamanho.
    /// </summary>
    public static (string? S3Key, string? ErrorMessage) BuildObjectKey(string normalizedPrefix, Guid tokenGuid, string extensionWithDot)
    {
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var fileName = $"{tokenGuid:D}_{stamp}{extensionWithDot}";
        var key = normalizedPrefix + fileName;
        if (key.Length > MaxNomeArquivoLength)
            return (null, $"Chave S3 excede {MaxNomeArquivoLength} caracteres (tamanho atual: {key.Length}). Use um segmento base mais curto.");
        return (key, null);
    }

    private static string? ValidateBaseSegment(string segment)
    {
        if (segment.Length > MaxBaseSegmentLength)
            return $"base deve ter no máximo {MaxBaseSegmentLength} caracteres.";

        if (segment.Contains('/') || segment.Contains('\\'))
            return "base não pode conter barras.";

        if (segment.Contains("..", StringComparison.Ordinal))
            return "base não pode conter '..'.";

        foreach (var c in segment)
        {
            if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                return "base permite apenas letras, números, hífen (-) e sublinhado (_).";
        }

        return null;
    }
}
