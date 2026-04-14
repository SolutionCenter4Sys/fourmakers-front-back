using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class MeusColaboradoresRequestDTO
    {
        public bool? SemFeedback { get; set; }
        public bool? SemOneOnOne { get; set; }
        public bool? SemOneOnOneAcimaDeParametroDias { get; set; }
        public bool? SemFeedbackAcimaDeParametroDias { get; set; }
    }

    public class MeusColaboradoresResponseDTO
    {
        public List<ColaboradorDetalhesDTO> MeusColaboradores { get; set; }
    }

    public class ColaboradorDetalhesDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string CodColaboradorExterno { get; set; }
        public string NomeCompleto { get; set; }
        public string Cargo { get; set; }
        public string Status { get; set; }
        public DateTime? DataUltimoFeedback { get; set; }
        public DateTime? DataUltimoOneOnOne { get; set; }
    }

    public class InserirFeedbackRequestDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public DateTime? DataReuniao { get; set; }
        public string DescricaoContinuar { get; set; }
        public string DescricaoComecar { get; set; }
        public string DescricaoParar { get; set; }
        public string DescricaoObservacoesGerais { get; set; }
    }

    public class InserirOneOnOneRequestDTO
    {
        public string CodigoInternoColaboradorAvaliado { get; set; }
        public DateTime? DataReuniao { get; set; }
        public string DescricaoAnotacoes { get; set; }
        public bool RegistroCritico { get; set; }
    }
}
