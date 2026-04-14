using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using DataTransferObject.Domain.Util;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill
{
    public class GestorExternoPerfilSkillInput : GestorExternoPerfilSkillBase
    {
        public ItemPerfilInput ItemPerfil { get; set; }
        public SkillInput Skill { get; set; }
        public NivelInput Nivel { get; set; }

        [JsonIgnore]
        public Guid GestorExternoPerfilId { get; set; }
        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        public void AtualizarPropriedadesDaClasseBase(GestorExternoPerfilSkillBase gestorExternoPerfilSkillBase)
        {
            this.AtualizarSafeComPropriedadesDe(gestorExternoPerfilSkillBase);
        }

        public void ConfigurarParaPersistencia(Guid gestorExternoPerfilId, string codigoInternoColaboradorAlteracao = null)
        {
            GestorExternoPerfilId = gestorExternoPerfilId;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;
        }
    }
}