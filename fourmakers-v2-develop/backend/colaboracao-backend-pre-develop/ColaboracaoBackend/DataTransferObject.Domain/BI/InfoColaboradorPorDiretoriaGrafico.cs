using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class InfoColaboradorPorDiretoriaGrafico
    {
        [JsonPropertyName("diretoria_id")]
        public long Diretoria_Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("quantidade")]
        public long Quantidade { get; set; }
    }
}