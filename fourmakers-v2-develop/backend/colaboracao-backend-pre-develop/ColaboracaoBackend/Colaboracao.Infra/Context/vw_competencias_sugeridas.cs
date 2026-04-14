using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_competencias_sugeridas
    {
        public long CompetenciaId { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public long? UsuarioCriacaoId { get; set; }
        public string CompetenciaTipo { get; set; }
        public long? qtdUsuariosCompetencia { get; set; }
    }
}