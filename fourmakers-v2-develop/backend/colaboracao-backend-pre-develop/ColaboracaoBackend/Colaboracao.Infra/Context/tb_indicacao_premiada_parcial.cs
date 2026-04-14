using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_indicacao_premiada_parcial
    {
        public long id { get; set; }
        public long id_usuario_indicou { get; set; }
        public long id_vaga { get; set; }
        public string link_indicacao { get; set; }
        public string nome_indicado { get; set; }
        public string email_indicado { get; set; }
        public string telefone_indicado { get; set; }
        public string linkedin { get; set; }
        public string relacao_indicado { get; set; }
        public sbyte disponivel { get; set; }
        public sbyte autorizou { get; set; }
        public DateTime? data_criacao { get; set; }
        public sbyte utilizado { get; set; }
        public string path_curriculo { get; set; }
        public string codigo_colaborador_interno_analista { get; set; }
        public string cod_diretoria { get; set; }
        public bool cv_valido { get; set; }
        public bool retorno_ao_profissional { get; set; }
        public string perfil_avaliado { get; set; }
        public string status { get; set; }
    }
}