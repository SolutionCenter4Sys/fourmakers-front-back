using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.ExcluirAlocacao
{
    public class RemoverAlocacoesEmLoteDTO
    {
        [JsonPropertyName("idsPeriodoAlocacao")]
        public List<string> Ids { get; set; }
    }
}