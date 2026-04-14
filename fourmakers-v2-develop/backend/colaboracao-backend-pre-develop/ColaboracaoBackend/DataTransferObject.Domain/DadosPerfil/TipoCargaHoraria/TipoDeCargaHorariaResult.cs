using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class TipoDeCargaHorariaResult : StatusResult
    {
        [JsonPropertyName("TipoDeCargaHoraria")]
        public List<TipoCargaHorariaDTO> TipoDeCargaHoraria { get; set; }
    }
}