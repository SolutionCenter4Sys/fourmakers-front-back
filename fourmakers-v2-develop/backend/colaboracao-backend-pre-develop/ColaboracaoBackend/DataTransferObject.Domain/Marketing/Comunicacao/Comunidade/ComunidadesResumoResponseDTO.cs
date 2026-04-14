using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class ComunidadesResumoResponseDTO
    {
        public List<ComunidadeResumoDTO> Comunidades { get; set; } = new List<ComunidadeResumoDTO>();
    }
}
