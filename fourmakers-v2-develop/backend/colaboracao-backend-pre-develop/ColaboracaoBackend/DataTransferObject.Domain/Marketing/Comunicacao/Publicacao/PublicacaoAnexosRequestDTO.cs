using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoAnexosRequestDTO
    {
        public List<PublicacaoAnexoInputDTO> Anexos { get; set; } = new List<PublicacaoAnexoInputDTO>();
    }
}
