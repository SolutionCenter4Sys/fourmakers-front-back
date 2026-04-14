using System;

namespace DataTransferObject.Domain.SRS.Offboarding
{
    public class OffboardingDTO
    {
        public int id { get; set; }
        public long candidate_id { get; set; }
        public long analista_id { get; set; }
        public int desligado { get; set; }
        public DateTime dataDeslig { get; set; }
        public string analistaDesl { get; set; }
        public int semEntrevistaDeDesligamento { get; set; }
        public string motivoDesligamento { get; set; }
        public string outrosAnotacoesDesligamento { get; set; }
        public string diretoriaDesl { get; set; }
        public string gestorDesl { get; set; }
        public DateTime dataEntrevista { get; set; }
        public string pergunta { get; set; }
        public string resposta { get; set; }
        public string linkFormIndividual { get; set; }
    }
}