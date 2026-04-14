using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.Util;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil
{
    public class GestorExternoPerfilInput : GestorExternoPerfilBase
    {
        public List<GestorExternoPerfilSkillInput> GestorExternoPerfilSkills { get; set; } = new List<GestorExternoPerfilSkillInput> { };

        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }
        [JsonIgnore]
        public bool Ativo { get; set; }
        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        public void AtualizarPropriedadesDaClasseBase(GestorExternoPerfilBase gestorExternoPerfilBase)
        {
            this.AtualizarSafeComPropriedadesDe(gestorExternoPerfilBase);
        }

        public void ConfigurarParaPersistencia(int orgId, string codigoInternoColaboradorAlteracao, Guid id)
        {
            Ativo = true;
            OrgId = orgId;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;
            Id = id;
        }
    }
}