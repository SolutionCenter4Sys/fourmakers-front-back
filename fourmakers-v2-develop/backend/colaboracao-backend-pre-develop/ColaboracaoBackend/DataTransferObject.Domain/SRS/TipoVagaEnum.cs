using System.Runtime.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public enum TipoVagaEnum
    {
        [EnumMember(Value = "0")]
        PADRAO,

        [EnumMember(Value = "1")]
        ESTRATEGICA
    }
}