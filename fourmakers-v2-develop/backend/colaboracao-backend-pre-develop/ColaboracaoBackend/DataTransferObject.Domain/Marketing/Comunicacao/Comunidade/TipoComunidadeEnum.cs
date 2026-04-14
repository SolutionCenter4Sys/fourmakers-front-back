using System.Runtime.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public enum TipoComunidadeEnum
    {
        [EnumMember(Value = "publica")]
        publica,

        [EnumMember(Value = "privada")]
        privada
    }
}
