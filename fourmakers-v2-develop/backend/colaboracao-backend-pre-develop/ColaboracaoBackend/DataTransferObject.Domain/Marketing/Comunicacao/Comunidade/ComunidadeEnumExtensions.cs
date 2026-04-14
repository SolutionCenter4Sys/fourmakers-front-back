using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public static class ComunidadeEnumExtensions
    {
        public static string ToDbValue(this PublicacaoConfiguracaoPoliticaEnum value)
        {
            return value switch
            {
                PublicacaoConfiguracaoPoliticaEnum.ForcarConfiguracaoComunidade => "forcar-configuracao-comunidade",
                PublicacaoConfiguracaoPoliticaEnum.Sugerir => "sugerir",
                PublicacaoConfiguracaoPoliticaEnum.Desativado => "desativado",
                _ => throw new ArgumentException($"Valor invalido para PublicacaoConfiguracaoPolitica: {value}")
            };
        }

        public static PublicacaoConfiguracaoPoliticaEnum ParsePolitica(string value)
        {
            return value?.ToLower() switch
            {
                "forcar-configuracao-comunidade" => PublicacaoConfiguracaoPoliticaEnum.ForcarConfiguracaoComunidade,
                "sugerir" => PublicacaoConfiguracaoPoliticaEnum.Sugerir,
                "desativado" => PublicacaoConfiguracaoPoliticaEnum.Desativado,
                _ => throw new ArgumentException($"Valor invalido para PublicacaoConfiguracaoPolitica: '{value}'. Valores aceitos: 'forcar-configuracao-comunidade', 'sugerir', 'desativado'.")
            };
        }
    }
}
