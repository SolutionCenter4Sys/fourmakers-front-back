using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class ComunidadeMembroDTO
    {
        public Guid Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string UrlFoto { get; set; }

        /// <summary>
        /// <see cref="ComunidadeDetalheDTO.Membros"/>: <see cref="ComunidadeFormaParticipacaoUsuario.UsuarioIndividual"/> (apenas <c>tb_mkt_comunidade_usuario_participando</c>).
        /// <see cref="ComunidadeGrupoResumoDTO.Membros"/>: <see cref="ComunidadeFormaParticipacaoUsuario.UsuarioPeloGrupo"/>.
        /// Moderadores não preenchem.
        /// </summary>
        public string FormaParticipacaoUsuario { get; set; }
    }
}
