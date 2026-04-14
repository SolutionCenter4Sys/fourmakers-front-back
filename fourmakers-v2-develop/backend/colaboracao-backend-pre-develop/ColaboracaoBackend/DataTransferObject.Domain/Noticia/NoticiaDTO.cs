using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Noticia
{
    public class NoticiaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("data_criacao")]
        public DateTime Data_criacao { get; set; }

        [JsonPropertyName("imagemPath")]
        public string ImagemPath { get; set; }

        [JsonPropertyName("thumbPath")]
        public string ThumbPath { get; set; }

        [JsonPropertyName("curtidas")]
        public int Curtidas { get; set; }
    }
}