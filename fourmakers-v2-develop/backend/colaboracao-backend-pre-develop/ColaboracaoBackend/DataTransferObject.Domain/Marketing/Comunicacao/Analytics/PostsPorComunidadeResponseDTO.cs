using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class PostsPorComunidadeResponseDTO
    {
        public IReadOnlyList<PostsPorComunidadeItemDTO> Itens { get; set; }
    }
}
