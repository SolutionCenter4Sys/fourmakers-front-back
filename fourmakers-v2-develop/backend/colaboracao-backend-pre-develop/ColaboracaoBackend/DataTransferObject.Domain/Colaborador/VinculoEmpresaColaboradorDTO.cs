using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class VinculoEmpresaColaboradorDTO
    {
        [JsonPropertyName("nome_fantasia")]
        public string NomeFantasia { get; set; }
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }
        [JsonPropertyName("confirmado")]
        public bool Confirmado { get; set; }
        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }
        [JsonPropertyName("data_convite")]
        public DateTime DataConvite { get; set; }
    }
}