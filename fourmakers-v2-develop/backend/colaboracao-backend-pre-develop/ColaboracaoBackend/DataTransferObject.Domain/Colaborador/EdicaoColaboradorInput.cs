using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class EdicaoColaboradorInput
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("codDiretoria")]
        public string CodDiretoria { get; set; }

        [JsonPropertyName("diretoria")]
        public string Diretoria { get; set; }

        [JsonPropertyName("codDepartamento")]
        public string CodDepartamento { get; set; }

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }

        [JsonPropertyName("dataAdmissao")]
        public DateTime DataAdmissao { get; set; }
    }
}