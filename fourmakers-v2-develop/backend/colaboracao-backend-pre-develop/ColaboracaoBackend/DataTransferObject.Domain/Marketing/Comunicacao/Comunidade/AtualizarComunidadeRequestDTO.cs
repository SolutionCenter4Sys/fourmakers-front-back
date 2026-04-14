using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class AtualizarComunidadeRequestDTO
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? CapaUrl { get; set; }
        public string Tipo { get; set; }
        public bool PermitePostagemMembro { get; set; }
        public bool PermiteSair { get; set; }
        public string PublicacaoConfiguracaoPolitica { get; set; }
        public bool PublicacaoPermiteComentario { get; set; }
        public bool PublicacaoPermiteLikeHabilitado { get; set; }
        public bool Ativo { get; set; }
        public List<string>? CodigosInternoColaboradoresModeradores { get; set; }
        public List<string>? GruposComunidade { get; set; }
        public List<string>? CodigoInternoColaboradoresParticipantes { get; set; }
    }
}
