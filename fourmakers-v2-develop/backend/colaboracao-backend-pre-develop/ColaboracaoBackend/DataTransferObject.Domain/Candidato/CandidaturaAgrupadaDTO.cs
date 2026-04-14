using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Candidato
{
    public class CandidaturaAgrupadaDTO
    {
        public string IdCandidatura { get; set; }
        public string NomeCandidato { get; set; }
        public long CodVaga { get; set; }
        public string NomeVaga { get; set; }
        public string NomeGestor { get; set; }
        public string NomeCliente { get; set; }
        public decimal? PretencaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string ModeloTrabalhoDescricao { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public string DisponibilidadeEntrevistaDescricao { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public string? DescricaoUltimaAlteracao { get; set; }
        public bool? Qualificado { get; set; }
        public string StatusDaVaga { get; set; }
        public List<CandidaturaAgrupadaAlteracaoDTO> Alteracoes { get; set; } = new List<CandidaturaAgrupadaAlteracaoDTO>();
        public List<DataTransferObject.Domain.ComentarioCandidaturaDTO> Comentarios { get; set; } = new List<DataTransferObject.Domain.ComentarioCandidaturaDTO>();
    }
} 