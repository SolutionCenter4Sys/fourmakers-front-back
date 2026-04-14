using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_integracao_srs_gcolb
    {
        public string cpf { get; set; }
        public string nome_completo { get; set; }
        public string email { get; set; }
        public DateTime data_nascimento { get; set; }
        public string rg { get; set; }
        public string data_admissao { get; set; }
        public string cep { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public sbyte ativo { get; set; }
        public int diretoria_id { get; set; }
        public string unidade_srs { get; set; }
        public DateTime data_alteracao { get; set; }
    }
}