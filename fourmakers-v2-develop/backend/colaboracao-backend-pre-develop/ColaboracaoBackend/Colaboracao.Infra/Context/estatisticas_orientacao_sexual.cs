#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class estatisticas_orientacao_sexual
    {
        public long assexual { get; set; }
        public long bissexual { get; set; }
        public long heterossexual { get; set; }
        public long homossexual { get; set; }
        public long prefiro_nao_responder { get; set; }
        public long sem_resposta { get; set; }
    }
}