using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class RedeColaboradorDTO
    {
        [JsonPropertyName("seguindo")]
        public List<ColaboradorDTO> Seguindo { get; set; }

        [JsonPropertyName("seguidores")]
        public List<ColaboradorDTO> Seguidores { get; set; }
    }
}