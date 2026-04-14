using Competencia.Domain.Enums;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia.Enums;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;

namespace DataTransferObject.Domain.Competencia.GestaoDeCompetencia
{
    public class NivelMigracaoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public TipoCompetenciaSRSEnum Tipo { get; set; }
        public ItemPerfilEnum TipoItemPerfil { get; set; }
        public CategoriaSkillEnum categoriaSkillSRS { get; set; }
        public NivelSenioridadeMigracaoEnum NivelEquivalencia { get; set; }

        public NivelMigracaoDTO(int id, string descricao, TipoCompetenciaSRSEnum tipo, ItemPerfilEnum itemPerfil, CategoriaSkillEnum categoriaSrs, NivelSenioridadeMigracaoEnum nivelEquivalencia)
        {
            Id = id;
            Descricao = descricao;
            Tipo = tipo;
            TipoItemPerfil = itemPerfil;
            categoriaSkillSRS = categoriaSrs;
            NivelEquivalencia = nivelEquivalencia;
        }
    }
}