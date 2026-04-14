using System.Runtime.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public enum FiltroStatusPublicacaoEnum
    {
        [EnumMember(Value = "1")]
        PUBLICA,

        [EnumMember(Value = "0")]
        NAOPUBLICA,

        [EnumMember(Value = "2")]
        TODOS
    }
}