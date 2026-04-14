using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorLogDTO
    {
        public Guid Id { get; set; }
        public string ColaboradorCodigoInternoColaborador { get; set; }
        public string Acao { get; set; }
        public string ColaboradorCodigoInternoColaboradorAlterador { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Objeto { get; set; }
        public string Alteracoes { get; set; }
    }
}
