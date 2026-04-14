using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoContabil
{
    public class RemessaContabilRegistroOutrosDTO : RemessaContabilRegistroDTO
    {
        public string Competencia { get; set; }
    }

}
