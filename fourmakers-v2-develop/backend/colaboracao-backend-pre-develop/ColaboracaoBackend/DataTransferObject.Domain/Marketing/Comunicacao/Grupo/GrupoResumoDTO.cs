using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    public class GrupoResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public bool PermiteCriarPublicacaoOficial { get; set; }
        public bool PublicacaoOficialRequerAprovacao { get; set; }
        public bool AprovaPublicacaoOficial { get; set; }

        public bool PermiteCriarComunidade { get; set; }
        public bool PermiteAcessarAnalytics { get; set; }
        public string Status { get; set; }
        public int QuantidadeParticipantes { get; set; }
        public List<string> IniciaisMembros { get; set; } = new List<string>();
    }
}
