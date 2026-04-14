using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class CandidaturaRecrutamentoDTO
    {
        public string Id { get; set; }
        public string CodColaborador { get; set; }
        public string IdVaga { get; set; }
        public long OrgId { get; set; }
        public bool Ativa { get; set; }
        public string TituloVaga { get; set; }
        public DateTime? Candidatura { get; set; }
        public string StatusId { get; set; }
        public string StatusDescricao { get; set; }
        public DateTime? UltimaAlteracao { get; set; }
        public decimal? PretencaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
        public string? ColaboradorResponsavel { get; set; }
    }
} 