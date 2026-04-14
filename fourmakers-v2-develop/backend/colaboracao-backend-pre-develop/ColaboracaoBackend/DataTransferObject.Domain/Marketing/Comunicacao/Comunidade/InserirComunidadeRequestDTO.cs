using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class InserirComunidadeRequestDTO
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
        public List<string> CodigosInternoColaboradoresModeradores { get; set; } = new List<string>();
        public List<string> GruposComunidade { get; set; } = new List<string>();
        public List<string> CodigoInternoColaboradoresParticipantes { get; set; } = new List<string>();
    }
}
