using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil
{
    public class GestorExternoPerfilResult : GestorExternoPerfilBase
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public IEnumerable<GestorExternoPerfilSkillResult> GestorExternoPerfilSkills { get; set; } = new List<GestorExternoPerfilSkillResult> { };
        public Guid? IdVaga { get; set; }
    }
}