using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class NomesGestoresResult : StatusResult
    {
        [JsonPropertyName("Gestor")]
        public List<GestorDTO> Gestores { get; set; }
    }
}