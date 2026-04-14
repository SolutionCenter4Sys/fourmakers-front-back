using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador
{
    public class RubricaColaboradorDetalhadoDTO
    {
        public string Id { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Percentual { get; set; }
        public string Hora { get; set; }
        public string CodigInternoColaborador { get; set; }
        public string CodigoRubricaFrequencia { get; set; }
        public RubricaInfoDTO Rubrica { get; set; } = new RubricaInfoDTO();
    }

    public class RubricaInfoDTO
    {
        public string RubricaId { get; set; }
        public string RubricaDescricao { get; set; }
        public string CodigoRubrica { get; set; }
        public RubricaTipoInfoDTO RubricaTipo { get; set; } = new RubricaTipoInfoDTO();
    }

    public class RubricaTipoInfoDTO
    {
        public string Descricao { get; set; }
        public string CodigoRubricaTipo { get; set; }
        public string Natureza { get; set; }
    }
} 