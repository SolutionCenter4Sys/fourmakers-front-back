using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        [JsonIgnore]
        public long? UsuarioCriacaoId { get; set; }
        [JsonIgnore]
        public string CpfUsuarioCriacao { get; set; }
        public bool Pendente { get; set; }

        public bool Ativo { get; set; }
    }
}