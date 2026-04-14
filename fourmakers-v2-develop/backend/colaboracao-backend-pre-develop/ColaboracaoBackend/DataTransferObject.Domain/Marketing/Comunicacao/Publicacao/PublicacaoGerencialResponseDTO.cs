using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoGerencialResponseDTO
    {
        public int QuantidadeAgendados { get; set; }
        public int QuantidadePendentesAprovacao { get; set; }
        public List<PublicacaoDetalheDTO> Publicacoes { get; set; } = new List<PublicacaoDetalheDTO>();
    }
}
