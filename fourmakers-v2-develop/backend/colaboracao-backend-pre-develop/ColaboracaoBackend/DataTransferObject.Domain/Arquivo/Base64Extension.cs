using System;

namespace DataTransferObject.Domain.Arquivo;


public static class Base64DTOExtensions
{
    public static byte[] Base64ToByteArray(this Base64DTO arquivoDTO)
    {
        if (string.IsNullOrWhiteSpace(arquivoDTO?.Base64))
        {
            throw new ArgumentException("Arquivo invalido");
        }

        return Convert.FromBase64String(arquivoDTO.Base64);
    }

    public static string GetTypeName(this Base64DTO arquivoDTO)
    {
        if (arquivoDTO == null)
            throw new ArgumentNullException(nameof(arquivoDTO));

        if (!Enum.IsDefined(typeof(ArquivoTipoEnum), arquivoDTO.Tipo))
            throw new ArgumentOutOfRangeException(nameof(arquivoDTO.Tipo), "Tipo inválido");

        return arquivoDTO.Tipo.ToString();
    }
}
