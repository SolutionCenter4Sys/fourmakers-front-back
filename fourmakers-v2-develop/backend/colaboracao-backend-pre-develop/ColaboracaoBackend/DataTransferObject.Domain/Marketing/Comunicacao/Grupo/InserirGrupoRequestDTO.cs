using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    public class InserirGrupoRequestDTO
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public bool PermiteCriarPublicacaoOficial { get; set; }
        public bool PublicacaoOficialRequerAprovacao { get; set; }
        public bool AprovaPublicacaoOficial { get; set; }

        public bool PermiteCriarComunidade { get; set; }
        public bool PermiteAcessarAnalytics { get; set; }
        public List<string> CodigoInternoColaboradoresParticipantes { get; set; } = new List<string>();
    }
}
