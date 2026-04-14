using System;
using System.Text.Json;
using DataTransferObject.Domain.Vaga;

namespace DataTransferObject.Domain.Candidato
{
    public class CandidaturaLogDTO
    {
        public string IdCandidatura { get; set; }
        public int IdStatus { get; set; }
        public string StatusDescricao { get; set; }
        public string Recrutador { get; set; }
        public string NomeCandidato { get; set; }
        public int CodVaga { get; set; }
        public string NomeVaga { get; set; }
        public string NomeGestor { get; set; }
        public string Objeto { get; set; }
        public string RecrutadorResponsavel { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
} 