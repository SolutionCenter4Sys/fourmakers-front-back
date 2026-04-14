using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class GravarPerdaVagaParam
    {
        [JsonPropertyName("codigoVaga")]
        public int CodigoVaga { get; set; }

        [JsonPropertyName("idMotivoPerda")]
        public Guid? IdMotivoPerda { get; set; }

        [JsonPropertyName("comentario")]
        public string Comentario { get; set; }
    }
}

