using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Projeto.GestorExterno
{
    public class GestorExternoResult : GestorExternoBase
    {
        public List<GestorExternoAreaAtuacaoResult> AreasDeAtuacao { get; set; } = new List<GestorExternoAreaAtuacaoResult> { };
        public string CodigoInternoColaborador { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}