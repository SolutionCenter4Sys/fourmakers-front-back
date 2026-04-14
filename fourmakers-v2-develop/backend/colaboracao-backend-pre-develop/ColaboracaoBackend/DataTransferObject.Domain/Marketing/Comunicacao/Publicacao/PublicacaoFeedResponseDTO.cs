using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoFeedResponseDTO
    {
        public int QuantidadeTotal { get; set; }
        public int QuantidadePendentesLeitura { get; set; }
        public int QuantidadeLidosAceitos { get; set; }
        public List<PublicacaoLabelResumoDTO> Labels { get; set; } = new List<PublicacaoLabelResumoDTO>();
        public List<PublicacaoTagResumoDTO> Tags { get; set; } = new List<PublicacaoTagResumoDTO>();
        public List<PublicacaoPastaResumoDTO> Pastas { get; set; } = new List<PublicacaoPastaResumoDTO>();
        public List<string> Filtros { get; set; } = new List<string>();
        public List<PublicacaoDetalheDTO> Publicacoes { get; set; } = new List<PublicacaoDetalheDTO>();
    }
}
