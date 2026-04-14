using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoTagResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeDocumentos { get; set; }
    }
}
