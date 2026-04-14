#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vaga_competencia
    {
        public long id { get; set; }
        public long? vaga_id { get; set; }
        public long? hardskill_id { get; set; }
        public long? softskill_id { get; set; }
        public long? metodologia_id { get; set; }
        public long? conhecimento_de_negocio_id { get; set; }
        public int? idioma_id { get; set; }
        public long? nivel_competencia_id { get; set; }

        public virtual tb_dominionegocio conhecimento_de_negocio { get; set; }
        public virtual tb_competencia hardskill { get; set; }
        public virtual tb_idioma idioma { get; set; }
        public virtual tb_metodologia metodologia { get; set; }
        public virtual tb_softskill softskill { get; set; }
    }
}