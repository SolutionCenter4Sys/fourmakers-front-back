using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarCandidaturasPorCodCandidatoResult
    {
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public string StatusCandidaturaId { get; set; }
        public string StatusCandidaturaDescricao { get; set; }
        public string CandidaturaId { get; set; }
        public string PretensaoSalarial { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string DisponibilidadeEntrevistaId { get; set; }
        public int? QuantidadeDiasPresencial { get; set; }
        public string? NomeCliente { get; set; }
        public string? CodigoCliente { get; set; }
        public string? CodigoGestor { get; set; }
        public string? NomeGestor { get; set; }
        public string IdVaga { get; set; }
        public string StatusVaga { get; set; }
        public DateTime? DataCandidatura { get; set; }
        public string CodigoInternoColaborador { get; set; }
    }
} 