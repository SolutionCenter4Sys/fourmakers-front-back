using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class ComunidadeGrupoResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        /// <summary>
        /// Usuários do grupo vinculados à comunidade (via <c>tb_mkt_grupo_usuario</c>). Cada item usa <see cref="ComunidadeFormaParticipacaoUsuario.UsuarioPeloGrupo"/>.
        /// </summary>
        public List<ComunidadeMembroDTO> Membros { get; set; } = new List<ComunidadeMembroDTO>();
    }
}
