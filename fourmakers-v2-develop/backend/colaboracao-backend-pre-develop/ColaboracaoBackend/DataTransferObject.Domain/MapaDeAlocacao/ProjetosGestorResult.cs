using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ProjetosColaboradorResult : StatusResult
    {
        [JsonPropertyName("Projetos")]
        public List<ProjetosColaboradorDTO> projetos { get; set; }
    }
}