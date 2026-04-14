using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ApontamentoColaboradorDTO
    {
        [JsonPropertyName("id")]
        public string TbColaboradorApontamentoId { get; set; }

        public long Horas { get; set; }
        public string Justificativa { get; set; }
        public string NomeUsuarioJustificativa { get; set; }
        public DateTime? DataUsuarioJustificativa { get; set; }

        [JsonPropertyName("data_registro")]
        public DateTime? Data { get; set; }

        //public string StatusApontamentoGrupo { get; set; }
        public string CodStatusApontamentoGrupo { get; set; }
        public int? NumeroSemana { get; set; }
        public int? NumeroSemanaDia { get; set; }

        public ProjetoDTO Projeto { get; set; }
        public AtividadeDTO Atividade { get; set; }

        public bool IsWeekend { get => Data?.DayOfWeek == DayOfWeek.Saturday || Data?.DayOfWeek == DayOfWeek.Sunday; }
        public string Observacao { get; set; }
    }
}