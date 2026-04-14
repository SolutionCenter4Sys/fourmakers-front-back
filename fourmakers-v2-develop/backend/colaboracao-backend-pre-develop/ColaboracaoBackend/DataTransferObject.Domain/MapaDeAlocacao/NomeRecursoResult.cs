using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class NomeRecursoResult : StatusResult
    {
        [JsonPropertyName("nomesRecursos")]
        public List<NomeRecursoDTO> NomesRecursos { get; set; }
    }
}