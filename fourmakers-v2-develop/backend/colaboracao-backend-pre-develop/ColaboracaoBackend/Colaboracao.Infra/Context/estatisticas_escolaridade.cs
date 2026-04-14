#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class estatisticas_escolaridade
    {
        public long mestrado_doutorado_incompleto_ou_cursando { get; set; }
        public long mestrado_doutorado_completo { get; set; }
        public long ensino_fundamental_completo_menos { get; set; }
        public long ensino_medio_completo_menos { get; set; }
        public long ensino_superior_incompleto_ou_cursando { get; set; }
        public long ensino_superior_completo { get; set; }
        public long pos_graduacao_incompleto_ou_cursando { get; set; }
        public long pos_graduacao_completo { get; set; }
        public long sem_resposta { get; set; }
        public long outros { get; set; }
    }
}