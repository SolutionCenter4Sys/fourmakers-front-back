using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Candidato
{
    public class CandidaturaArquivosDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("idCandidatura")]
        public string IdCandidatura { get; set; }

        [JsonPropertyName("idComentario")]
        public string IdComentario { get; set; }

        [JsonPropertyName("codColaboradorCriador")]
        public string CodColaboradorCriador { get; set; }

        [JsonPropertyName("dataArquivo")]
        public DateTime? DataArquivo { get; set; }

        [JsonPropertyName("linkArquivo")]
        public string? LinkArquivo { get; set; }
    }
    
    public class CandidaturaArquivosParams
    {
        [JsonPropertyName("idCandidatura")]
        public Guid IdCandidatura { get; set; }
        
        [JsonPropertyName("idComentario")]
        public Guid IdComentario { get; set; }
    
        [JsonPropertyName("bytes")]
        public byte[] bytes { get; set; }
        
        [JsonPropertyName("nomeArquivo")]
        public string NomeArquivo { get; set; }
        
        [JsonPropertyName("tipoArquivo")]
        public string TipoArquivo { get; set; }

    }
}