using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class PerfilProfissionalResult : StatusResult
    {
        [JsonPropertyName("perfilProfissional")]
        public PerfilProfissionalDTO PerfilProfissional { get; set; }
    }
}