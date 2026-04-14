using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill
{
    public class GestorExternoPerfilSkillDTO
    {
        public string GestorExternoPerfilId { get; set; }
        public long ItemPerfilId { get; set; }
        public long SkillId { get; set; }
        public long NivelId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public bool Relevante { get; set; }
        public int Id { get; set; }
    }
}