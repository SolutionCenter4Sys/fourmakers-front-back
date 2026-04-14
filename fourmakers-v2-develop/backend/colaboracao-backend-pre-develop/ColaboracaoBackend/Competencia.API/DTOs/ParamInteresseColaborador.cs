using Competencia.Domain.Enums;
using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ParamInteresseColaborador
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("tipoId")]
        public ItemPerfilEnum TipoId { get; set; }

        [JsonPropertyName("skillId")]
        public int SkillId { get; set; }

        [JsonPropertyName("nivelId")]
        public long NivelId { get; set; }

        [JsonPropertyName("gestorExternoPerfil")]
        public string GestorExternoPerfil { get; set; }

        [JsonPropertyName("interesseAtivo")]
        public bool InteresseAtivo { get; set; } = false;
    }
}