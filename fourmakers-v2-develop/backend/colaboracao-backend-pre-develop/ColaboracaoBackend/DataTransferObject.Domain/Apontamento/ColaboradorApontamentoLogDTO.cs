using System;

namespace DataTransferObject.Domain.Apontamento
{
    public class ColaboradorApontamentoLogDTO
    {
        public string Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public string ColaboradorApontamentoId { get; set; }
        public string? TbStatusApontamentoAnteriorId { get; set; }
        public string TbStatusApontamentoNovoId { get; set; }
        public string? Justificativa { get; set; }
        public long? HorasAnterior { get; set; }
        public long? HorasNovo { get; set; }
        public long? HorasReprovadas { get; set; }
        public string TbVigenciaId { get; set; }
        public string TbProjetoOrgCodProjeto { get; set; }
        public int TbOrgId { get; set; }
        public string TbColaboradorOrgTbColaboradorCpf { get; set; }
        public int? NumeroSemana { get; set; }
        public int? NumeroSemanaDia { get; set; }
        public string TbAtividadeId { get; set; }
        public string TbColaboradorCpfCriacao { get; set; }
    }
}