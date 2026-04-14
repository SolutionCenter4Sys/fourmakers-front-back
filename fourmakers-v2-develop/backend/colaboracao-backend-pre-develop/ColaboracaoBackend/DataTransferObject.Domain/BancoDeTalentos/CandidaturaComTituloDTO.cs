using System;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class CandidaturaComTituloDTO
    {
        public string IdCandidatura { get; set; }
        public string TituloVaga { get; set; }
        public string NomeCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoGestor { get; set; }
        public string NomeGestor { get; set; }
        public string StatusCandidatura { get; set; }
        public string IdVaga { get; set; }
        public long? CodVaga { get; set; }
        public string StatusVaga { get; set; }
        public DateTime? DataCandidatura { get; set; }
    }
} 