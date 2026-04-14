using System;

namespace DataTransferObject.Domain.Vaga
{
    public class LogPretensaoModeloCandidaturaDTO
    {
        public string Id { get; set; }
        public string IdCandidatura { get; set; }
        public string IdVaga { get; set; }
        public string TituloVaga { get; set; }
        public string CodigoInternoCandidato { get; set; }
        public string NomeCandidato { get; set; }
        public string TipoOperacao { get; set; }
        public decimal? PretensaoSalarialValorAnterior { get; set; }
        public decimal? PretensaoSalarialValorNovo { get; set; }
        public string ModeloTrabalhoIdAnterior { get; set; }
        public string ModeloTrabalhoIdNovo { get; set; }
        public string ModeloTrabalhoDescricaoAnterior { get; set; }
        public string ModeloTrabalhoDescricaoNovo { get; set; }
        public string CodigoInternoExecutor { get; set; }
        public string NomeExecutor { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string OrigemAlteracao { get; set; }
    }
}
