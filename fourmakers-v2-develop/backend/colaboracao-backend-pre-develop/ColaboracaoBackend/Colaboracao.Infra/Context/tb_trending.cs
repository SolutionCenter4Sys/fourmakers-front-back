using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_trending
    {
        public long id { get; set; }
        public DateTime data_busca { get; set; }
        public string resultado_busca { get; set; }
    }
}