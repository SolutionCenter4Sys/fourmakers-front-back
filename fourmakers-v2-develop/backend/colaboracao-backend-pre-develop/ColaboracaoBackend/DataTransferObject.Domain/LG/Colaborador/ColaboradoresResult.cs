using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG.Colaborador
{
    public class ColaboradoresResult : StatusPaginacaoLGResult
    {
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorLGDTO> Colaboradores { get; set; }
    }
}