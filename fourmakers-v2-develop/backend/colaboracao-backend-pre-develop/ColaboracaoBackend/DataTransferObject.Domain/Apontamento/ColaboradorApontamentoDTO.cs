using System;

namespace DataTransferObject.Domain.Apontamento
{
    public class ColaboradorApontamentoDTO
    {
        public string Id { get; set; }
        public long Horas { get; set; }
        public string Justificativa { get; set; }
        public DateTime Data { get; set; }
        public int? NumeroSemana { get; set; }
        public int? NumeroSemanaDia { get; set; }
        public string AtividadeId { get; set; }
        public string StatusApontamentoId { get; set; }
        public string StatusDescricao { get; set; }
        public int StatusGrupoCodigo { get; set; }
        public string StatusGrupoDescricao { get; set; }
        public string TipoApontamento { get; set; }
        public string VigenciaId { get; set; }
        public string ProjetoCodigo { get; set; }
        public int OrgId { get; set; }
        public string ColaboradorCpf { get; set; }
        public string Observacao { get; set; }

        public string CpfRequest { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string? CodigoColaboradorJustificativa { get; set; } = null;
    }
}