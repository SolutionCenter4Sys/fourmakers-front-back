using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class FavoritarVagasDTO
    {
        [JsonPropertyName("id_vaga")]
        public long Id_vaga { get; set; }
        [JsonPropertyName("ativo")]
        public sbyte? Ativo { get; set; }
        [JsonPropertyName("tb_usuario_id")]
        public long Tb_usuario_id { get; set; }
        [JsonIgnore]
        [JsonPropertyName("data_criacao")]
        public DateTime? Data_criacao { get; set; }
        [JsonIgnore]
        [JsonPropertyName("data_alteracao")]
        public DateTime? Data_alteracao { get; set; }
    }
}