using System.Collections.Generic;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;

namespace DataTransferObject.Domain.MapaDeAlocacao.Perfil
{
    public class ObterEstatisticasMatchSkillSenioridadeResult
    {
        public List<EstatisticaFaixaMatch> EstatisticasPorFaixa { get; set; }
        public int TotalPessoas { get; set; }
        
        public ObterEstatisticasMatchSkillSenioridadeResult()
        {
            EstatisticasPorFaixa = new List<EstatisticaFaixaMatch>();
        }
    }

    public class EstatisticaFaixaMatch
    {
        public string Faixa { get; set; }
        public int QuantidadePessoas { get; set; }
        public double PercentualMinimo { get; set; }
        public double PercentualMaximo { get; set; }
    }
}
