namespace DataTransferObject.Domain.Apontamento
{
    public class ApontarHorasEmLoteDTO
    {
        public string cpfColaborador { get; set; }
        public string ProjetoId { get; set; }
        public string AtividadeId { get; set; }
        public long Horas { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public int? DiaQuebraSemana { get; set; }
        public bool? DeveSomarApontamentoDia { get; set; }
        public bool IncluirSabado { get; set; }
        public bool IncluirDomingo { get; set; }
        public bool IncluirFeriado { get; set; }
        public string Observacao { get; set; }
    }
}