using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class AlterarDependentesResult : StatusResult
    {
        public AlterarDependentesResult()
        {
            AlterarDependentesDTO = new AlterarDependentesDTO();
        }

        [JsonPropertyName("alterarDependentes")]
        public AlterarDependentesDTO AlterarDependentesDTO { get; set; }
    }
}