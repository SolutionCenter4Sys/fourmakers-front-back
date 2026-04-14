using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Foursys;
using System;

namespace DataTransferObject.Domain.SRS
{
    public class IndicacaoPremiadaParcialDTO
    {
        public int Id { get; set; }
        public DateTime DataIndicacao { get; set; }
        public string NomeDoFourTalent { get; set; }
        public string EmailFourTalent { get; set; }
        public int VagaId { get; set; }
        public string TituloDaVaga { get; set; }
        public string StatusDaVaga { get; set; }
        public AnalistaResponsavelDTO AnalistaResponasvel { get; set; }
        public string NomeCompletoCandidado { get; set; }
        public string DeOndeConhece { get; set; }
        public bool DisponivelParaParticipar { get; set; }
        public bool AutorizouOEnvioDoCV { get; set; }
        public string LinkedinCandidato { get; set; }
        public UnidadesDTO Diretoria { get; set; }
        public bool CvValido { get; set; }
        public bool RetornoAoProfissional { get; set; }
        public string PerfilAvaliado { get; set; }
        public string Status { get; set; }
    }
}