using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class ComunidadeDetalheDTO : ComunidadeResumoDTO
    {
        public List<ComunidadeGrupoResumoDTO> Grupos { get; set; } = new List<ComunidadeGrupoResumoDTO>();
        public List<ComunidadeMembroDTO> Membros { get; set; } = new List<ComunidadeMembroDTO>();
        public List<ComunidadeMembroDTO> Moderadores { get; set; } = new List<ComunidadeMembroDTO>();
    }
}
