using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill
{
    public class GestorExternoPerfilSkillResult : GestorExternoPerfilSkillBase
    {
        public ItemPerfilResult ItemPerfil { get; set; }
        public SkillResult Skill { get; set; }
        public NivelResult Nivel { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}