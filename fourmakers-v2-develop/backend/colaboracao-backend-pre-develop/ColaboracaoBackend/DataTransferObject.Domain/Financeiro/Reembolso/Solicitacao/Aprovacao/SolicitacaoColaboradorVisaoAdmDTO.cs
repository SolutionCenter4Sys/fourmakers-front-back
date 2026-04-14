using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoColaboradorVisaoAdmDTO
{
        public int Id { get; set; }
        public string CodigoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public decimal ValorSolicitado { get; set; }
        public decimal? ValorAprovado { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Projeto { get; set; }
        public string Cliente { get; set; }
        public string Observacao { get; set; }
        public string Descricao { get; set; }
        public int TotalDespesas { get; set; }
        public string NomeAprovador { get; set; }
        public decimal? ValorExcecao { get; set; }
        public bool? CustoClienteExcecao { get; set; }
        public string Objetivo { get; set; }
        public string? Destino { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Categoria { get; set;  }
        public List<SolicitacaoDocumentoDTO> SolicitacaoDocumentos { get; set; } = [];
}