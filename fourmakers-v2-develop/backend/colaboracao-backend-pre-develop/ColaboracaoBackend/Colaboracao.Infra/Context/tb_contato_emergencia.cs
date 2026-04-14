using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_contato_emergencia
    {
        public Guid id { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public string grau_parentesco { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
    }
}