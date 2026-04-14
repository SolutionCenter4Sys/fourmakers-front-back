using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class ListaUnidadesResult : StatusResult
    {
        public ListaUnidadesResult()
        {
            ListaUnidades = new List<UnidadesDTO>();
        }

        [JsonPropertyName("ListaUnidadesResult")]
        public List<UnidadesDTO> ListaUnidades { get; set; }
    }
}