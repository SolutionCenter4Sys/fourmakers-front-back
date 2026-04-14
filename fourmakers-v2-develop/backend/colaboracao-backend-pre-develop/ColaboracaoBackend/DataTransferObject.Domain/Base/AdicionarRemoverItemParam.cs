using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class AdicionarRemoverItemParam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nivelId")]
        public long? NivelId { get; set; }
        //public long NivelId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("interesseAtivo")]
        public bool InteresseAtivo { get; set; } = false;

        [JsonPropertyName("tipoId")]
        public int TipoId { get; set; }

        [JsonPropertyName("skillId")]
        public int SkillId { get; set; }

        [JsonPropertyName("gestorExternoPerfil")]
        public string GestorExternoPerfil { get; set; }
    }
}