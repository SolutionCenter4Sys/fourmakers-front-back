using System.Runtime.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public enum ModalidadeEnum
    {
        [EnumMember(Value = "0")]
        VAZIO,

        [EnumMember(Value = "1")]
        PRESENCIAL,

        [EnumMember(Value = "2")]
        HIBRIDO,

        [EnumMember(Value = "3")]
        REMOTO
    }
}