using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH
{
    public class DashboardRHDTO
    {
        public int QtdTotalColaboradores { get; set; }
        public decimal PorcentagemGestoresOneOnOneEmDia { get; set; }
        public decimal PorcentagemGestoresFeedbackEmDia { get; set; }
        public int QtdSemOneOnOneHaMaisQtdParametroDias { get; set; }
        public int QtdSemFeedbackHaMaisQtdParametroDias { get; set; }
    }

    public class ListaColaboradoresRequestDTO
    {
        public bool? SemFeedback { get; set; }
        public bool? SemOneOnOne { get; set; }
        public bool? SemOneOnOneDias { get; set; }
        public bool? SemFeedbackDias { get; set; }
    }

    public class ListaColaboradoresResponseDTO
    {
        public List<ColaboradorRHDTO> Colaboradores { get; set; }
    }

    public class ColaboradorRHDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string Cargo { get; set; }
        public List<string> NomesColaboradoresSuperiores { get; set; }
        public string Status { get; set; }
        public DateTime? DataUltimoFeedback { get; set; }
        public DateTime? DataUltimoOneOnOne { get; set; }
    }

    public class InserirParametrizacaoRequestDTO
    {
        public int? FrequenciaEsperadaOneOnOneDias { get; set; }
        public int? FrequenciaEsperadaFeedbackDias { get; set; }
    }
}
