using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class GraficoColaboradorPorDiretoriaDTO
    {
        public GraficoColaboradorPorDiretoriaDTO()
        {
            ColaboradorDiretoriaGraf = new List<InfoColaboradorPorDiretoriaGrafico>();
        }

        [JsonPropertyName("colaboradorDiretoriaGraf")]
        public List<InfoColaboradorPorDiretoriaGrafico> ColaboradorDiretoriaGraf { get; set; }
    }
}