using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador
{
    public class RubricaColaboradorResult : RubricaColaboradorBase
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Descricao { get; set; }
        public string NomeColaborador { get; set; }
        public string TipoCalculo { get; set; }
        public string FrequenciaDescricao { get; set; }
        public string PeriodoVigencia { get; set; }
    }
}