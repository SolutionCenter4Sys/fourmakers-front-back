using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class TemplateContratacaoLogDTO
    {
        public Guid Id { get; set; }
        public Guid TemplateContratacaoId { get; set; }
        public string Acao { get; set; }
        public string ColaboradorCodigoInternoColaboradorAlterador { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Objeto { get; set; }
        public string Alteracoes { get; set; }
    }
}
