using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class ListaIdsPorCompetenciaResult : StatusResult
    {
        public ListaIdsPorCompetenciaResult()
        {
            ListaDeIds = new List<long>();
        }

        [JsonPropertyName("listaDeIds")]
        public List<long> ListaDeIds { get; set; }
    }
}