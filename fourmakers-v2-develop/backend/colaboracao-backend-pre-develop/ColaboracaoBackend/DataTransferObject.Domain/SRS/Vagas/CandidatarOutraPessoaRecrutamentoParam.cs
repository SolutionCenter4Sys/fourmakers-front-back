using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class CandidatarOutraPessoaRecrutamentoParam
    {
        public int CodigoVaga { get; set; }
        public List<string> OpcoesContatoIds { get; set; }
        public string CodigoColaborador { get; set; }
        public decimal? PretencaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
    }
}