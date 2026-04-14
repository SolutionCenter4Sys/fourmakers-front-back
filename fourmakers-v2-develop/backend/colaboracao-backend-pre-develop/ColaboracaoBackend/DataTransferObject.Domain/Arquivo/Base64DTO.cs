using System;

namespace DataTransferObject.Domain.Arquivo;

public class Base64DTO
{
    public string Base64 { get; set; }
    public ArquivoTipoEnum Tipo { get; set; }

    /// <summary>GUID do registro em tb_token_files_temp (upload prévio na API UploadFiles).</summary>
    public string? TokenArquivoTemp { get; set; }
}