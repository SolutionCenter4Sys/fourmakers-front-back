using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class AnaliseDocumentoSolicitacaoDTO
{
    public DateTime Data { get; set; }
    public string Empresa { get; set; }
    public string Endereco { get; set; }
    public List<AnaliseDocumentoSolicitacaoProdutoDTO> Itens { get; set; }
}

public class AnaliseDocumentoSolicitacaoProdutoDTO
{
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; }
}

public class AnaliseDocumentoSolicitacaoTotalizadorDTO
{
    public List<AnaliseDocumentoSolicitacaoDTO> Analises { get; set; }
    public DateTime? Data { get; set; }
    public decimal? Valor { get; set; }
    public int Quantidade { get; set; }
}