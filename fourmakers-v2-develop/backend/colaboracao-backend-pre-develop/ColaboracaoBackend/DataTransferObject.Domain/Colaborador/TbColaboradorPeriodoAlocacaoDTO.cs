using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class TbColaboradorPeriodoAlocacaoDTO
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public double QuantidadeHoras { get; set; }
        public int IncluiFimdesemana { get; set; }
        public int Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Observacao { get; set; }
        public string Oportunidade { get; set; }
        public int? Prioritario { get; set; }
        public double? Percentual { get; set; }
        public string CodigoColaborador { get; set; }
        public string CodigoProjeto { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public int TbOrgId { get; set; }
        public int? CodTbdAlocado { get; set; }
        public Guid? TbAtividadeId { get; set; }
    }
}