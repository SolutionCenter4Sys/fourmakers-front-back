using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class ListarIdiomasResult : StatusResult
    {
        public ListarIdiomasResult()
        {
            ListaIdiomas = new List<IdiomaDTO>();
        }

        [JsonPropertyName("ListaIdiomasResult")]
        public List<IdiomaDTO> ListaIdiomas { get; set; }
    }
}