#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class estatisticas_genero
    {
        public long feminino { get; set; }
        public long masculino { get; set; }
        public long nao_binario { get; set; }
        public long homem_cisgenero { get; set; }
        public long Agenero { get; set; }
        public long transgenero { get; set; }
        public long mulher_cisgenero { get; set; }
        public long prefiro_nao_responder { get; set; }
        public long sem_resposta { get; set; }
    }
}