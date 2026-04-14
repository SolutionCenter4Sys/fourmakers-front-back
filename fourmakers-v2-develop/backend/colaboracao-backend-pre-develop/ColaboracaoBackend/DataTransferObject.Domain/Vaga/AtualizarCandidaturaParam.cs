using System;

namespace DataTransferObject.Domain.Vaga
{
    public class AtualizarCandidaturaParam
    {
        public string IdCandidatura { get; set; }
        public decimal? PretencaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
    }
} 