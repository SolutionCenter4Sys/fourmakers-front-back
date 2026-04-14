using System.Runtime.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public enum PublicacaoConfiguracaoPoliticaEnum
    {
        [EnumMember(Value = "forcar-configuracao-comunidade")]
        ForcarConfiguracaoComunidade,

        [EnumMember(Value = "sugerir")]
        Sugerir,

        [EnumMember(Value = "desativado")]
        Desativado
    }
}
