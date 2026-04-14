using DataTransferObject.Domain.Usuario;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.CCH
{
    public class RecursoValidacaoProjetoResult
    {
        [JsonPropertyName("cdProfissional")]
        public long CdProfissional { get; set; }
        [JsonPropertyName("nmProfissional")]
        public string NmProfissional { set; get; }
        [JsonPropertyName("cdCargo")]
        public int CdCargo { get; set; }
        [JsonPropertyName("nmCargo")]
        public string NmCargo { set; get; }
        [JsonPropertyName("horasTrabalhadas")]
        public string horasTrabalhadas { get; set; }
        [JsonPropertyName("nmEnderecoEletronico")]
        public string EnderecoEletronico { get; set; }
        [JsonPropertyName("flFuncionarioAtivo")]
        public bool flFuncionarioAtivo { get; set; }
        public EnumeradorDeAcesso EnumeradorDeAcesso { get; set; }
    }
}