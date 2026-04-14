#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_candidatos_vaga
    {
        public int Id { get; set; }
        public long? id_vaga { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public long? candidato_id { get; set; }
    }
}