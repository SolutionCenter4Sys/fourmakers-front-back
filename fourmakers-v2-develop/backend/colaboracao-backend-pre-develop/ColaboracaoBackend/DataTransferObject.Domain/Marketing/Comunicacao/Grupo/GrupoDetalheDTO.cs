using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Marketing.Comunicacao;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    public class GrupoDetalheDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public bool PermiteCriarPublicacaoOficial { get; set; }
        public bool PublicacaoOficialRequerAprovacao { get; set; }
        public bool AprovaPublicacaoOficial { get; set; }

        public bool PermiteCriarComunidade { get; set; }
        public bool PermiteAcessarAnalytics { get; set; }
        public List<ColaboradorResumoDTO> ColaboradoresParticipantes { get; set; } = new List<ColaboradorResumoDTO>();
    }
}
