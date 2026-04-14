using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia.MetodologiaClient
{
    public class ListaMetodologiaColaboradorResultClient : StatusResult
    {
        public ListaMetodologiaColaboradorResultClient()
        {
            Metodologias = new List<MetodologiaColaboradorDTO>();
        }

        [JsonPropertyName("retorno")]
        public List<MetodologiaColaboradorDTO> Metodologias { get; set; }
    }
}